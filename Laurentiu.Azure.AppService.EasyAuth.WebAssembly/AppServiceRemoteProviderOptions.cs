namespace Laurentiu.Azure.AppService.EasyAuth.WebAssembly
{
    public class AppServiceRemoteProviderOptions
    {
        public string BrowserStorageType { get; set; } = "sessionStorage";

        public string StorageKeyPrefix { get; set; } = "Blazor.EasyAuth";

        public bool UseDevIdentity { get; set; } = false;

        public List<string> WhitelistedNameClaimValues { get; set; } = new();
    }
}
