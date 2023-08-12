using System;
using System.Threading.Tasks;
using FunnyExperience.Server.Models.DTOs.User.Request;
using FunnyExperience.Server.Models.DTOs.User.Response;

namespace FunnyExperience.Server.Services.Interfaces;

public interface IUserService
{
    Task<LoginResponse> Authenticate(string username, string password);
    Task<LoginResponse> CreateUser(RegisterUserDto user);
    Task<UserNameResponse> FindByEmail(RequestResetPasswordRequest data);
    Task<bool> UpdatePassword(Guid id, UpdatePasswordRequest resetPasswordRequest);
    Task<LoginResponse> RefreshToken(Guid id);
    void Logout(Guid userId);
    Task<ProfileResponse> GetProfile(Guid userId);
}