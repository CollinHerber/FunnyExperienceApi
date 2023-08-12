using FunnyExperience.Server.Configuration;
using FunnyExperience.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FunnyExperience.Server.Api;

public class FunnyExperienceDbFactory : IDesignTimeDbContextFactory<FunnyExperienceDbContext>
{
    public FunnyExperienceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var builder = new DbContextOptionsBuilder<FunnyExperienceDbContext>();

        var connectionString = configuration.DbConnectionString();
        builder.UseNpgsql(connectionString);

        return new FunnyExperienceDbContext(builder.Options);
    }
}