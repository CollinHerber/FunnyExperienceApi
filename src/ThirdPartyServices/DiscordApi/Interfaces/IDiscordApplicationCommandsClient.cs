using Cowbot.Server.Models.DTOs.DiscordApi.Models.ApplicationCommands;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordApplicationCommandsClient
{
    Task<List<DiscordApplicationCommand>> GetGuildApplicationCommands(string guildId);
    Task<DiscordApplicationCommand?> CreateGuildApplicationCommand(DiscordApplicationCommand command, string guildId);
    Task<DiscordApplicationCommand?> UpdateGuildApplicationCommand(DiscordApplicationCommand command, string guildId);
    Task DeleteGuildApplicationCommand(string commandId, string guildId);
}