using System;

namespace FunnyExperience.Server.Models.DTOs.User.Response;

public class LoginResponse {
    public string Token { get; set; }
    public Guid Id { get; set; }
    public long ExpiresAt { get; set; }
    public bool ResetPassword { get; set; }
    public bool TokenRequired { get; set; }
    public bool AlreadyRegistered { get; set; }
}