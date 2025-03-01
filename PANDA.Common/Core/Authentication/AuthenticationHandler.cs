using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PANDA.Common.Core.Authentication;

public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public AuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // --------------------
            // Setup claims
            // --------------------

            List<Claim> claims =[];
            
            // Culture
            if (Request.Headers.TryGetValue("PANDA-Culture", out var culture))
            {
                claims.Add(new(AuthenticationCommon.Static.Claims.Culture, culture.ToString()));
            }

            // Declare
            // --------------------

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            // Cleanup & respond
            // --------------------

            //AccountExtensions.ExtendSession(claims);
            return AuthenticateResult.Success(ticket);
        }
    }