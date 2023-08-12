using Microsoft.Extensions.DependencyInjection;
using Cowbot.Server.Redis.Interfaces;
using Cowbot.Server.Redis.Repositories;

namespace Cowbot.Server.Redis {
    public static class Injection {
        public static void AddRedis(this IServiceCollection services) {
            services.AddScoped<IJwtRepository, JwtRepository>();
        }
    }
}
