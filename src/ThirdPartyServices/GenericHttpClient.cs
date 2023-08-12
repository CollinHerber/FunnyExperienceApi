using Cowbot.Server.Lib;
using Cowbot.Server.Models.DatabaseModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Cowbot.Server.Models.DTOs.DiscordApi.Models.DiscordInteraction;
using Cowbot.Server.Models.DTOs.Shared;
using System.Diagnostics;

namespace Cowbot.Server.ThirdPartyServices;

public class GenericHttpClient : IGenericHttpClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public GenericHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<DiscordRestDataCommandResponse> SendDeleteRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action)
    {
        var convertedUrl = MapUrlWithParams(url, command, interaction);
        var urlWithParams = BuildUrlWithQueryParams(convertedUrl, command, interaction);
        var response = await _httpClient.DeleteAsync(urlWithParams);
        var body = await response.Content.ReadAsStringAsync();
        var serializedBody = new object();
        try
        {
            serializedBody = JsonConvert.DeserializeObject(body);
        } catch{
            //throw new Exception("Failed to serialize response");
        }
        var commandResponse = new DiscordRestDataCommandResponse
        {
            RequestUrl = urlWithParams
        };
        if (action.RestRequestMetadata.RequestResponseMappings != null)
        {
            commandResponse.ResponseJson = BuildResponseObject(serializedBody, action.RestRequestMetadata.RequestResponseMappings);
        }
        else
        {
            commandResponse.ResponseJson = JsonConvert.SerializeObject(serializedBody, Formatting.Indented);
        }
        return commandResponse;
    }
    
    public async Task<DiscordRestDataCommandResponse> SendGetRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action)
    {
        var convertedUrl = MapUrlWithParams(url, command, interaction);
        var urlWithParams = BuildUrlWithQueryParams(convertedUrl, command, interaction);
        var response = await _httpClient.GetAsync(urlWithParams);
        var body = await response.Content.ReadAsStringAsync();
        var serializedBody = new object();
        try
        {
            serializedBody = JsonConvert.DeserializeObject(body);
        } catch{
            //throw new Exception("Failed to serialize response");
        }
        var commandResponse = new DiscordRestDataCommandResponse
        {
            RequestUrl = urlWithParams
        };
        if (action.RestRequestMetadata.RequestResponseMappings != null)
        {
            commandResponse.ResponseJson = BuildResponseObject(serializedBody, action.RestRequestMetadata.RequestResponseMappings);
        } else
        {
            commandResponse.ResponseJson = JsonConvert.SerializeObject(serializedBody, Formatting.Indented);
        }
        return commandResponse;
    }
    
    public async Task<DiscordRestDataCommandResponse> SendPostRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action)
    {
        var expando = CreateRequestObject(command, interaction);
        var json = JsonConvert.SerializeObject(expando, _jsonSerializerSettings);
        var requestObject = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, requestObject);
        var body = await response.Content.ReadAsStringAsync();
        var serializedBody = new object();
        try
        {
            serializedBody = JsonConvert.DeserializeObject(body);
        } catch{}
        var commandResponse = new DiscordRestDataCommandResponse
        {
            RequestJson = JsonConvert.SerializeObject(expando, Formatting.Indented),
            RequestUrl = url
        };
        if (action.RestRequestMetadata.RequestResponseMappings != null)
        {
            commandResponse.ResponseJson = BuildResponseObject(serializedBody, action.RestRequestMetadata.RequestResponseMappings);
        }
        else
        {
            commandResponse.ResponseJson = JsonConvert.SerializeObject(serializedBody, Formatting.Indented);
        }
        return commandResponse;
    }

    public async Task<DiscordRestDataCommandResponse> SendPutRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action)
    {
        var expando = CreateRequestObject(command, interaction);
        var json = JsonConvert.SerializeObject(expando, _jsonSerializerSettings);
        var requestObject = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, requestObject);
        var body = await response.Content.ReadAsStringAsync();
        var serializedBody = new object();
        try
        {
            serializedBody = JsonConvert.DeserializeObject(body);
        }
        catch { }
        var commandResponse = new DiscordRestDataCommandResponse
        {
            RequestJson = JsonConvert.SerializeObject(expando, Formatting.Indented),
            RequestUrl = url
        };
        if (action.RestRequestMetadata.RequestResponseMappings != null)
        {
            commandResponse.ResponseJson = BuildResponseObject(serializedBody, action.RestRequestMetadata.RequestResponseMappings);
        }
        else
        {
            commandResponse.ResponseJson = JsonConvert.SerializeObject(serializedBody, Formatting.Indented);
        }
        return commandResponse;
    }

    public async Task<DiscordRestDataCommandResponse> SendPatchRequest(string url, DiscordCommand command, DiscordInteraction interaction, DiscordCommandAction action)
    {
        var expando = CreateRequestObject(command, interaction);
        var json = JsonConvert.SerializeObject(expando, _jsonSerializerSettings);
        var requestObject = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(url, requestObject);
        var body = await response.Content.ReadAsStringAsync();
        var serializedBody = new object();
        try
        {
            serializedBody = JsonConvert.DeserializeObject(body);
        }
        catch { }
        var commandResponse = new DiscordRestDataCommandResponse
        {
            RequestJson = JsonConvert.SerializeObject(expando, Formatting.Indented),
        };
        if (action.RestRequestMetadata.RequestResponseMappings != null)
        {
            commandResponse.ResponseJson = BuildResponseObject(serializedBody, action.RestRequestMetadata.RequestResponseMappings);
        }
        else
        {
            commandResponse.ResponseJson = JsonConvert.SerializeObject(serializedBody, Formatting.Indented);
        }
        return commandResponse;
    }

    private void SetHeadersFromActions(DiscordCommand command)
    {
        
        foreach (var action in command.DiscordCommandActions)
        {
            if (action.RestRequestMetadata.RequestHeaders != null)
            {
                foreach (var header in action.RestRequestMetadata.RequestHeaders)
                {
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Name, header.Value);    
                }
            }
        }
    }

    private string BuildUrlWithQueryParams(string url, DiscordCommand command, DiscordInteraction interaction)
    {
        var editedUrl = url + "?";
        var commandParamCount = 0;
        var defaultParamCount = 0;
        if (command.CommandInformation?.Options != null)
        {
            foreach (var property in command.CommandInformation.Options)
            {
                commandParamCount++;
                var parameter = interaction.Data.Options.Where(x => x.Name == property.Name).First();
                editedUrl += $"{property.Name}={GetValue(parameter)}";
                if (command.CommandInformation.Options.Count > commandParamCount)
                {
                    editedUrl += "&";
                }
            }
        }

        if (command.CommandInformation?.Options != null &&
            command.DiscordCommandActions[0]?.RestRequestMetadata?.DefaultRequestData != null)
        {
            editedUrl += "&";
        }

        if (command.DiscordCommandActions[0]?.RestRequestMetadata?.DefaultRequestData != null)
        {
            foreach (var property in command.DiscordCommandActions[0]?.RestRequestMetadata?.DefaultRequestData)
            {
                editedUrl += $"{property.Name}={property.Value}";
                defaultParamCount++;
                if (command.DiscordCommandActions[0]?.RestRequestMetadata?.DefaultRequestData.Count > defaultParamCount)
                {
                    editedUrl += "&";
                }
            }
        }

        return editedUrl;
    }

    private Dictionary<string, object> CreateRequestObject(DiscordCommand command, DiscordInteraction interaction)
    {
        var mappings = new List<string>();
        var expando = new Dictionary<string, object>();
        if (command.CommandInformation?.Options != null)
        {
            foreach (var property in command.CommandInformation.Options)
            {
                if (property.ApiPath != null)
                {
                    mappings.Add(property.ApiPath);
                }
            }
            mappings.ToList().ForEach(y =>
            {
                var property = command.CommandInformation.Options.Where(x => x.ApiPath == y).First();
                var parameter = interaction.Data.Options.Where(x => x.Name == property.Name).First();
                var parts = y.Split(".");
                var index = 0;
                var current = expando;
                parts.ToList().ForEach(y =>
                {

                    var keyExists = current.ContainsKey(y);

                    if (parts.Length - 1 == index++)
                    {
                        if (keyExists)
                        {
                            current[y] = GetValue(parameter);
                        }
                        else
                        {
                            current.TryAdd(y, GetValue(parameter));
                        }
                    }
                    else
                    {
                        if (keyExists)
                        {
                            current = expando[y] as Dictionary<string, object>;
                        }
                        else
                            current = new Dictionary<string, object>();

                        if (current != null) expando.TryAdd(y, current);
                    }
                });
            });
        }
        SetHeadersFromActions(command);
        AddDefaultDataToExpando(expando, command, interaction);
        return expando;
    }

    private string BuildResponseObject(dynamic serializedBody, List<RestRequestResponseMapping> mappings)
    {
        if (serializedBody == null)
        {
            return "No Response Body";
        }

        if (mappings == null || mappings.Count == 0)
        {
            return serializedBody.ToString();
        }

        var dictionary = new Dictionary<string, object>();
        foreach (var mapping in mappings)
        {
            var parts = mapping.ApiPath.Split(".");
            dictionary.Add(mapping.Name, GetNestedValue(serializedBody, parts, 0));
        } 
        return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
    }

    private dynamic GetNestedValue(dynamic obj, string[] parts, int index)
    {
        return index >= parts.Length ? obj : GetNestedValue(obj[parts[index]], parts, index + 1);
    }

    
    private dynamic BuildObject(string structure, string value, int skipCount = 0) {
        if (structure == string.Empty) {
            return value;
        }

        var parts = structure.Split('.');
        return new Dictionary<string, object> {{
            parts.First(), BuildObject(string.Join(".", parts.Skip(skipCount + 1)), value)
        }};
    }

    private dynamic GetValue(DiscordInteractionDataOption option)
    {
        if (option?.Value == null)
        {
            return null;
        }
        switch (option.Type)
        {
            case DiscordApplicationCommandOptionType.String:
                return option.Value.ToString();
            case DiscordApplicationCommandOptionType.Number:
                return decimal.Parse(option.Value);
            case DiscordApplicationCommandOptionType.Boolean:
                return bool.Parse(option.Value);
            case DiscordApplicationCommandOptionType.Integer:
                return int.Parse(option.Value);
            default:
                return option.Value;
        }
    }

    private dynamic? GetValue(DiscordApplicationCommandOptionType type, string value)
    {
        if (value == null)
        {
            return null;
        }
        switch (type)
        {
            case DiscordApplicationCommandOptionType.String:
                return value.ToString();
            case DiscordApplicationCommandOptionType.Number:
                return decimal.Parse(value);
            case DiscordApplicationCommandOptionType.Boolean:
                return bool.Parse(value);
            case DiscordApplicationCommandOptionType.Integer:
                return int.Parse(value);
            default:
                return value;
        }
    }

    private string MapUrlWithParams(string url, DiscordCommand command, DiscordInteraction interaction)
    {
        if (command.CommandInformation?.Options == null)
        {
            return url;
        }
        foreach (var option in command.CommandInformation.Options) 
        {
            var parameter = interaction.Data.Options.Where(x => x.Name == option.Name).First();
            var stringToReplace = "${" + parameter.Name + "}";
            url = url.Replace(stringToReplace, parameter.Value);
        }
        return url;
    }

    private void AddDefaultDataToExpando(Dictionary<string, object> expando, DiscordCommand command, DiscordInteraction interaction)
    {
        if (!command.DiscordCommandActions.Any())
        {
            return;
        }
        foreach (var defaultData in command.DiscordCommandActions[0]?.RestRequestMetadata?.DefaultRequestData)
        {
            var property = defaultData.Name;
            var parts = defaultData.ApiPath.Split(".");
            var index = 0;
            var current = expando;
            parts.ToList().ForEach(y => {

                var keyExists = current.ContainsKey(y);

                if (parts.Length - 1 == index++)
                {
                    if (keyExists)
                    {
                        current[property] = GetValue(defaultData.Type, defaultData.Value);
                    }
                    else
                    {
                        current.TryAdd(property, GetValue(defaultData.Type, defaultData.Value));
                    }
                }
                else
                {
                    if (keyExists)
                    {
                        current = expando[y] as Dictionary<string, object>;
                    }
                    else
                        current = new Dictionary<string, object>();
                    expando.TryAdd(y, current);
                }
            });
        }
    }

    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}

public class DiscordRestDataCommandResponse
{
    public string RequestJson { get; set; }
    public string ResponseJson { get; set; }
    public string RequestUrl { get; set; }
}