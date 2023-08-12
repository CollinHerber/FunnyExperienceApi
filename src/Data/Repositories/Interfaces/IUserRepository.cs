using FunnyExperience.Server.Models.DatabaseModels;

namespace FunnyExperience.Server.Data.Repositories.Interfaces;

public interface IUserRepository {
    Task<User> GetByIdAsync(Guid id);
    Task<TDto> GetByIdAsync<TDto>(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByDiscordId(string discordId);
    Task<Guid> CreateAsync(User user, string password);
    Task UpdateAsync(User user, string[] roles = null);
    Task UpdatePasswordAsync(Guid id, string password);
    Task<string> GeneratePasswordResetTokenAsync(Guid id);
    Task<bool> ResetPasswordAsync(string email, string password, string token);
    Task<User> GetByIdAsNoTracking(Guid id);
    Task<List<string>> GetRoles(Guid userId);
    Task AddRoleAsync(Guid userId, string role);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<User> GetProfile(Guid id);

}