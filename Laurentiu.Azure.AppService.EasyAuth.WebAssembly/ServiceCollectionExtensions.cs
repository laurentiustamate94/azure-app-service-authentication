using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Laurentiu.Azure.AppService.EasyAuth.WebAssembly
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServiceEasyAuth(this IServiceCollection services)
        {
            return services.AddAppServiceEasyAuth(options => { });
        }

        public static IServiceCollection AddAppServiceEasyAuth(this IServiceCollection services, Action<RemoteAuthenticationOptions<AppServiceRemoteProviderOptions>> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<AuthenticationStateProvider, AppServiceRemoteAuthenticationService>();

            services.AddRemoteAuthentication<RemoteAuthenticationState, RemoteUserAccount, AppServiceRemoteProviderOptions>(configureOptions);

            return services;
        }
    }
}