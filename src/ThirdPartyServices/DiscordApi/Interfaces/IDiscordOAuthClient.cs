using Cowbot.Server.Models.DTOs.DiscordApi.Models.OAuth;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordOAuthClient
{
    Task<AccessTokenResponse?> ExchangeCodeForToken(string code, string? redirectUrl);
    Task<AccessTokenResponse?> RefreshToken(string refreshToken);
}