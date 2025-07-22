using System.Globalization;
using System.Net;
using AspNet.Security.OAuth.Discord;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using website.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme; 
})
.AddCookie()
.AddDiscord(opts =>
{
    opts.ClientId = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")!;
    opts.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")!;
    opts.CallbackPath = "/discord-callback";
    opts.SaveTokens = true;
    
    
    opts.CorrelationCookie.SameSite = SameSiteMode.Lax;
    opts.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

    opts.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
        string.Format(
            CultureInfo.InvariantCulture,
            "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
            user.GetString("id"),
            user.GetString("avatar"),
            user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));
    
    opts.Scope.Add("identify");
    opts.Scope.Add("email");
});
builder.Services.AddAuthorization();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.MapGet($"/static/{{gameId}}/{{*filePath}}", async (HttpContext context) =>
{
    var gameId = context.Request.RouteValues["gameId"]?.ToString();
    var filePath = context.Request.RouteValues["filePath"]?.ToString();

    var client = new BlobClient(
        Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING")!,
        "rpg",
        $"{gameId}/{filePath}"
    );

    await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

    var exists = await client.ExistsAsync(CancellationToken.None);
    if (!exists)
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;

    var properties = await client.GetPropertiesAsync();
    var contentType = properties.Value.ContentType;

    context.Response.ContentType = contentType;
    context.Response.StatusCode = 200;
    var fileContent = await client.DownloadToAsync(context.Response.Body);
});
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().RequireAuthorization()
    .AddInteractiveServerRenderMode();

    
app.UseAuthentication();
app.UseAuthorization();

app.Run();
