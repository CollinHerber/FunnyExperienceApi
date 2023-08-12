using System;

namespace FunnyExperience.Server.Models.DTOs.User.Response;

public class ProfileResponse
{
    public string Token { get; set; }
    public Guid Id { get; set; }
    public string DiscordId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Avatar { get; set; }
}