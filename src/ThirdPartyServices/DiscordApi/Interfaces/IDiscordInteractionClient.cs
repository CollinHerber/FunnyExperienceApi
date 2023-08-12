using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInteraction;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordUser;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Guild;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Role;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordInteractionClient
{
    Task<dynamic> UpdateInteractionMessage(DiscordInteraction interaction, string interactionToken);
}