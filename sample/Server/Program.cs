using Laurentiu.Azure.AppService.EasyAuth.Web;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register EasyAuth services to container
builder.Services.AddAppServiceEasyAuth(options =>
{
    // Use this when running locally
    options.UseDevIdentity = builder.Environment.IsDevelopment();

    /*
    // Use this in production code
    options.WhitelistedNameClaimValues = new() { "Laurentiu Stamate" };
    options.UseDevIdentity = builder.Environment.IsDevelopment();
    */
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    // Map local development endpoints
    app.MapEasyAuthDevEndpoints(useEmptyAuthMeJson: false);
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// Add Authorization middleware
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
