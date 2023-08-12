namespace FunnyExperience.Server.Models.DTOs.User.Request;

public class ResetPasswordRequest
{
    public string Email { get; set; }
    public string NewPassword {get; set;}
    public string AccessToken {get; set;}
}