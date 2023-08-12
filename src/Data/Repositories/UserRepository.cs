using AutoMapper;
using AutoMapper.QueryableExtensions;
using FunnyExperience.Server.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using FunnyExperience.Server.Models;
using FunnyExperience.Server.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace FunnyExperience.Server.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly FunnyExperienceDbContext _db;
    private readonly static Lazy<IEnumerable<string>> SPropertiesToIgnore = new(new List<string>());
    private readonly static Lazy<IEnumerable<PropertyInfo>> SUserProperties = new(typeof(User).GetProperties());


    public UserRepository(UserManager<User> userManager, IMapper mapper, FunnyExperienceDbContext db)
    {
        _userManager = userManager;
        _mapper = mapper;
        _db = db;
    }

    public async Task AddRoleAsync(Guid userId, string role)
    {
        var user = await GetByIdAsync(userId);
        await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<List<string>> GetRoles(Guid userId)
    {
        return (await _userManager.GetRolesAsync(await GetByIdAsync(userId))).ToList();
    }

    public async Task<IEnumerable<User>> GetUsersInRoleAsync(string role)
    {
        return await _userManager.GetUsersInRoleAsync(role);
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _userManager.Users
            .IgnoreQueryFilters()
            .Where(x => x.Id == id)
            .FirstAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _userManager.Users
            .AnyAsync(x => x.Id == id);
    }

    public async Task<User> GetByIdAsNoTracking(Guid id)
    {
        return await _userManager.Users.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<TDto> GetByIdAsync<TDto>(Guid id)
    {
        return await _userManager.Users
            .IgnoreQueryFilters()
            .Where(x => x.Id == id)
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByDiscordId(string discordId)
    {
        return await _userManager.Users.Where(x => x.DiscordId == discordId).FirstOrDefaultAsync();
    }

    public async Task<Guid> CreateAsync(User user, string password)
    {
        user.CreatedDate = DateTime.UtcNow;
        user.UserName = user.Email;
        user.PhoneNumberConfirmed = false;
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
        }

        await _userManager.SetLockoutEnabledAsync(user, false);

        return user.Id;
    }

    public async Task UpdateAsync(User user, string[] roles = null)
    {
        var currentUser = await GetByIdAsync(user.Id);

        _db.Entry(currentUser).CurrentValues.SetValues(user);
        foreach (var property in SUserProperties.Value)
        {
            var propValue = property.GetValue(user, null);

            if (SPropertiesToIgnore.Value.Contains(property.Name)) continue;
            if (propValue != null) continue;
            _db.Entry(currentUser).Property(property.Name).CurrentValue = null;
        }

        _db.Entry(currentUser).Property(x => x.ConcurrencyStamp).IsModified = false;

        var result = await _userManager.UpdateAsync(currentUser);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("\n", result.Errors));
        }

        if (roles != null)
        {
            await _userManager.RemoveFromRolesAsync(currentUser, await _userManager.GetRolesAsync(currentUser));
            await _userManager.AddToRolesAsync(currentUser, roles);
        }

        await _userManager.UpdateNormalizedEmailAsync(currentUser);
        await _userManager.UpdateNormalizedUserNameAsync(currentUser);
    }

    public async Task<bool> ResetPasswordAsync(string email, string password, string token)
    {
        var user = await GetByEmailAsync(email);

        foreach (var validator in _userManager.PasswordValidators)
        {
            var validateCheck = await validator.ValidateAsync(_userManager, user, password);
            if (!validateCheck.Succeeded)
            {
                return false;
            }
        }

        var resetStatus = await _userManager.ResetPasswordAsync(user, token, password);
        if (!resetStatus.Succeeded)
            return false;

        await _userManager.SetLockoutEnabledAsync(user, false);
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(-1));

        return true;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.Users.Where(x => x.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task UpdatePasswordAsync(Guid id, string password)
    {
        var user = await GetByIdAsync(id);

        foreach (var validator in _userManager.PasswordValidators)
        {
            var validateCheck = await validator.ValidateAsync(_userManager, user, password);
            if (!validateCheck.Succeeded)
            {
                return;
                //throw new InvalidEntityException(string.Join("\n", validateCheck.Errors.Select(x => x.Description)));
            }
        }

        await _userManager.UpdateSecurityStampAsync(user);
        user.UserName = user.Email;
        await UpdateAsync(user);

        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, password);
        await _userManager.SetLockoutEnabledAsync(user, false);
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(-1));
    }

    public async Task<string> GeneratePasswordResetTokenAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            return false;
            //throw new EntityNotFoundException($"{EntityName} not found with the id: {id}");
        }
        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
        entity.UserName = entity.UserName + "-Deleted" + timeStamp;
        entity.NormalizedUserName = entity.NormalizedUserName + "-Deleted" + timeStamp;
        entity.Email = entity.Email + "-Deleted" + timeStamp;
        entity.NormalizedEmail = entity.NormalizedEmail + "-Deleted" + timeStamp;
        await UpdateAsync(entity);
        return true;
    }

    public async Task<User> GetProfile(Guid id)
    {
        return await _userManager.Users
            .IgnoreQueryFilters()
            .Where(x => x.Id == id)
            .SingleAsync();
    }
}