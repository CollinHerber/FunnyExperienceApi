using FunnyExperience.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FunnyExperience.Server.Data;

public static class Seeder {
    private static IList<IdentityRole<Guid>> s_roleList = new List<IdentityRole<Guid>>();

    public static async Task SeedDataAsync(IServiceScope scope, FunnyExperienceDbContext db) {
        using (var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>()) {
            await SeedRolesAsync(roleManager);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager) {
        s_roleList = await roleManager.Roles.ToListAsync();

        foreach (var permission in Enum.GetValues(typeof(AuthorizationPolicyType)).Cast<AuthorizationPolicyType>()) {
            await SeedRoleAsync(roleManager, Enum.GetName(typeof(AuthorizationPolicyType), permission));
        }
    }

    private static async Task SeedRoleAsync(RoleManager<IdentityRole<Guid>> roleManager, string name) {
        if (!s_roleList.Any(r => r.Name == name)) {
            var role = new IdentityRole<Guid> { Name = name };
            await roleManager.CreateAsync(role);
            s_roleList.Add(role);
        }
    }
}