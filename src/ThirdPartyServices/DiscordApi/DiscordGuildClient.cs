using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using Newtonsoft.Json;
using Cowbot.Server.Lib;
using Cowbot.Server.Lib.Extensions;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInvite;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Role;
using System.Text;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Guild;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordUser;
using Microsoft.Extensions.Caching.Distributed;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordGuildClient : IDiscordGuildClient
{
    private readonly HttpClient _httpClient;
    private readonly IDiscordChannelClient _discordChannelClient;
    private readonly IDistributedCache _cache;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public DiscordGuildClient(HttpClient httpClient, IDiscordChannelClient discordChannelClient, IDistributedCache cache)
    {
        _httpClient = httpClient;
        _discordChannelClient = discordChannelClient;
        _cache = cache;
    }

    public async Task<DiscordGuild?> GetGuild(string guildId)
    {
        var url = guildId + "?with_counts=true";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "GetGuild");
        }
        var result = JsonConvert.DeserializeObject<DiscordGuild>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<List<DiscordChannel>?> GetGuildChannels(string guildId)
    {
        return await _cache.GetOrSetNullableAsWithHourlyExpirationAsync(GetGuildChannelsString(guildId), 3, async () => {
            var url = guildId + "/channels";
            var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "GetGuildChannels");
                return null;
            }

            var result = JsonConvert.DeserializeObject<List<DiscordChannel>>(body, _jsonSerializerSettings);
            return result != null ? result.OrderBy(x => x.Name).ToList() : result;
        });
    }

    public async Task<List<DiscordGuildRole>?> GetGuildRoles(string guildId)
    {
        return await _cache.GetOrSetNullableAsWithHourlyExpirationAsync(GetGuildRolesString(guildId), 3, async () => {
            var url = guildId + "/roles";
            var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "GetGuildRoles");
                return null;
            }

            var result = JsonConvert.DeserializeObject<List<DiscordGuildRole>>(body, _jsonSerializerSettings);
            return result;
        });
    }

    public async Task<DiscordGuildMember?> GetGuildMember(string guildId, string userId)
    {
        var url = guildId + "/members/" + userId;
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "GetGuildMember");
        }
        var result = JsonConvert.DeserializeObject<DiscordGuildMember>(body, _jsonSerializerSettings);
        return result;
    }
    
    public async Task<List<DiscordGuildMember>?> SearchGuildMembers(string guildId, string searchQuery)
    {
        var url = $"{guildId}/members/search?query={searchQuery}";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "SearchGuildMembers");
        }
        var result = JsonConvert.DeserializeObject<List<DiscordGuildMember>>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<DiscordChannel?> CreateGuildChannel(string guildId, DiscordChannel channel)
    {
        var url = guildId + "/channels";
        var response = await _httpClient.PostAsync(url, CreateRequestObject(channel));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "CreateGuildChannel");
        }
        var result = JsonConvert.DeserializeObject<DiscordChannel>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<dynamic?> ModifyGuildMember(string guildId, string userId, ModifyDiscordUserRequest request)
    {
        var url = guildId + "/members/" + userId;
        var response = await _httpClient.PatchAsync(url, CreateRequestObject(request));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "ModifyGuildMember");
        }
        var result = JsonConvert.DeserializeObject<dynamic>(body, _jsonSerializerSettings);
        return result;
    }

    
    public async Task<bool> AddRoleToGuildMember(string guildId, string userId, string roleId)
    {
        var url = guildId + "/members/" + userId + "/roles/" + roleId;
        var response = await _httpClient.PutAsync(url, CreateRequestObject(userId));
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(await response.Content.ReadAsStringAsync(), "AddRoleToGuildMember");
        }
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> AddRoleToGuildMember(string guildId, DiscordGuildMember member, string roleId)
    {
        var url = guildId + "/members/" + member.User.Id + "/roles/" + roleId;
        var response = await _httpClient.PutAsync(url, CreateRequestObject(member));
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(await response.Content.ReadAsStringAsync(), "AddRoleToGuildMember");
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveRoleFromGuildMember(string guildId, DiscordGuildMember member, string roleId)
    {
        var url = guildId + "/members/" + member.User.Id + "/roles/" + roleId;
        var response = await _httpClient.DeleteAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(await response.Content.ReadAsStringAsync(), "RemoveRoleFromGuildMember");
        }
        return response.IsSuccessStatusCode;
    }
    
    public async Task<List<DiscordApiInvite>?> GetGuildInvites(string guildId)
    {
        var url = guildId + "/invites";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await _discordChannelClient.SendDiscordApiFailureMessageToBetsyChannel(body, "GetGuildInvites");
        }
        var result = JsonConvert.DeserializeObject<List<DiscordApiInvite>>(body, _jsonSerializerSettings);
        return result;
    }
    
    private static string GetGuildChannelsString(string guildId)
    {
        return string.Format(Constants.CacheKeys.DiscordGuildChannelsKey, guildId);
    }
    
    private static string GetGuildRolesString(string guildId)
    {
        return string.Format(Constants.CacheKeys.DiscordGuildRolesKey, guildId);
    }
    
    public async Task ClearCacheForGuildChannels(string guildId)
    {
        await _cache.RemoveAsync(GetGuildChannelsString(guildId));
    }
    
    public async Task ClearCacheForGuildRoles(string guildId)
    {
        await _cache.RemoveAsync(GetGuildRolesString(guildId));
    }

    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}