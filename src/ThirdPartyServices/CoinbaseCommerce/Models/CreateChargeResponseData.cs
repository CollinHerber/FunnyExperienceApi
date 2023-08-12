using System.Collections.Generic;
using CoinbaseCommerce.Models;

namespace Cowbot.Server.CoinbaseCommerce.Models
{
    public class CreateChargeResponseData : ChargeBasicDetails
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string LogoUrl { get; set; }

        public string HostedUrl { get; set; }

        public string CreatedAt { get; set; }

        public string ExpiresAt { get; set; }

        public List<TimelinePayload> Timeline { get; set; }

        public MetadataPayload Metadata { get; set; }

        public string PricingType { get; set; }

        public PricingPayload Pricing { get; set; }

        public AddressesPayload Addresses { get; set; }

        public List<PaymentPayload> Payments { get; set; }

        public string RedirectUrl { get; set; }

        public string CancelUrl { get; set; }

    }
}
