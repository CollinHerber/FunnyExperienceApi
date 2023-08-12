namespace Cowbot.Server.ThirdPartyServices.Twitch.Models;

public class TwitchCreateSubscriptionResponse
{
    public List<TwitchApiEventSubscription> Data { get; set; }
}

public class TwitchApiEventSubscription
{
    public Guid Id { get; set; }
    public string Status { get; set; }
}