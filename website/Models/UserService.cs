using Microsoft.AspNetCore.Components.Authorization;


public class UserModel
{
    public UserModel(string? userName)
    {
        UserName = userName;
    }

    public string? UserName { get; set; }

    public string? Avatar { get; set; }
}
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
            return new UserModel(user.Identity.Name)
            {
                Avatar = user.Claims.Where(c => c.Type == "urn:discord:avatar:url").Select(c => c.Value).FirstOrDefault(string.Empty)
            };
        }
        else
        {
            return null;
        }
    }
}