using Microsoft.AspNetCore.Authentication;

namespace Laurentiu.Azure.AppService.EasyAuth.Web
{
    public class AppServiceAuthenticationOptions : AuthenticationSchemeOptions
    {
        public bool UseDevIdentity { get; set; } = false;

        public List<string> WhitelistedNameClaimValues { get; set; } = new();
    }
}
