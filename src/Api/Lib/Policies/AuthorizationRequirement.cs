using FunnyExperience.Server.Models;
using Microsoft.AspNetCore.Authorization;

namespace FunnyExperience.Server.Api.Lib.Policies;

public class AuthorizationRequirement : IAuthorizationRequirement {
    public readonly AuthorizationPolicyType Type;

    public AuthorizationRequirement(AuthorizationPolicyType type) => Type = type;
}