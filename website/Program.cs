using System.Globalization;
using AspNet.Security.OAuth.Discord;
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
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().RequireAuthorization()
    .AddInteractiveServerRenderMode();

    
app.UseAuthentication();
app.UseAuthorization();

app.Run();
