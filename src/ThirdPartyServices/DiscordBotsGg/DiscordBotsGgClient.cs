using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Cowbot.Server.ThirdPartyServices.Topgg.Interfaces;
using Cowbot.Server.ThirdPartyServices.Topgg.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Cowbot.Server.ThirdPartyServices.Topgg;

public class DiscordBotsGgClient : IDiscordBotsGgClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly string _jwtToken;
    private readonly string _botId;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public DiscordBotsGgClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = "https://discord.bots.gg/api/v1/";
        _configuration = configuration;
        _botId = _configuration.BetsyClientId();
        _jwtToken = _configuration.TopggJwtToken();
        SetupHttpClient();
    }

    public async Task PostBetsyBotStats(DiscordBotsGgPostStatRequest request)
    {
        var url = _baseUrl + "bots/" + _botId + "/stats";
        await _httpClient.PostAsync(url, CreateRequestObject(request));
    }

    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _jwtToken);
    }
}