namespace CoinbaseCommerce.Models
{
	public class BlockPayload
    {
        public string Height { get; set; }
        public string Hash { get; set; }
        public long ConfirmationsAccumulated { get; set; }
        public long ConfirmationsRequired { get; set; }
    }
}
