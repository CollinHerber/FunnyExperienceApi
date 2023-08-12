using Cowbot.Server.Models.IpQualityScoreModels;
using Cowbot.Server.ThirdPartyServices.OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cowbot.Server.ThirdPartyServices.OpenAI;
public interface IOpenAiHttpClient
{
    Task<OpenAiChatResponse> MakeChatGptRequest(string message);
}
