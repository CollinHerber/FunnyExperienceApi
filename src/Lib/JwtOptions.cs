using Microsoft.IdentityModel.Tokens;

namespace FunnyExperience.Server.Lib;

public class JwtOptions
{
    public SigningCredentials SigningCredentials { get; set; }
}