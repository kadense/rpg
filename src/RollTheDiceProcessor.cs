using System.Text.RegularExpressions;
using Kadense.Models.Discord;

namespace Kadense.RPG;

[DiscordSlashCommand("roll", "Roll some dice!")]
[DiscordSlashCommandOption("whattoroll", "The dice to roll, e.g. 1d6, 2d20+3, etc.", true)]
public class RollTheDiceProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction)
    {
        string whatToRoll = interaction.Data?.Options?.Where(opt => opt.Name == "whattoroll").FirstOrDefault()?.Value ?? "1d6";

        var match = Regex.Match(whatToRoll, @"^(?<count>\d*)d(?<sides>\d+)(?<mods>([+-]\d+)*)$", RegexOptions.IgnoreCase);
        int count = 1, sides = 6, modifier = 0;
        string op = "+";
        if (match.Success)
        {
            count = string.IsNullOrEmpty(match.Groups["count"].Value) ? 1 : int.Parse(match.Groups["count"].Value);
            sides = int.Parse(match.Groups["sides"].Value);

            var mods = match.Groups["mods"].Value;
            if (!string.IsNullOrEmpty(mods))
            {
                // Find all +n or -n in the mods string
                foreach (Match m in Regex.Matches(mods, @"([+-]\d+)"))
                {
                    modifier += int.Parse(m.Value);
                }
            }
        }
        if (modifier < 0)
        {
            op = "-";
        }

        var embeds = new List<DiscordEmbed>();
        embeds.Add(new DiscordEmbed
        {
            Title = "Roll Request",
            Description = $"Rolling {count}d{sides}{(modifier != 0 ? $" {op} {Math.Abs(modifier)}" : "")}",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        });

        var rolls = new List<int>();
        for (int i = 0; i < count; i++)
        {
            var roll = random.Next(1, sides + 1);
            rolls.Add(roll);
            embeds.Last().Fields!.Add(new DiscordEmbedField
            {
                Name = $"#{i + 1}",
                Value = roll.ToString(),
            });
        }
        if (modifier != 0)
        {
            embeds.Last().Fields!.Add(new DiscordEmbedField
            {
                Name = $"Modifier",
                Value = $"{(modifier > 0 ? "+" : "")}{modifier}",
            });
        }
        int total = rolls.Sum() + modifier;

        string rollDetails = string.Join(", ", rolls);
        string modifierText = modifier != 0 ? $" + {modifier}" : "";

        embeds.Last().Fields!.Add(new DiscordEmbedField
        {
            Name = $"Total",
            Value = total.ToString()
        });

        return Task.FromResult(new DiscordInteractionResponse
        {
            Data = new DiscordInteractionResponseData
            {
                Embeds = embeds,
                Content = $"You rolled: {total}"
            }
        });
    }
}
