using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace Laurentiu.Azure.AppService.EasyAuth.WebAssembly
{
    public class AppServiceRemoteAuthenticationService : AuthenticationStateProvider, IRemoteAuthenticationService<RemoteAuthenticationState>
    {
        private readonly AppServiceRemoteProviderOptions providerOptions;
        private readonly RemoteAuthenticationApplicationPathsOptions authenticationPaths;
        private readonly HttpClient httpClient;
        private readonly NavigationManager navigation;
        private readonly IJSRuntime jsRuntime;
        private readonly ILogger<AppServiceRemoteAuthenticationService> logger;

        public AppServiceRemoteAuthenticationService(
            IOptions<RemoteAuthenticationOptions<AppServiceRemoteProviderOptions>> options,
            NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            ILogger<AppServiceRemoteAuthenticationService> logger)
        {
            this.providerOptions = options.Value.ProviderOptions;
            this.authenticationPaths = options.Value.AuthenticationPaths;
            this.httpClient = new HttpClient() { BaseAddress = new Uri(navigationManager.BaseUri) };
            this.navigation = navigationManager;
            this.jsRuntime = jsRuntime;
            this.logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // TODO check if security exploitable
            if (providerOptions.UseDevIdentity)
            {
                return new AuthenticationState(new ClaimsPrincipal(AuthMeParser.DevIdentity));
            }

            try
            {
                var data = await httpClient.GetStringAsync(AppServiceAuthenticationDefaults.AuthMeEndpoint);
                var identity = AuthMeParser.Parse(data);

                if (providerOptions.WhitelistedNameClaimValues.Any()
                    && !providerOptions.WhitelistedNameClaimValues.Contains(identity.Name))
                {
                    logger.LogError($"User not whitelisted. Name: {identity.Name}");

                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Could not get authentication state.");
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            var stateId = new Uri(context.Url).PathAndQuery.Split("?")[0].Split("/", StringSplitOptions.RemoveEmptyEntries).Last();
            var serializedState = await jsRuntime.InvokeAsync<string>($"{providerOptions.BrowserStorageType}.getItem", $"{providerOptions.StorageKeyPrefix}.{stateId}");
            var state = JsonSerializer.Deserialize<RemoteAuthenticationState>(serializedState)
                ?? throw new InvalidOperationException("The stored authentication state is invalid for the current structure.");

            return new RemoteAuthenticationResult<RemoteAuthenticationState>
            {
                State = state,
                Status = RemoteAuthenticationStatus.Success
            };
        }

        public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            var sessionKeys = await jsRuntime.InvokeAsync<string[]>("eval", $"Object.keys({providerOptions.BrowserStorageType})");
            var stateKey = sessionKeys.FirstOrDefault(key => key.StartsWith(providerOptions.StorageKeyPrefix));

            if (stateKey != null)
            {
                await jsRuntime.InvokeAsync<string>($"{providerOptions.BrowserStorageType}.removeItem", stateKey);
            }

            return new RemoteAuthenticationResult<RemoteAuthenticationState> { Status = RemoteAuthenticationStatus.Success };
        }

        public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            if (!context.InteractiveRequest.TryGetAdditionalParameter(AppServiceAuthenticationDefaults.InteractiveRequestProviderKeyName, out string selectedProvider))
            {
                throw new InvalidOperationException("The authentication provider is not provided for the current structure.");
            }

            if (!Enum.TryParse(selectedProvider, ignoreCase: true, result: out AppServiceEasyAuthProviderType provider))
            {
                throw new InvalidOperationException("The authentication provider is invalid for the current structure.");
            }

            var stateId = Guid.NewGuid().ToString();
            await jsRuntime.InvokeVoidAsync($"{providerOptions.BrowserStorageType}.setItem", $"{providerOptions.StorageKeyPrefix}.{stateId}", JsonSerializer.Serialize(context.State));
            navigation.NavigateTo($"/.auth/login/{selectedProvider}?post_login_redirect_uri={BuildRedirectUri(authenticationPaths.LogInCallbackPath)}/{stateId}", forceLoad: true);

            return new RemoteAuthenticationResult<RemoteAuthenticationState>
            {
                Status = RemoteAuthenticationStatus.Redirect
            };
        }

        public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            navigation.NavigateTo($"/.auth/logout?post_logout_redirect_uri={BuildRedirectUri(authenticationPaths.LogOutCallbackPath)}", forceLoad: true);

            return new RemoteAuthenticationResult<RemoteAuthenticationState>
            {
                Status = RemoteAuthenticationStatus.Redirect
            };
        }

        private string BuildRedirectUri(string path)
        {
            return new Uri(new Uri(navigation.BaseUri), path).ToString();
        }
    }
}
