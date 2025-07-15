using Kadense.Models.Discord;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class DiscordButtonCommandAttribute : Attribute
{
    public string Name { get; }

    public string Description { get; set; }

    public DiscordButtonCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
