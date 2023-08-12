using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FunnyExperience.Server.Lib;

public static class Constants
{
    public static class CacheKeys
    {
        public const string GetTwitchAppKey = "TwitchAppKey";
        public const string GuildKey = "Guild-{0}";
        public const string SupportTicketSettingsKey = "SupportTicketSettings-{0}";
        public const string SupportTicketKey = "SupportTicket-{0}";
        public const string DiscordUser = "DiscordUser-{0}";
        public const string AutoRespondersForGuildKey = "AutoRespondersGuild-{0}";
        public const string DiscordGuildChannelsKey = "DiscordGuildChannels-{0}";
        public const string DiscordGuildRolesKey = "DiscordGuildRoles-{0}";
    }

    public static class Json
    {
        public static JsonSerializerSettings SnakeCaseSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };



        public static JsonSerializerSettings CamelCaseSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
    }

    public static class TwitchKeys
    {
        public const string StreamOnline = "stream.online";
        public const string StreamOffline = "stream.offline";
        public const string ChannelFollow = "channel.follow";
        public const string ChannelUpdate = "channel.update";
        public const string ChannelSubscribe = "channel.subscribe";
        public const string ChannelSubscriptionEnd = "channel.subscription.end";
        public const string ChannelSubscriptionGift = "channel.subscription.gift";
        public const string ChannelSubscriptionMessage = "channel.subscription.message";
        public const string ChannelHypetrainBegin = "channel.hype_train.begin";
        public const string ChannelHypetrainEnd = "channel.hype_train.end";
        public const string ChannelPollBegin = "channel.poll.begin";
        public const string ChannelPollProgress = "channel.poll.progress";
        public const string ChannelPollEnd = "channel.poll.end";
        public const string ChannelPredictionBegin = "channel.prediction.begin";
        public const string ChannelPredictionProgress = "channel.prediction.progress";
        public const string ChannelPredictionLock = "channel.prediction.lock";
        public const string ChannelPredictionEnd = "channel.prediction.end";

        public static bool ContainsKey(string key)
        {
            switch (key)
            {
                case StreamOnline:
                case StreamOffline:
                case ChannelFollow: 
                case ChannelUpdate:
                case ChannelSubscribe:
                case ChannelSubscriptionEnd:
                case ChannelSubscriptionGift:
                case ChannelSubscriptionMessage:
                case ChannelHypetrainBegin:
                case ChannelHypetrainEnd:
                case ChannelPollBegin:
                case ChannelPollEnd:
                case ChannelPollProgress:
                case ChannelPredictionBegin:
                case ChannelPredictionProgress:
                case ChannelPredictionLock:
                case ChannelPredictionEnd:
                    return true;
                default:
                    return false;
            }
        }
    }
}