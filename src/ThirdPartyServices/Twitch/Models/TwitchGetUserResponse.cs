namespace Cowbot.Server.ThirdPartyServices.Twitch.Models;

public class TwitchGetUserResponse
{
    public List<TwitchUser> Data { get; set; }
}

public class TwitchUser
{
    public string Id { get; set; }
    public string Login { get; set; }
    public string DisplayName { get; set; }
    public string Type { get; set; }
    public string BroadcasterType { get; set; }
    public string Description { get; set; }
    public string ProfileImageUrl { get; set; }
    public string OfflineImageUrl { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}