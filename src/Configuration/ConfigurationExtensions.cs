using Microsoft.Extensions.Configuration;

namespace FunnyExperience.Server.Configuration;

public static class ConfigurationExtensions
{
    public static string DbConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("Db") ?? configuration["Db"];
    }

    public static string JwtKey(this IConfiguration configuration)
    {
        return configuration.GetConfig("JwtKey");
    }

    public static string RedirectUrl(this IConfiguration configuration)
    {
        return configuration.GetConfig("RedirectUrl");
    }

    public static string PasswordCreationSuffix(this IConfiguration configuration)
    {
        return configuration.GetConfig("PasswordCreationSuffix");
    }

    public static string BaseApiUrl(this IConfiguration configuration)
    {
        return configuration.GetConfig("BaseApiUrl");
    }
    public static string WebAppUrl(this IConfiguration configuration)
    {
        return configuration.GetConfig("WebAppUrl");
    }

    public static string RedisConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConfig("RedisDb");
    }
    private static string GetConfig(this IConfiguration configuration, string key)
    {
        return configuration[key];
    }

    private static string GetSectionConfig(this IConfiguration configuration, string section, string key)
    {
        return configuration.GetSection(section).GetConfig(key);
    }
}