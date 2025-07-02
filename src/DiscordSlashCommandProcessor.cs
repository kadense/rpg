using Kadense.Models.Discord;

namespace Kadense.RPG;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class DiscordSlashCommandOptionAttribute : Attribute
{
    public string Name { get; }

    public string Description { get; set; }

    public bool Required { get; set; }
    public DiscordSlashCommandOptionAttribute(string name, string description, bool required = false)
    {
        Name = name;
        Description = description;
        Required = required;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class DiscordSlashCommandAttribute : Attribute
{
    public string Name { get; }

    public string Description { get; set; }

    public DiscordSlashCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

public interface IDiscordSlashCommandProcessor
{
    public Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction);
}
