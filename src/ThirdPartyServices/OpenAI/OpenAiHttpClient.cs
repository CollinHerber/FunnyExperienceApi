using Cowbot.Server.Lib;
using Cowbot.Server.ThirdPartyServices.OpenAI.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Cowbot.Server.ThirdPartyServices.OpenAI;
public class OpenAiHttpClient : IOpenAiHttpClient
{

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public OpenAiHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        SetupHttpClient();
    }

    public async Task<OpenAiChatResponse> MakeChatGptRequest(string message)
    {
        var response = await _httpClient.PostAsync("chat/completions", CreateRequestObject(new OpenAiChatRequest {
            Model = "gpt-3.5-turbo",
            Messages = new List<OpenAiChatRequestMessage> {
                new () {
                    Role = "user",
                    Content = message
                }
            }
        }));
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<OpenAiChatResponse>(body, _jsonSerializerSettings);
        return result;
    }
    
    private StringContent CreateRequestObject(dynamic obj)
    {
        var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
