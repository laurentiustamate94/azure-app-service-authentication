using Microsoft.Extensions.DependencyInjection;

namespace Laurentiu.Azure.AppService.EasyAuth.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServiceEasyAuth(this IServiceCollection services)
        {
            return services.AddAppServiceEasyAuth(options => { });
        }

        public static IServiceCollection AddAppServiceEasyAuth(this IServiceCollection services, Action<AppServiceAuthenticationOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddAuthentication(AppServiceAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<AppServiceAuthenticationOptions, AppServiceAuthenticationHandler>(
                    AppServiceAuthenticationDefaults.AuthenticationScheme,
                    AppServiceAuthenticationDefaults.AuthenticationScheme,
                    configureOptions);

            return services;
        }
    }
}