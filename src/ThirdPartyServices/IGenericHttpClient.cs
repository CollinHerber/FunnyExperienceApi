using Cowbot.Server.Models.DatabaseModels;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInteraction;

namespace Cowbot.Server.ThirdPartyServices;

public interface IGenericHttpClient
{
    Task<DiscordRestDataCommandResponse> SendDeleteRequest(string url, DiscordCommand command,
        DiscordInteraction interaction, DiscordCommandAction action);

    Task<DiscordRestDataCommandResponse> SendGetRequest(string url, DiscordCommand command,
        DiscordInteraction interaction, DiscordCommandAction action);
    Task<DiscordRestDataCommandResponse> SendPostRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action);
    Task<DiscordRestDataCommandResponse> SendPutRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action);
    Task<DiscordRestDataCommandResponse> SendPatchRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action);
}