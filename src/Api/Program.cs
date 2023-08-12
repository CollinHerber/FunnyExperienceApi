using FunnyExperience.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace FunnyExperience.Server.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build()
            .Migrate()
            .Seed()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => {
                config.AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseKestrel(options => {
                    options.Listen(IPAddress.Any, 5000);
                });
                webBuilder.UseStartup<Startup>();
            });
}