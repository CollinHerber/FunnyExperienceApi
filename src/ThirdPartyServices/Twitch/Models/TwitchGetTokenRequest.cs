namespace Cowbot.Server.ThirdPartyServices.Twitch.Models;

public class TwitchGetTokenRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string GrantType  { get; set; }
    public string Scope { get; set; }
}