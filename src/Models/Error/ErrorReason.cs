namespace FunnyExperience.Server.Models.DTOs.Error;

public enum ErrorReason {
    Unauthorized,
    ServerError,
    ValidationError,
    InvalidData,
    LimitReached
}