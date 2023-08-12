using FunnyExperience.Server.Services;
using FunnyExperience.Server.Services.Interfaces;
using FunnyExperience.Server.Api.Lib.Policies;
using FunnyExperience.Server.Data.Repositories;
using FunnyExperience.Server.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FunnyExperience.Server.Api;

public static class InjectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthorizationHandler, AuthorizationHandler>();
        services.AddScoped<IPlayerService, PlayerService>();
        return services;
    }
}