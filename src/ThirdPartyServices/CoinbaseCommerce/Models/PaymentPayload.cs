namespace CoinbaseCommerce.Models
{
	public class PaymentPayload
	{
		public string Network { get; set; }

		public string TransactionId { get; set; }

		public string Status { get; set; }

		public PricingPayload Value { get; set; }

		public BlockPayload Block { get; set; }
	}
}
