namespace FunnyExperience.Server.Models.DTOs.User.Request;

public class ConfirmEmailRequest
{
    public string Email {get; set;}
    public string Token {get; set;}
}