using Microsoft.Extensions.Configuration;
using Cowbot.Server.Lib;
using Newtonsoft.Json;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using System.Text;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInteraction;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordInteractionClient : IDiscordInteractionClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public DiscordInteractionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<dynamic> UpdateInteractionMessage(DiscordInteraction interaction, string interactionToken)
    {
        var response = await _httpClient.PostAsync($"{interaction}/", CreateRequestObject(interaction));
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(body, _jsonSerializerSettings);
        return result;
    }

    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}