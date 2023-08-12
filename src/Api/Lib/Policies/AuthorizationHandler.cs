using FunnyExperience.Server.Data.Repositories.Interfaces;
using FunnyExperience.Server.Lib.Extensions;
using FunnyExperience.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Api.Lib.Policies;

public class AuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>, IAuthorizationHandler {
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement) {
        var userId = _httpContextAccessor.HttpContext.GetUserId();
        var roles = await _userRepository.GetRoles(userId);

        var isAllowed = false;
        if (roles?.Count > 0)
        {
            isAllowed = roles.Contains(Enum.GetName(typeof(AuthorizationPolicyType), requirement.Type));
        }

        if (isAllowed) {
            context.Succeed(requirement);
        } else {
            context.Fail();
        }
    }
}