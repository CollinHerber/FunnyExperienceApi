using Cowbot.Server.ThirdPartyServices.Topgg.Models;

namespace Cowbot.Server.ThirdPartyServices.Topgg.Interfaces;

public interface IDiscordBotsGgClient
{
    Task PostBetsyBotStats(DiscordBotsGgPostStatRequest request);
}