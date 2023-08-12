using FunnyExperience.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FunnyExperience.Server.Configuration;

namespace FunnyExperience.Server.Data;

public static class DatabaseExtensions
{
    public static IHost Migrate(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<FunnyExperienceDbContext>();
        dbContext.Database.Migrate();

        return host;
    }

    public static IHost Seed(this IHost webHost) {
        using var scope = webHost.Services.GetService<IServiceScopeFactory>()?.CreateScope();
        using var dbContext = scope?.ServiceProvider.GetRequiredService<FunnyExperienceDbContext>();
        if (scope == null) return webHost;
        if (dbContext != null)
            Seeder.SeedDataAsync(scope, dbContext).GetAwaiter().GetResult();

        return webHost;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FunnyExperienceDbContext>(options =>
        {
            var connectionString = configuration.DbConnectionString();

            options.UseNpgsql(connectionString);
#if DEBUG
            options.EnableSensitiveDataLogging();      
#endif
        });
        return services;
    }
}