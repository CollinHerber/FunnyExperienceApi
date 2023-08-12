using Microsoft.Extensions.Configuration;
using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using System.Text;
using Cowbot.Server.Models;
using Cowbot.Server.Models.DatabaseModels;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInteraction;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Message;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordWebhookClient : IDiscordWebhookClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public DiscordWebhookClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
    }
    
    public async Task<dynamic> UpdateInteractionMessage(DiscordInteractionResponse interaction, string interactionToken)
    {
        var response = await _httpClient.PostAsync($"{interactionToken}", CreateRequestObject(interaction.Data));
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