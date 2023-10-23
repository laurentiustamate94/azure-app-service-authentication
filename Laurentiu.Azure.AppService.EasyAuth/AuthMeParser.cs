using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laurentiu.Azure.AppService.EasyAuth
{
    public static class AuthMeParser
    {
        private class AuthMeModel
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("provider_name")]
            public string ProviderName { get; set; }

            [JsonPropertyName("user_id")]
            public string UserId { get; set; }

            [JsonPropertyName("user_claims")]
            public AuthMeUserClaimModel[] UserClaims { get; set; }
        }

        private class AuthMeUserClaimModel
        {
            [JsonPropertyName("typ")]
            public string Type { get; set; }

            [JsonPropertyName("val")]
            public string Value { get; set; }
        }

        public static readonly ClaimsIdentity DevIdentity = Parse(AppServiceAuthenticationDefaults.DevAuthMeJson);

        public static ClaimsIdentity Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var providers = JsonSerializer.Deserialize<List<AuthMeModel>>(data);
            if (providers == null || providers.Count == 0)
            {
                return new ClaimsIdentity();
            }

            if (providers.Count > 1)
            {
                // TBD what we do here
            }

            var authMeModel = providers.First();

            if (!Enum.TryParse(authMeModel.ProviderName, ignoreCase: true, result: out AppServiceEasyAuthProviderType provider))
            {
                // TBD what we do here
            }

            var claims = GetClaims(authMeModel);
            return new ClaimsIdentity(claims, authMeModel.ProviderName, AppServiceAuthenticationDefaults.AuthMeNameClaimType, string.Empty);
        }

        private static List<Claim> GetClaims(AuthMeModel authMeModel)
        {
            var userClaims = new List<Claim>();

            foreach (var userClaim in authMeModel.UserClaims)
            {
                userClaims.Add(new Claim(userClaim.Type, userClaim.Value));
            }

            return userClaims;
        }
    }
}
