using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Laurentiu.Azure.AppService.EasyAuth.Web
{
    public class AppServiceAuthenticationHandler : AuthenticationHandler<AppServiceAuthenticationOptions>
    {
        public AppServiceAuthenticationHandler(
            IOptionsMonitor<AppServiceAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.UseDevIdentity)
            {
                var devPrincipal = new ClaimsPrincipal(AuthMeParser.DevIdentity);
                return AuthenticateResult.Success(new AuthenticationTicket(devPrincipal, AppServiceAuthenticationDefaults.AuthenticationScheme));
            }

            try
            {
                var principal = ClaimsPrincipalParser.Parse(Request);

                if (Options.WhitelistedNameClaimValues.Any()
                    && !Options.WhitelistedNameClaimValues.Contains(principal.Identity.Name))
                {
                    Logger.LogError($"User not whitelisted. Name: {principal.Identity.Name}");

                    return AuthenticateResult.Fail("Not authorized.");
                }

                return AuthenticateResult.Success(new AuthenticationTicket(principal, AppServiceAuthenticationDefaults.AuthenticationScheme));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Could not parse claims from request.");
            }

            return AuthenticateResult.NoResult();
        }
    }
}
