namespace Cowbot.Server.ThirdPartyServices.Topgg.Models;

public class TopggPostStatRequest
{
    public int ServerCount { get; set; }
    public int[]? Shards { get; set; }
    public int? ShardId { get; set; }
    public int? ShardCount { get; set; }
}