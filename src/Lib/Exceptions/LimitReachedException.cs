namespace FunnyExperience.Server.Lib.Exceptions;

public class LimitReachedException: BaseException
{
    public LimitReachedException(string message) : base(message) { }
}