using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FunnyExperience.Server.Models.DTOs.User.Request;
using FunnyExperience.Server.Models.DTOs.User.Response;
using FunnyExperience.Server.Services.Interfaces;
using FunnyExperience.Server.Configuration;
using FunnyExperience.Server.Data.Repositories.Interfaces;
using FunnyExperience.Server.Models.DatabaseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FunnyExperience.Server.Services;

public class UserService : IUserService
{ 
    private readonly IMapper _mapper;
    private readonly SignInManager<User> _signinManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo,
        IMapper mapper,
        SignInManager<User> signinManager,
        IConfiguration config,
        UserManager<User> userManager,
        IConfiguration configuration,
        IWebHostEnvironment hostEnvironment
    )
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _signinManager = signinManager;
        _config = config;
        _userManager = userManager;
    }

    public async Task<LoginResponse> Authenticate(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _userRepo.GetByEmailAsync(email);

        if (user == null)
            return null;

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            await _userManager.UpdateSecurityStampAsync(user);
            return new LoginResponse
            {
                Id = user.Id,
                ResetPassword = true
            };
        }

        var result = await _signinManager.PasswordSignInAsync(email, password.Trim(), false, true);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid Login");
        }

        await _userRepo.UpdateAsync(user);

        var roles = await _userRepo.GetRoles(user.Id);
        return new LoginResponse
        {
            Token = GenerateJwtToken(user),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(40320).ToUnixTimeMilliseconds(),
            Id = user.Id
        };
    }

    public async Task<LoginResponse> RefreshToken(Guid id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        var roles = await _userRepo.GetRoles(id);

        return new LoginResponse
        {
            Token = GenerateJwtToken(user),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(40320).ToUnixTimeMilliseconds(),
            Id = user.Id
        };
    }

    private string GenerateJwtToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(40320);

        var payload = new JwtPayload {
            { "sub", user.Id.ToString() },
            { "jti", Guid.NewGuid().ToString() },
            { "id", user.Id.ToString() },
            { "exp", (int)expires.Subtract(new DateTime(1970, 1, 1)).TotalSeconds }
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.JwtKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(new JwtHeader(creds), payload);

        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

        return stringToken;
    }

    public void Logout(Guid userId)
    {
        //no-op
#if !DEBUG
            //_jwtRepository.RemoveByUserId(userId);
#endif
    }

    public async Task<LoginResponse> CreateUser(RegisterUserDto registerUser)
    {
        var existingUser = await _userRepo.GetByEmailAsync(registerUser.Email);

        if (existingUser != null)
        {
            return new LoginResponse
            {
                AlreadyRegistered = true
            };
        }

        var user = _mapper.Map<User>(registerUser);

        await _userRepo.CreateAsync(user, registerUser.Password);
        if (user.Id == Guid.Empty)
        {
            throw new Exception("Failed to create user");
        }

        return await Authenticate(registerUser.Email, registerUser.Password);
    }
    
    public async Task<bool> UpdatePassword(Guid id, UpdatePasswordRequest resetPasswordRequest)
    {
        var user = await _userRepo.GetByIdAsync(id);

        if (!await _userManager.CheckPasswordAsync(user, resetPasswordRequest.CurrentPassword))
        {
            return false;
        }
        await _userRepo.UpdatePasswordAsync(id, resetPasswordRequest.NewPassword);
        return true;
    }

    public async Task<UserNameResponse> FindByEmail(RequestResetPasswordRequest data)
    {
        return _mapper.Map<UserNameResponse>(await _userRepo.GetByEmailAsync(data.Email));
    }
    

    public async Task<ProfileResponse> GetProfile(Guid userId)
    {
        var user = await _userRepo.GetProfile(userId);
        if (user.FirstName == null) {
            await _userRepo.UpdateAsync(user);
        }
        return new ProfileResponse()
        {
            Id = userId,
            DiscordId = user.DiscordId,
        };
    }
}