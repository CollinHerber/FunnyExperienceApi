namespace Cowbot.Server.Redis.Interfaces
{
	public interface IJwtRepository {
        void Add(string jwt, Guid userId);
        void RemoveByUserId(Guid userId);
        bool JwtExists(Guid userId, string jwt);
    }
}
