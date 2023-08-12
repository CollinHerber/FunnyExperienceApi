using Cowbot.Server.ThirdPartyServices.Topgg.Models;

namespace Cowbot.Server.ThirdPartyServices.Topgg.Interfaces;

public interface ITopggClient
{
    Task PostBetsyBotStats(TopggPostStatRequest request);
}