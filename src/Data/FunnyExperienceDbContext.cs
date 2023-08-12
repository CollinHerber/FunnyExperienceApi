using FunnyExperience.Server.Data.Configurations;
using FunnyExperience.Server.Data.Repositories;
using System.Linq.Expressions;
using FunnyExperience.Server.Models;
using FunnyExperience.Server.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

namespace FunnyExperience.Server.Data;

public class FunnyExperienceDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DbSet<Player> Player { get; set; }

    public FunnyExperienceDbContext(DbContextOptions options) : base(options)
    {
        //no-op
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Role");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogin");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRole");
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
    }
}