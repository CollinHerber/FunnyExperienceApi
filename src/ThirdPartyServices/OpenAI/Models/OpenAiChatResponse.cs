using System.Numerics;

namespace Cowbot.Server.ThirdPartyServices.OpenAI.Models;

public class OpenAiChatResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public int Created { get; set; }
    public string Model { get; set; }
    public OpenAiChatResponseUsage Usage { get; set; }
    public List<OpenAiChatResponseChoices> Choices { get; set; }
}

public class OpenAiChatResponseUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

public class OpenAiChatResponseChoices
{
    public OpenAiChatResponseChoiceMessage Message { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public int TotalTokens { get; set; }
}

public class OpenAiChatResponseChoiceMessage
{
    public string Role { get; set; }
    public string Content { get; set; }
}