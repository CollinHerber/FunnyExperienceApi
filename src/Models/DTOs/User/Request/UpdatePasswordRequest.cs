namespace FunnyExperience.Server.Models.DTOs.User.Request;

public class UpdatePasswordRequest
{
    public string CurrentPassword {get; set;}
    public string NewPassword {get; set;}
}