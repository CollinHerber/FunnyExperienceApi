using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Api.Lib.Policies;

public class JwtAuthenticationHandler : JwtBearerHandler {

    public JwtAuthenticationHandler(IConfiguration configuration, IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) {

    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        return await base.HandleAuthenticateAsync();
    }
}