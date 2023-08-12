using Cowbot.Server.ThirdPartyServices.Twitch.Models;

namespace Cowbot.Server.ThirdPartyServices.Twitch.Interfaces;

public interface ITwitchClient
{
    Task<TwitchCreateSubscriptionResponse> CreateChannelSubscription(string id, string type);
    Task<dynamic> ListTwitchSubcriptions();
    Task<dynamic> DeleteChannelSubscription(Guid subscriptionId);
    Task<TwitchGetUserResponse> GetTwitchUserFromLogin(string twitchLogin);
}