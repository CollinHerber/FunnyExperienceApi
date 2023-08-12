using Microsoft.Extensions.Configuration;
using Cowbot.Server.Configuration;
using Cowbot.Server.Lib;
using Cowbot.Server.Lib.Exceptions;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.ApplicationCommands;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using System.Text;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordApplicationCommandsClient : IDiscordApplicationCommandsClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;
    private readonly IDiscordChannelClient _discordChannelClient;

    public DiscordApplicationCommandsClient(HttpClient httpClient, IDiscordChannelClient discordChannelClient)
    {
        _httpClient = httpClient;
        _discordChannelClient = discordChannelClient;
    }

    public async Task<List<DiscordApplicationCommand>> GetGuildApplicationCommands(string guildId)
    {
        var url = "guilds/" + guildId + "/commands";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "GetGuildApplicationCommands");
        }
        var result = JsonConvert.DeserializeObject<List<DiscordApplicationCommand>>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<DiscordApplicationCommand?> CreateGuildApplicationCommand(DiscordApplicationCommand command, string guildId)
    {
        var url =  "guilds/" + guildId + "/commands";
        var requestObject = CreateRequestObject(command);
        var response = await _httpClient.PostAsync(url, requestObject);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "CreateGuildApplicationCommand");
            throw new DiscordApiException("Failed to Create Command", url);
        }
        var result = JsonConvert.DeserializeObject<DiscordApplicationCommand>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<DiscordApplicationCommand?> UpdateGuildApplicationCommand(DiscordApplicationCommand command, string guildId)
    {
        var url = "guilds/" + guildId + "/commands";
        var response = await _httpClient.PatchAsync(url, CreateRequestObject(command));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "UpdateGuildApplicationCommand");
            throw new DiscordApiException("Failed to Create Command", url);
        }
        var result = JsonConvert.DeserializeObject<DiscordApplicationCommand>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task DeleteGuildApplicationCommand(string commandId, string guildId)
    {
        var url = "guilds/" + guildId + "/commands/" + commandId;
        var response = await _httpClient.DeleteAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(body, _jsonSerializerSettings);
    }

    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}