using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordUser;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Guild;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordUserClient
{
    Task<List<DiscordGuild?>> GetUsersGuilds(string disordAccessToken);
    Task<DiscordUser?> GetCurrentUser(string discordAccessToken);
    Task<DiscordChannel> CreateDmChannel(CreateDmChannelRequest request, string botOverride = "");
}