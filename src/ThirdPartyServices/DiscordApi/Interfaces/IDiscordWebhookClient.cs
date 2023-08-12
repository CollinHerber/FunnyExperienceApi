using Cowbot.Server.Models;
using Cowbot.Server.Models.DatabaseModels;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInteraction;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Message;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordWebhookClient
{
    Task<dynamic> UpdateInteractionMessage(DiscordInteractionResponse interaction, string interactionToken);
}