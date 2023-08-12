namespace CoinbaseCommerce.Models
{
	public class PricingPayload
	{
		public PricingAmountPayload Local { get; set; }

		public PricingAmountPayload Bitcoin { get; set; }

		public PricingAmountPayload Ethereum { get; set; }
        public PricingAmountPayload Litecoin { get; set; }

        public PricingAmountPayload Bitcoincash { get; set; }
        public PricingAmountPayload Usdc { get; set; }
        public PricingAmountPayload Dai { get; set; }
    }
}
