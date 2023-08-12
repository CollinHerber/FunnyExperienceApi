using Cowbot.Server.Configuration;
using Cowbot.Server.Redis.Interfaces;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Cowbot.Server.Redis.Interfaces;

namespace Cowbot.Server.Redis.Repositories
{
	public class JwtRepository : IJwtRepository {
        private readonly IConnectionMultiplexer _client;

        public JwtRepository(IConfiguration configuration) {
#if !DEBUG
            _client = ConnectionMultiplexer.Connect(configuration.RedisConnectionString());
#endif
        }

        public void Add(string jwt, Guid userId) {
            //Remove all previous JWT since user just logged in and this should be only valid JWT
            //This will only allow a user to be signed in at one place at a time
            //Remove this to allow multiple tokens that will only be cleared on logout or expiration
            //RemoveByUserId(userId);
            var db = _client.GetDatabase();
            db.SetAdd(userId.ToString(), jwt);
        }

        public void RemoveByUserId(Guid userId) {
#if !DEBUG
            var db = _client.GetDatabase();
            db.KeyDelete(userId.ToString());
#endif
        }

        public bool JwtExists(Guid userId, string jwt) {
            var db = _client.GetDatabase();
            return db.SetContains(userId.ToString(), jwt);
        }
    }
}
