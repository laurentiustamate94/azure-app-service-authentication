using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Laurentiu.Azure.AppService.EasyAuth.Web
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapEasyAuthDevEndpoints(this IEndpointRouteBuilder endpoints, bool useEmptyAuthMeJson = false)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            endpoints.MapGet(AppServiceAuthenticationDefaults.AuthMeEndpoint, ([FromQuery] string? provider) =>
            {
                return useEmptyAuthMeJson ? AppServiceAuthenticationDefaults.EmptyAuthMeJson : AppServiceAuthenticationDefaults.DevAuthMeJson;
            });

            endpoints.MapGet(AppServiceAuthenticationDefaults.AuthLoginEndpointPattern, ([FromRoute] string provider, [FromQuery] string? post_login_redirect_uri) =>
            {
                return Results.Redirect(post_login_redirect_uri ?? "/", permanent: true);
            });

            endpoints.MapGet(AppServiceAuthenticationDefaults.AuthLogoutEndpointPattern, ([FromQuery] string? post_logout_redirect_uri) =>
            {
                return Results.Redirect(post_logout_redirect_uri ?? "/", permanent: true);
            });
        }
    }
}
