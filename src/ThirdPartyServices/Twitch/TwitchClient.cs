using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Cowbot.Server.Lib.Extensions;
using Cowbot.Server.ThirdPartyServices.Twitch.Interfaces;
using Cowbot.Server.ThirdPartyServices.Twitch.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Cowbot.Server.ThirdPartyServices.Twitch;

public class TwitchClient : ITwitchClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache _cache;
    private readonly string _baseUrl;
    private readonly string _twitchClientId;
    private readonly string _twitchSecret;
    private readonly string _baseApiUrl;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public TwitchClient(HttpClient httpClient, IConfiguration configuration, IDistributedCache cache)
    {
        _httpClient = httpClient;
        _baseUrl = "https://api.twitch.tv/";
        _configuration = configuration;
        _twitchClientId = _configuration.TwitchClientId();
        _twitchSecret = _configuration.TwitchSecret();
        _baseApiUrl = configuration.BaseApiUrl();
        _cache = cache;
        SetupHttpClient();
    }

    public async Task<TwitchCreateSubscriptionResponse> CreateChannelSubscription(string id, string type)
    {
        await SetAuthorizationHeader();
        var url = $"{_baseUrl}helix/eventsub/subscriptions";
        var request = new TwitchCreateSubscriptionRequest
        {
            Type = type,
            Version = "1",
            Condition = new TwitchCondition
            {
                BroadcasterUserId = id
            },
            Transport = new TwitchTransport
            {
                Callback = $"{_baseApiUrl}/Webhook/TwitchLiveNotification",
                Method = "webhook",
                Secret = "betsys3cret"
            }
        };
        var stringContent = CreateRequestObject(request);
        var response = await _httpClient.PostAsync(url, stringContent);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to create twitch subscription : " + body);
        }
        var result = JsonConvert.DeserializeObject<TwitchCreateSubscriptionResponse>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<dynamic> ListTwitchSubcriptions()
    {
        await SetAuthorizationHeader();
        var url = $"{_baseUrl}helix/eventsub/subscriptions";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<dynamic> DeleteChannelSubscription(Guid subscriptionId)
    {
        await SetAuthorizationHeader();
        var url = $"{_baseUrl}helix/eventsub/subscriptions?id={subscriptionId}";
        var response = await _httpClient.DeleteAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<TwitchGetTokenResponse> GetAppToken(dynamic? something = null)
    {
        var cachedResponse = await _cache.GetOrSetAsWithHourlyExpirationAsync<TwitchGetTokenResponse>(Constants.CacheKeys.GetTwitchAppKey, 1, async () =>
        {
            var url = $"https://id.twitch.tv/oauth2/token?client_id={_twitchClientId}&client_secret={_twitchSecret}&grant_type=client_credentials";
            var response = await _httpClient.PostAsync(url, CreateRequestObject(something));
            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TwitchGetTokenResponse>(body, _jsonSerializerSettings);
            return result;
        });
        return cachedResponse;
    }

    public async Task<TwitchGetUserResponse> GetTwitchUserFromLogin(string twitchLogin)
    {
        await SetAuthorizationHeader();
        var url = $"{_baseUrl}helix/users?login={twitchLogin}";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<TwitchGetUserResponse>(body, _jsonSerializerSettings);
        return result;
    }

    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task SetAuthorizationHeader()
    {
        var token = await GetAppToken();
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token.AccessToken);
    }

    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Client-Id", _twitchClientId);
    }
}