using System;

namespace FunnyExperience.Server.Lib.Exceptions;

public class BaseException : Exception {
    public BaseException(string message) : base(message) { }
}