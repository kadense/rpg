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
using Kadense.RPG.DataAccess;
using website.Components;
using System.Data.Common;
using Microsoft.FluentUI.AspNetCore.Components;


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
builder.Services.AddScoped<UserService>();
builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents(opts =>
{
    opts.ValidateClassNames = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.MapGet($"/api/v1/game/{{gameId}}", async (HttpContext context) =>
{
    var gameId = context.Request.RouteValues["gameId"]?.ToString();
    var game = new GamesFactory().EndGames().Where(g => g.Id == gameId).First();
    var client = new DataConnectionClient();
    await client.WriteGameAsync(game);
    foreach (var location in game.Locations)
    {
        await client.WriteGameLocationAsync(game.Id, location);
    }
    foreach (var character in game.Characters)
    {
        await client.WriteGameCharacterAsync(game.Id, character);
    }
    foreach (var item in game.Items)
    {
        await client.WriteGameItemAsync(game.Id, item);
    }
});
app.MapGet($"/assets/{{gameId}}/{{*filePath}}", async (HttpContext context) =>
{
    var gameId = context.Request.RouteValues["gameId"]?.ToString();
    var filePath = context.Request.RouteValues["filePath"]?.ToString();
    var client = new DataConnectionClient();
    var contentType = await client.ReadGameAssetContentTypeAsync(gameId!, filePath!);
    if (contentType == null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        return;
    }

    context.Response.ContentType = contentType;
    context.Response.StatusCode = 200;
    await client.ReadGameAssetToStreamAsync(gameId!, filePath!, context.Response.Body);
});
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().RequireAuthorization()
    .AddInteractiveServerRenderMode();

    
app.UseAuthentication();
app.UseAuthorization();

app.Run();
