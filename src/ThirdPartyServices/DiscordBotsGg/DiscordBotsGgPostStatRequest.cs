namespace Cowbot.Server.ThirdPartyServices.Topgg.Models;

public class DiscordBotsGgPostStatRequest
{
    public int GuildCount { get; set; }
    public int? ShardId { get; set; }
    public int? ShardCount { get; set; }
}