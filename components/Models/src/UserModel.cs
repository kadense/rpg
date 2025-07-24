namespace Kadense.RPG.Models;

public class UserModel
{
    public UserModel(string? userName)
    {
        UserName = userName;
    }

    public string? UserName { get; set; }

    public string? Avatar { get; set; }

    public string? Email { get; set; }

    public string? Identifier { get; set; }

    public bool IsGlobalAdmin()
    {
        var globalAdmins = Environment.GetEnvironmentVariable("ADMIN_USERS") ?? "1130524771059249203";
        return globalAdmins.Split(",").Contains(Identifier);
    }
}