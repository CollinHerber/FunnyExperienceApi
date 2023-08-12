using Cowbot.Server.Configuration;
using Microsoft.Extensions.Configuration;
using Cowbot.Server.Lib;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using System.Text;
using Cowbot.Server.Models.DatabaseModels;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Message;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;
using Microsoft.IdentityModel.Tokens;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordChannelClient : IDiscordChannelClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;
    private readonly IDiscordMessageInterpreter _discordMessageInterpreter;
    private readonly IConfiguration _configuration;

    public DiscordChannelClient(HttpClient httpClient, IConfiguration configuration,
        IDiscordMessageInterpreter discordMessageInterpreter)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _discordMessageInterpreter = discordMessageInterpreter;
    }

    public async Task<DiscordChannel?> GetDiscordChannel(string channelId)
    {
        var url = channelId;
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "GetDiscordChannel");
        }

        var result = JsonConvert.DeserializeObject<DiscordChannel>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<DiscordApiMessage?> GetMostRecentMessage(string channelId)
    {
        var url = channelId + $"/messages?limit={1}";
        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "GetMostRecentMessage");
        }

        var result = JsonConvert.DeserializeObject<List<DiscordApiMessage>>(body, _jsonSerializerSettings);
        return result?.First();
    }

    public async Task<List<DiscordApiMessage>?> GetChannelMessages(string channelId, string? after, int limit)
    {
        var url = channelId + $"/messages?limit={limit}";
        if (!string.IsNullOrWhiteSpace(after))
        {
            url += $"&after={after}";
        }

        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "GetChannelMessages");
            return null;
        }

        var result = JsonConvert.DeserializeObject<List<DiscordApiMessage>>(body, _jsonSerializerSettings);
        return result;
    }

    private async Task<List<DiscordApiMessage>?> GetChannelMessagesWithBefore(string channelId, string? before,
        int limit)
    {
        var url = channelId + $"/messages?limit={limit}";
        if (!string.IsNullOrWhiteSpace(before))
        {
            url += $"&before={before}";
        }

        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "GetChannelMessagesWithBefore");
        }

        var result = JsonConvert.DeserializeObject<List<DiscordApiMessage>>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<List<DiscordApiMessage>> GetAllChannelMessages(string channelId, int hundreds = 20)
    {
        var allMessages = new List<DiscordApiMessage>();
        var channelMessages = await GetChannelMessages(channelId, null, 100);
        if (channelMessages == null)
        {
            return allMessages;
        }

        allMessages.AddRange(channelMessages ?? throw new InvalidOperationException());
        if (allMessages.Count != 100) return allMessages;
        var fetchAmount = 100;
        var executionAmounts = 1;
        while (fetchAmount == 100 && executionAmounts < hundreds)
        {
            var messages = await GetChannelMessages(channelId, allMessages[^1].Id, 100);
            fetchAmount = messages.Count;
            allMessages.AddRange(messages);
            executionAmounts++;
        }

        return allMessages;
    }

    public async Task<List<DiscordApiMessage>> GetAllChannelMessagesFromPast14Days(string channelId, int executionMax)
    {
        var allMessages = new List<DiscordApiMessage>();
        var mostRecent = await GetMostRecentMessage(channelId);
        if (mostRecent == null)
        {
            return allMessages;
        }

        allMessages.Add(mostRecent);
        var fetchAmount = 100;
        var executionAmounts = 1;
        var fourteenDaysAgo = DateTime.Now.AddDays(-14);
        var olderThanFourteenDays = false;
        while (fetchAmount == 100 && executionAmounts < executionMax && !olderThanFourteenDays)
        {
            var messages = await GetChannelMessagesWithBefore(channelId, allMessages[^1].Id, 100);
            if (messages != null && messages.Any() && messages[0].Timestamp < fourteenDaysAgo)
            {
                olderThanFourteenDays = true;
            }
            else
            {
                if (messages != null)
                {
                    fetchAmount = messages.Count;
                    allMessages.AddRange(messages);
                }

                executionAmounts++;
            }
        }

        return allMessages.Where(x => x.Timestamp > fourteenDaysAgo).ToList();
    }

    public async Task<DiscordApiMessage?> SendWebhookExceptionDebugMessageToBetsyChannel(MessageData message)
    {
        return await SendMessageToChannel(message, _configuration.WebhookExceptionChannel());
    }

    public async Task SendDiscordApiFailureMessageToBetsyChannel(string body, string function)
    {
        await SendMessageToChannel(GetErrorMessageData(body, function), _configuration.DiscordApiExceptionChannel());
    }
    
    public async Task<DiscordApiMessage?> SendMessageToChannel(MessageData message, string channelId, DiscordServer server)
    {
        if (channelId == null)
        {
            throw new ArgumentNullException(nameof(channelId));
        }

        if (server.CustomBotActive == true && !server.CustomBotJwtKey.IsNullOrEmpty())
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bot {server.CustomBotJwtKey}");
        }
        
        message = await _discordMessageInterpreter.FormatDiscordMessage(message, channelId, server.Id);

        return await SendMessageToChannel(message, channelId, new List<DiscordEmbedField> {
            new () {
                Name = "Server Guild ID",
                Value = server.GuildId,
                Inline = true
            },
            new () {
                Name = "Server Custom Bot Active",
                Value = server.CustomBotActive == false ? "False" : "True",
                Inline = true
            }
        });
    }

    public async Task<DiscordApiMessage?> SendMessageToChannel(MessageData message, string channelId, List<DiscordEmbedField>? additionalFields = null)
    {
        if (channelId == null)
        {
            throw new ArgumentNullException(nameof(channelId));
        }

        var url = channelId + "/messages";
        var response = await _httpClient.PostAsync(url, CreateRequestObject(message));
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<DiscordApiMessage>(body, _jsonSerializerSettings);
        if (response.IsSuccessStatusCode) return result;
        
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bot {_configuration.BetsyBotToken()}");
        var errorMessage = GetErrorMessageData(body, "SendMessage", additionalFields);
        await _httpClient.PostAsync($"{_configuration.DiscordApiExceptionChannel()}/messages", CreateRequestObject(errorMessage));
        return result;
    }

    public async Task<DiscordApiMessage?> SendFileMessageToChannel(MessageData message, string fileName, byte[] bytes,
        string channelId, string? botOverride = "")
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
        var multiForm = new MultipartFormDataContent();

        if (!botOverride.IsNullOrEmpty())
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bot {botOverride}");
        }

        // add API method parameters
        multiForm.Add(CreateRequestObject(message), "payload_json");
        multiForm.Add(new ByteArrayContent(bytes), "files[0]", fileName);

        var url = channelId + "/messages";
        var response = await _httpClient.PostAsync(url, multiForm);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<DiscordApiMessage>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<DiscordApiMessage?> EditMessageInChannel(DiscordMessage message, DiscordServer server)
    {
        var url = message.DiscordChannelId + "/messages/" + message.DiscordMessageId;
        var response = await _httpClient.PatchAsync(url, CreateRequestObject(message.Message));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "EditMessageInChannel");
        }

        var result = JsonConvert.DeserializeObject<DiscordApiMessage>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<DiscordApiMessage?> EditMessageInChannel(MessageData message, string discordMessageId,
        string discordChannelId)
    {
        var url = discordChannelId + "/messages/" + discordMessageId;
        var response = await _httpClient.PatchAsync(url, CreateRequestObject(message));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "EditMessageInChannel");
        }

        var result = JsonConvert.DeserializeObject<DiscordApiMessage>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<dynamic?> DeleteMessageInChannel(string messageId, string channelId)
    {
        var url = channelId + "/messages/" + messageId;
        var response = await _httpClient.DeleteAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "DeleteMessageInChannel");
        }

        var result = JsonConvert.DeserializeObject<dynamic>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<dynamic?> DeleteChannel(string channelId)
    {
        var response = await _httpClient.DeleteAsync(channelId);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "DeleteChannel");
        }

        var result = JsonConvert.DeserializeObject<dynamic>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<DiscordChannel?> UpdateChannel(DiscordChannel channel, string channelId)
    {
        var response = await _httpClient.PatchAsync(channelId, CreateRequestObject(channel));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "UpdateChannel");
        }

        var result = JsonConvert.DeserializeObject<DiscordChannel>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task DeleteBulkMessagesFromChannel(string channelId, List<string> messageIds)
    {
        var url = channelId + "/messages/bulk-delete";
        var messageIdsObject = new BulkDeleteMessages { Messages = messageIds };
        var response = await _httpClient.PostAsync(url, CreateRequestObject(messageIdsObject));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "DeleteBulkMessagesFromChannel");
        }
    }

    public async Task<DiscordApiMessage?> StartThreadForMessage(string messageId, string channelId, string name)
    {
        if (channelId == null)
        {
            throw new ArgumentNullException(nameof(channelId));
        }

        if (messageId == null)
        {
            throw new ArgumentNullException(nameof(messageId));
        }

        var url = $"{channelId}/messages/{messageId}/threads";
        var response = await _httpClient
            .PostAsync(url, CreateRequestObject(new CreateDiscordThreadRequest { Name = name }));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            await SendDiscordApiFailureMessageToBetsyChannel(body, "StartThreadForMessage");
        }

        var result = JsonConvert.DeserializeObject<DiscordApiMessage>(body, _jsonSerializerSettings);
        return result;
    }

    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private MessageData GetErrorMessageData(string body, string function, List<DiscordEmbedField>? additionalFields = null)
    {
        var message = new MessageData {
            Embeds = new List<DiscordEmbed> {
                new() {
                    Title = "Discord API Failure",
                    Description = body,
                    Fields = new List<DiscordEmbedField> { new() { Name = "Function", Value = function } }
                }
            }
        };
        if (additionalFields != null)
        {
            message.Embeds[0].Fields.AddRange(additionalFields);
        }

        return message;
    }
}

public class CreateDiscordThreadRequest
{
    public string? Name { get; set; }
}