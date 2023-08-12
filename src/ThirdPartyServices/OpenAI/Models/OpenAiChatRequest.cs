namespace Cowbot.Server.ThirdPartyServices.OpenAI.Models;

public class OpenAiChatRequest
{
    public string Model { get; set; }
    public List<OpenAiChatRequestMessage> Messages { get; set; }
    public double? Temperature { get; set; }
}

public class OpenAiChatRequestMessage
{
    public string Role { get; set; }
    public string Content { get; set; }
}