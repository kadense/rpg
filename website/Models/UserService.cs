using Kadense.RPG.DataAccess;
using Microsoft.AspNetCore.Components.Authorization;

public class UserService
{
    public async Task<UserModel?> GetUserModelAsync(AuthenticationStateProvider authStateProvider)
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        /*
        user.Claims.ToList().ForEach(c =>
        {
            Console.WriteLine($"Claim \"{c.Type}\" = \"{c.Value}\"");
        });
        */

        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            var client = new DataConnectionClient();
            var userModel = new UserModel(user.Identity.Name)
            {
                Avatar = user.Claims.Where(c => c.Type == "urn:discord:avatar:url").Select(c => c.Value).FirstOrDefault(string.Empty),
                Email = user.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Select(c => c.Value).FirstOrDefault(string.Empty),
                Identifier = user.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Select(c => c.Value).FirstOrDefault(string.Empty)
            };
            await client.WriteUserAsync(userModel);
            return userModel;
        }
        else
        {
            return null;
        }
    }
}