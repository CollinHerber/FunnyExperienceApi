using Microsoft.Extensions.Configuration;
using Cowbot.Server.Configuration;
using Cowbot.Server.Data.Repositories.Interfaces;
using Cowbot.Server.Lib;
using Cowbot.Server.Lib.Exceptions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Cowbot.Server.ThirdPartyServices.DiscordApi.Interfaces;
using System.Text;
using Cowbot.Server.Models;
using Cowbot.Server.Models.DatabaseModels;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Message;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.Channel;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Cowbot.Server.ThirdPartyServices.DiscordApi;

public class DiscordMessageInterpreter : IDiscordMessageInterpreter
{
    private readonly IKeyValueCategoryRepository _keyValueCategoryRepository;
    private readonly IDiscordSupportTicketRepository _supportTicketRepository;
    private string _key = "";
    private string _ticketUserId = "";
    public DiscordMessageInterpreter(IKeyValueCategoryRepository keyValueCategoryRepository, 
        IDiscordSupportTicketRepository supportTicketRepository)
    {
        _keyValueCategoryRepository = keyValueCategoryRepository;
        _supportTicketRepository = supportTicketRepository;
    }
    private enum DiscordMessageFormatterTypes
    {
        Time,
        KeyValueCategory,
        KeyValue,
        TicketUser
    }
    public async Task<MessageData> FormatDiscordMessage(MessageData message, string channelId, Guid? discordServerId)
    {
        if (message.Content != null && (message.Content.Contains("${") || message.Content.Contains("${}")))
        {
            message.Content = message.Content.Replace("${}", "");
            await HandleTimeFormats(message);
            await HandleKeyValueGrab(message, discordServerId);
            await HandleTicketUserFormats(message, channelId);
            await HandleKeyCategoryGrab(message, discordServerId);
        }

        return message;
    }

    private async Task HandleTimeFormats(MessageData message)
    {
        const string currentTimePattern = @"\${time:(\d+)\}";
        var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        var currentTimeReplacement = (int)t.TotalSeconds;
        await RegexUpdateMessage(message, currentTimePattern, currentTimeReplacement.ToString(), DiscordMessageFormatterTypes.Time, null);
    }
    
    private async Task HandleTicketUserFormats(MessageData message, string channelId)
    {
        const string currentTimePattern = @"\${ticketUser}";
        if (_ticketUserId.IsNullOrEmpty())
        {
            try
            {
                var response = await _supportTicketRepository.GetByDiscordChannelId(channelId);
                if (response == null)
                {
                    return;
                }
                _ticketUserId = response.DiscordUserId;
            }
            catch (Exception e)
            {
                return;
            }
        }
        await RegexUpdateMessage(message, currentTimePattern, $"<@{_ticketUserId}>", DiscordMessageFormatterTypes.TicketUser, null);
    }
    
    private async Task HandleKeyValueGrab(MessageData message, Guid? discordServerId)
    {
        const string pattern = @"\$\{keyValue\s*\:\s*(\w+)\s*\:\s*(\w+)\}";
        await RegexUpdateMessage(message, pattern, "", DiscordMessageFormatterTypes.KeyValue, discordServerId);
    }
    
    private async Task HandleKeyCategoryGrab(MessageData message, Guid? discordServerId)
    {
        const string pattern = @"\$\{keyValueCategory\s*\:\s*(\w+)\s*\}";
        await RegexUpdateMessage(message, pattern, "", DiscordMessageFormatterTypes.KeyValueCategory, discordServerId);
    }

