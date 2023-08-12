using Cowbot.Server.CoinbaseCommerce.Models;
using Cowbot.Server.Models;
using System.Threading.Tasks;

namespace Cowbot.Server.CoinbaseCommerce
{
	public interface ICoinbaseCommerceClient
	{
		Task<CreateChargeResponse> CreateCoinbasePayment(string name, string description, decimal amount, string key, string guildId, string channelId);

		Task<CoinbaseCharge> GetChargeById(string id);
	}
}
