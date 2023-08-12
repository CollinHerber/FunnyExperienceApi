using System;

namespace FunnyExperience.Server.Models.DTOs.User.Response;

public class UserNameResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DiscordId { get; set; }
}