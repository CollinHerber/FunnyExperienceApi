using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Cowbot.Server.Models.IpQualityScoreModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cowbot.Server.ThirdPartyServices.IpQualityScore;
public class IPQualityScoreClient : IIpQualityScoreClient
{

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    private readonly string _ipQualityUri;
    private readonly string _phoneQualityUri;
    private readonly string _emailQualityUri;

    public IPQualityScoreClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _ipQualityUri = "https://www.ipqualityscore.com/api/json/ip/" + configuration.GetIpQualityScoreApiKey();
        _phoneQualityUri = "https://www.ipqualityscore.com/api/json/phone/" + configuration.GetIpQualityScoreApiKey();
        _emailQualityUri = "https://www.ipqualityscore.com/api/json/email/" + configuration.GetIpQualityScoreApiKey();
        SetupHttpClient();
    }

    public async Task<IpQualityResponse> GetIPQuality(string ip, string user_agent)
    {
        var url = $"{_ipQualityUri}/{ip}?strictness=2&allow_public_access_points=true&lighter_penalties=true&user_agent={user_agent}";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<IpQualityResponse>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<EmailQualityResponse> GetEmailQualityScore(string email)
    {
        var url = $"{_emailQualityUri}/{email}";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<EmailQualityResponse>(body, _jsonSerializerSettings);
        result.FirstSeenISO = result.FirstSeen.ISO;
        result.DomainAgeISO = result.DomainAge.ISO;
        return result;
    }

    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
