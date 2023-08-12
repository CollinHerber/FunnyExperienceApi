namespace FunnyExperience.Server.Lib.Exceptions;

public class DiscordApiException: BaseException
{
    public DiscordApiException(string message, string url) : base(message) { }
}