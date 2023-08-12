namespace Cowbot.Server.ThirdPartyServices.Twitch.Models;

public class TwitchCreateSubscriptionRequest
{
    public string Type { get; set; }
    public string Version { get; set; }
    public TwitchCondition Condition { get; set; }
    public TwitchTransport Transport { get; set; }
}

public class TwitchCondition
{
    public string BroadcasterUserId { get; set; }
}

public class TwitchTransport
{
    public string Method { get; set; }
    public string Callback { get; set; }
    public string Secret { get; set; }
}