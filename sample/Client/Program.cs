using Laurentiu.Azure.AppService.EasyAuth.Sample.Client;
using Laurentiu.Azure.AppService.EasyAuth.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register EasyAuth services to container
builder.Services.AddAppServiceEasyAuth(options =>
{
    options.ProviderOptions.WhitelistedNameClaimValues = new() { "John Doe" };
    
    // Use this when running the Server project
    options.ProviderOptions.UseDevIdentity = false;

    /*
    // Use this when debugging only the Client project
    options.ProviderOptions.UseDevIdentity = builder.HostEnvironment.IsDevelopment();
    */
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
