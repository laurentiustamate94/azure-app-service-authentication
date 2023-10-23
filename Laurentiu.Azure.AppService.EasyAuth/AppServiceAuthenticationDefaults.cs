namespace Laurentiu.Azure.AppService.EasyAuth
{
    public static class AppServiceAuthenticationDefaults
    {
        public const string AuthenticationScheme = "AppServiceAuthentication";

        public const string AuthMeEndpoint = "/.auth/me";

        public const string AuthLoginEndpointPattern = "/.auth/login/{provider}";

        public const string AuthLogoutEndpointPattern = "/.auth/logout";

        public const string DevAuthMeJson = "[{\"provider_name\":\"none\",\"user_claims\":[{\"typ\":\"http:\\/\\/schemas.xmlsoap.org\\/ws\\/2005\\/05\\/identity\\/claims\\/name\",\"val\":\"John Doe\"}]}]";

        public const string EmptyAuthMeJson = "[]";

        public const string AuthMeNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

        public const string InteractiveRequestProviderKeyName = "provider";
    }
}
