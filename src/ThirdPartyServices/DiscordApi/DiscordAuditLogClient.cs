using Microsoft.Extensions.Configuration;
using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.ApplicationCommands;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using System.Text;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordAuditLogClient : IDiscordAuditLogClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly string _botToken;
    private readonly string _clientId;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public DiscordAuditLogClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = "https://discord.com/api/v10/guilds/";
        _configuration = configuration;
        _botToken = _configuration.BetsyBotToken();
        _clientId = _configuration.BetsyClientId();
        SetupHttpClient();
        SetBotAuthorizationHeader();
    }

    private void SetBotAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bot " + _botToken);
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
    }
}