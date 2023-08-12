using Newtonsoft.Json;

namespace Cowbot.Server.CoinbaseCommerce.Models
{
    public class MetadataPayload
    {
        [JsonProperty("server_id")]
        public string ServerId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}
