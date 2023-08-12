using Cowbot.Server.CoinbaseCommerce.Models;
using CoinbaseCommerce.Models;

namespace Cowbot.Server.ThirdPartyServices.CoinbaseCommerce.Models
{
    public class CreateChargePayload
    {
        public string Name {get; set;}

        public string Description { get; set; }

        public string PricingType { get; set; }

        public PricingAmountPayload LocalPrice { get; set; }

        public MetadataPayload Metadata { get; set; }

        public string RedirectUrl { get; set; }

        public string CancelUrl { get; set; }
    }
}
