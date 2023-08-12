using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.OAuth;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordOAuthClient : IDiscordOAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IConfiguration _configuration;
    private string _clientId;
    private string _clientSecret;
    private string _redirectUrl;
    private readonly IDiscordChannelClient _discordChannelClient;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public DiscordOAuthClient(HttpClient httpClient, IConfiguration configuration, IDiscordChannelClient discordChannelClient)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _discordChannelClient = discordChannelClient;
        _baseUrl = "https://discord.com/api/oauth2/";
        _clientId = _configuration.BetsyClientId();
        _clientSecret = _configuration.BetsyClientSecret();
        _redirectUrl = _configuration.RedirectUrl();
        SetupHttpClient();
    }

    public async Task<AccessTokenResponse?> ExchangeCodeForToken(string code, string? redirectUrl)
    {
        var url = _baseUrl + "token";
        var content = new List<KeyValuePair<string, string>>
        {
            new("client_id", _clientId),
            new("client_secret", _clientSecret),
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", redirectUrl ?? _redirectUrl)
        };
        var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(content));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "ExchangeCodeForToken");
        }
        var result = JsonConvert.DeserializeObject<AccessTokenResponse>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<AccessTokenResponse?> RefreshToken(string refreshToken)
    {
        var url = _baseUrl + "/token";
        var content = new List<KeyValuePair<string, string>>
        {
            new("client_id", _clientId),
            new("client_secret", _clientSecret),
            new("grant_type", "refresh_token"),
            new("refresh_token", refreshToken)
        };
        var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(content));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "RefreshToken");
        }
        var result = JsonConvert.DeserializeObject<AccessTokenResponse>(body, _jsonSerializerSettings);
        return result;
    }

    private void SetAuthorizationHeader(string discordAccessToken)
    {
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + discordAccessToken);
    }

    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-formurlencoded"));
    }
}