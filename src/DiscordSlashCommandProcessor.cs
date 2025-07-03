using Kadense.Models.Discord;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG;

public enum DiscordSlashCommandOptionType
{
    String = 3,
    Integer = 4,
    Boolean = 5,
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class DiscordSlashCommandOptionAttribute : Attribute
{
    public string Name { get; }

    public string Description { get; set; }

    public bool Required { get; set; }

    public DiscordSlashCommandOptionType Type { get; set; } = DiscordSlashCommandOptionType.String;

    public string[] Choices { get; set; } = Array.Empty<string>();

    
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
    public Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction, ILogger logger);
}
