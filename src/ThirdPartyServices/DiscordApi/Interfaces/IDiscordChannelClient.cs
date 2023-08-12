using Cowbot.Server.Models;
using Cowbot.Server.Models.DatabaseModels;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Message;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;

public interface IDiscordChannelClient
{
    Task<DiscordChannel?> GetDiscordChannel(string channelId);
    Task<List<DiscordApiMessage>?> GetChannelMessages(string channelId, string? after, int limit);
    Task<DiscordApiMessage?> SendMessageToChannel(MessageData message, string channelId, DiscordServer server);
    Task<DiscordApiMessage?> SendMessageToChannel(MessageData message, string channelId, List<DiscordEmbedField>? additionalFields = null);
    Task<dynamic?> DeleteMessageInChannel(string messageId, string channelId);
    Task<DiscordApiMessage?> EditMessageInChannel(DiscordMessage message, DiscordServer server);
    Task<DiscordApiMessage?> EditMessageInChannel(MessageData message, string discordMessageId, string discordChannelId);
    Task<List<DiscordApiMessage>> GetAllChannelMessages(string channelId, int hundreds = 20);
    Task<List<DiscordApiMessage>> GetAllChannelMessagesFromPast14Days(string channelId, int executionMax);
    Task<dynamic?> DeleteChannel(string channelId);
    Task<DiscordChannel?> UpdateChannel(DiscordChannel channel, string channelId);
    Task<DiscordApiMessage?> SendFileMessageToChannel(MessageData message, string filename, byte[] bytes, string channelId, string? botOverride = "");
    Task<DiscordApiMessage?> GetMostRecentMessage(string channelId);
    Task DeleteBulkMessagesFromChannel(string channelId, List<string> messageIds);
    Task<DiscordApiMessage?> StartThreadForMessage(string messageId, string channelId, string name);
    Task<DiscordApiMessage?> SendWebhookExceptionDebugMessageToBetsyChannel(MessageData message);
    Task SendDiscordApiFailureMessageToBetsyChannel(string body, string function);
}