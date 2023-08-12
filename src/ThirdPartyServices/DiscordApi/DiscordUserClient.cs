using Cowbot.Server.Lib;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordUser;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Guild;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Cowbot.Server.Lib.Extensions;
using Microsoft.Extensions.Configuration;
using Cowbot.Server.Configuration;
using System.Text;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordUserClient : IDiscordUserClient
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;
    private readonly string _baseUrl;
    private readonly string _botClientId;
    private readonly IConfiguration _configuration;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public DiscordUserClient(HttpClient httpClient, IDistributedCache cache, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _cache = cache;
        _baseUrl = "https://discord.com/api/v10/users/@me";
        _configuration = configuration;
        _botClientId = _configuration.BetsyBotToken();
        SetupHttpClient();
    }

    public async Task<DiscordUser?> GetCurrentUser(string discordAccessToken)
    {
        return await _cache.GetOrSetNullableAsWithHourlyExpirationAsync(GetDiscordUserString(discordAccessToken), 4, async () => {
            SetAuthorizationHeader(discordAccessToken);
            var response = await _httpClient.GetAsync(_baseUrl);
            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<DiscordUser>(body, _jsonSerializerSettings);
            return result;
        });
    }

    public async Task<List<DiscordGuild?>> GetUsersGuilds(string disordAccessToken)
    {
        SetAuthorizationHeader(disordAccessToken);
        var url = $"{_baseUrl}/guilds";
        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<DiscordGuild>>(body, _jsonSerializerSettings);
            return result;
        }
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            //TODO: Handle refresh token logic
            throw new UnauthorizedAccessException("Expired Token? Please relog");
        }
        return null;
    }

    public async Task<DiscordChannel?> CreateDmChannel(CreateDmChannelRequest request, string? botOverride = "")
    {
        if (!string.IsNullOrEmpty(botOverride))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bot {botOverride}");
        }
        else
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bot " + _botClientId);   
        }
        var url = $"{_baseUrl}/channels";
        var response = await _httpClient.PostAsync(url, CreateRequestObject(request));
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<DiscordChannel>(body, _jsonSerializerSettings);
        return result;
    }

    private static string GetDiscordUserString(string accessToken)
    {
        return string.Format(Constants.CacheKeys.DiscordUser, accessToken);
    }

    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private void SetAuthorizationHeader(string discordAccessToken)
    {
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + discordAccessToken);
    }

    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}