    private async Task RegexUpdateMessage(MessageData message, string pattern, string replacement, DiscordMessageFormatterTypes type, Guid? discordServerId)
    {
        var regex = new Regex(pattern, RegexOptions.Multiline);
        if (message.Content != null)
        {
            message.Content = await ReplaceRegexWithGroups(regex, message.Content, replacement, type, discordServerId);
        }

        if (message.Embeds != null)
        {
            foreach (var embed in message.Embeds)
            {
                embed.Description = await ReplaceRegexWithGroups(regex, embed.Description, replacement, type, discordServerId);
                embed.Title = await ReplaceRegexWithGroups(regex, embed.Title, replacement, type, discordServerId);
                if (embed.Footer != null)
                {
                    embed.Footer.Text = await ReplaceRegexWithGroups(regex, embed.Footer.Text, replacement, type, discordServerId);
                }

                if (embed.Fields != null && embed.Fields.Any())
                {
                    foreach (var field in embed.Fields)
                    {
                        field.Value = await ReplaceRegexWithGroups(regex, field.Value, replacement, type, discordServerId);
                        field.Name = await ReplaceRegexWithGroups(regex, field.Name, replacement, type, discordServerId);
                    }
                }
            }   
        }

        if (message.Components == null) return;
        foreach (var component in message.Components.SelectMany(componentRow => componentRow.Components))
        {
            if (component.Description != null)
            {
                component.Description = await ReplaceRegexWithGroups(regex, component.Description, replacement, type, discordServerId);   
            }

            if (component.Value != null)
            {
                component.Value = await ReplaceRegexWithGroups(regex, component.Value, replacement, type, discordServerId);
            }
                    
            if (component.Label != null)
            {
                component.Label = await ReplaceRegexWithGroups(regex, component.Label, replacement, type, discordServerId);
            }

            if (component.Options == null) continue;
            foreach (var option in component.Options)
            {
                option.Label = await ReplaceRegexWithGroups(regex, option.Label, replacement, type, discordServerId);
                option.Description = await ReplaceRegexWithGroups(regex, option.Description, replacement, type, discordServerId);
                option.Value = await ReplaceRegexWithGroups(regex, option.Value, replacement, type, discordServerId);
            }
        }
    }

    private async Task<string> ReplaceRegexWithGroups(Regex regex, string message, string replacement, DiscordMessageFormatterTypes type, Guid? discordServerId)
    {
        switch (type)
        {
            case DiscordMessageFormatterTypes.Time:
                if (!message.Contains("${time:")) return message;
                break;
            case DiscordMessageFormatterTypes.KeyValue:
                if (!message.Contains("${keyValue:")) return message;
                break;
            case DiscordMessageFormatterTypes.KeyValueCategory:
                if (!message.Contains("${keyValueCategory:")) return message;
                break;
            case DiscordMessageFormatterTypes.TicketUser:
                if (!message.Contains("${ticketUser}")) return message;
                break;
        }

        switch (type)
        {
            case DiscordMessageFormatterTypes.KeyValueCategory when _key.IsNullOrEmpty():
            {
                var match = regex.Match(message);
                if (match.Groups.Count > 1)
                {
                    var response = await _keyValueCategoryRepository.GetNextUndeliveredKeyForCategory(match.Groups[1].Value);
                    if (response != null)
                    {
                        _key = response;
                    }
                }

                break;
            }
            case DiscordMessageFormatterTypes.KeyValue when _key.IsNullOrEmpty():
            {
                var match = regex.Match(message);
                if (match.Groups.Count > 1)
                {
                    var response = await _keyValueCategoryRepository.GetKeyValueForCategoryName(match.Groups[1].Value, (Guid) discordServerId, match.Groups[2].Value);
                    if (response != null)
                    {
                        _key = response;
                    }
                }

                break;
            }
        }

        return regex.Replace(message, (match2) =>
        {
            var param = match2.Groups[1].Value;
            var newString = replacement;
            switch (type)
            {
                case DiscordMessageFormatterTypes.Time:
                    newString = (long.Parse(replacement) + long.Parse(param)).ToString();
                    break;
                case DiscordMessageFormatterTypes.KeyValueCategory:
                    newString = _key;
                    break;
            }
            return newString;
        });
    }
} 