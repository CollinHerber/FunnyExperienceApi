using FunnyExperience.Server.Api.Lib.Policies;
using FunnyExperience.Server.Configuration;
using FunnyExperience.Server.Data;
using FunnyExperience.Server.Lib;
using FunnyExperience.Server.Models.DatabaseModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Api.Lib.Extensions;

public static class AuthenticationExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var keyBytes = Encoding.ASCII.GetBytes(configuration.JwtKey());
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        
        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddUserStore<UserStore<User, IdentityRole<Guid>, FunnyExperienceDbContext, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>>()
            .AddRoleStore<RoleStore<IdentityRole<Guid>, FunnyExperienceDbContext, Guid, IdentityUserRole<Guid>, IdentityRoleClaim<Guid>>>()
            .AddDefaultTokenProviders();
        
        services.Configure<IdentityOptions>(options => {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 30, 0);
            options.User.RequireUniqueEmail = true;
        });

        services.Configure<JwtOptions>(options =>
        {
            options.SigningCredentials = signingCredentials;
        });

        services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "CustomJwt";
                options.DefaultScheme = "CustomJwt";
                options.DefaultChallengeScheme = "CustomJwt";

            })
            .AddScheme<JwtBearerOptions, JwtAuthenticationHandler>("CustomJwt", options => {
                options.AddQueryStringAuthentication();
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingCredentials.Key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }
    private static void AddQueryStringAuthentication(this JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = (context) => {
                if (!context.Request.Query.TryGetValue("access_token", out var values))
                {
                    return Task.CompletedTask;
                }

                if (values.Count > 1)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    context.Fail(
                        "Only one 'access_token' query string parameter can be defined. " +
                        $"However, {values.Count:N0} were included in the request."
                    );

                    return Task.CompletedTask;
                }

                var token = values.Single();

                if (string.IsNullOrWhiteSpace(token))
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    context.Fail(
                        "The 'access_token' query string parameter was defined, " +
                        "but a value to represent the token was not included."
                    );

                    return Task.CompletedTask;
                }

                context.Token = token;

                return Task.CompletedTask;
            }
        };
    }
}