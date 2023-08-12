using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInvite;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordUser;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Guild;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Role;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordGuildClient
{
    Task<DiscordGuild?> GetGuild(string guildId);
    Task<List<DiscordChannel>?> GetGuildChannels(string guildId);
    Task<DiscordChannel?> CreateGuildChannel(string guildId, DiscordChannel channel);
    Task<List<DiscordGuildRole>?> GetGuildRoles(string guildId);
    Task<DiscordGuildMember?> GetGuildMember(string guildId, string userId);
    Task<dynamic?> ModifyGuildMember(string guildId, string userId, ModifyDiscordUserRequest request);
    Task<bool> AddRoleToGuildMember(string guildId, DiscordGuildMember member, string roleId);
    Task<bool> RemoveRoleFromGuildMember(string guildId, DiscordGuildMember member, string roleId);
    Task ClearCacheForGuildChannels(string guildId);
    Task ClearCacheForGuildRoles(string guildId);
    Task<List<DiscordGuildMember>?> SearchGuildMembers(string guildId, string searchQuery);
    Task<List<DiscordApiInvite>?> GetGuildInvites(string guildId);
}