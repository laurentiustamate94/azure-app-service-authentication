using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;

namespace Laurentiu.Azure.AppService.EasyAuth.Sample.Client.Pages.EasyAuth
{
    public partial class Login
    {
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (AuthenticationState == null)
            {
                return;
            }

            var authState = await AuthenticationState;
            var user = authState?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                NavigationManager.NavigateTo("/");
            }
        }

        public void BeginLogIn(AppServiceEasyAuthProviderType provider)
        {
            var options = !string.IsNullOrEmpty(NavigationManager.HistoryEntryState)
                ? JsonSerializer.Deserialize<InteractiveRequestOptions>(NavigationManager.HistoryEntryState)
                : new InteractiveRequestOptions()
                {
                    Interaction = InteractionType.SignIn,
                    ReturnUrl = NavigationManager.BaseUri,
                };

            options.TryAddAdditionalParameter(AppServiceAuthenticationDefaults.InteractiveRequestProviderKeyName, provider.ToString());

            NavigationManager.NavigateToLogin("authentication/login", options);
        }
    }
}
