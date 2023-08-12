using Cowbot.Server.Models.IpQualityScoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cowbot.Server.ThirdPartyServices.IpQualityScore;
public interface IIpQualityScoreClient
{
    Task<IpQualityResponse> GetIPQuality(string ip, string user_agent);
    Task<EmailQualityResponse> GetEmailQualityScore(string email);

}
