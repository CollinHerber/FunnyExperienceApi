using Cowbot.Server.Models.DatabaseModels;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordMessageInterpreter
{
    Task<MessageData> FormatDiscordMessage(MessageData message, string channelId, Guid? discordServerId);
}