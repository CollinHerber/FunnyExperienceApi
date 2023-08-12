using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Cowbot.Server.ThirdPartyServices.Topgg.Interfaces;
using Cowbot.Server.ThirdPartyServices.Topgg.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Cowbot.Server.ThirdPartyServices.Topgg;

public class TopggClient : ITopggClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly string _topggJwt;
    private readonly string _topggBotId;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public TopggClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = "https://top.gg/api/";
        _configuration = configuration;
        _topggBotId = _configuration.TopggBotId();
        _topggJwt = _configuration.TopggJwtToken();
        SetupHttpClient();
    }

    public async Task PostBetsyBotStats(TopggPostStatRequest request)
    {
        var url = _baseUrl + "bots/" + _topggBotId + "/stats";
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
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _topggJwt);
    }
}