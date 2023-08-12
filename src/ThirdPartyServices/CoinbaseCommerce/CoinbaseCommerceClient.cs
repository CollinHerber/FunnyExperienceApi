using Cowbot.Server.CoinbaseCommerce.Models;
using CoinbaseCommerce.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Cowbot.Server.Lib;
using Cowbot.Server.ThirdPartyServices.CoinbaseCommerce.Models;
using System.Text;

namespace Cowbot.Server.CoinbaseCommerce;

public class CoinbaseCommerceClient : ICoinbaseCommerceClient
{

    private readonly HttpClient _httpClient;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public CoinbaseCommerceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        SetupHttpClient();
    }

    public async Task<CreateChargeResponse> CreateCoinbasePayment(string name, string description, decimal amount, string key, string guildId, string channelId)
    {

        var createChargeDetails = new CreateChargePayload
        {
            Name = name,
            Description = description,
            PricingType = "fixed_price",
            LocalPrice = new PricingAmountPayload()
            {
                Amount = amount,
                Currency = "USD"
            },
            Metadata = new MetadataPayload
            {
                ServerId = guildId,
                ChannelId = channelId
            },
            RedirectUrl = "http://localhost:9500",
        };

        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-CC-Api-Key",  key);
        var url = "https://api.commerce.coinbase.com/charges";

        var json = JsonConvert.SerializeObject(createChargeDetails, _jsonSerializerSettings);

        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, stringContent);
        var body = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<CreateChargeResponse>(body, _jsonSerializerSettings);

        return result;
    }

    public async Task<List<CoinbaseCharge>> GetChargeList()
    {
        var url = "https://api.commerce.coinbase.com/charges";

        var response = await _httpClient.GetAsync(url);

        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<CoinbaseCharge>>(body, _jsonSerializerSettings);
        return result;
    }

    public async Task<CoinbaseCharge> GetChargeById(string id)
    {
        var url = "https://api.commerce.coinbase.com/charges/" + id;

        var response = await _httpClient.GetAsync(url);

        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CoinbaseCharge>(body, _jsonSerializerSettings);
        return result;
    }

    private void SetupHttpClient()
    {


        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("X-CC-Version", "2018-03-22");
    }

}