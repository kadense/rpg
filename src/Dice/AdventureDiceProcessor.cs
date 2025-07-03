using System.Text.RegularExpressions;
using Kadense.Models.Discord;

namespace Kadense.RPG.Dice;

[DiscordSlashCommand("adventure-dice", "Roll some adventure dice!")]
[DiscordSlashCommandOption("skill", "roll the skill dice", false, Type = DiscordSlashCommandOptionType.Boolean)]
[DiscordSlashCommandOption("danger", "roll the danger dice", false, Type = DiscordSlashCommandOptionType.Boolean)]
public class AdventureDiceProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();


    public string DiceValueSkill(int value) => value switch
    {
        1 => ":white_check_mark: **You do the thing**",
        2 => ":white_square_button: **Empty**",
        3 => ":white_square_button: **Empty**",
        4 => ":white_square_button: **Empty**",
        5 => ":white_check_mark: **You do the thing**",
        6 => ":white_square_button: **Empty**",
        _ => throw new ArgumentException($"Unknown roll value: {value}"),
    };
    
    public string DiceValueDanger(int value) => value switch
    {
        1 => ":white_square_button:",
        2 => ":white_check_mark:",
        3 => ":white_square_button:",
        4 => ":skull:",
        5 => ":white_square_button:",
        6 => ":skull:",
        _ => throw new ArgumentException($"Unknown roll value: {value}"),
    };
    
    
    public string DiceValueBasic1(int value) => value switch
    {
        1 => ":money_bag:",
        2 => ":frowning2:",
        3 => ":slight_smile:",
        4 => ":skull:",
        5 => ":negative_squared_cross_mark:",
        6 => ":white_check_mark:",
        _ => throw new ArgumentException($"Unknown roll value: {value}"),
    };
    
    
    public string DiceValueBasic2(int value) => value switch
    {
        1 => ":negative_squared_cross_mark:",
        2 => ":negative_squared_cross_mark:",
        3 => ":negative_squared_cross_mark:",
        4 => ":white_check_mark:",
        5 => ":white_check_mark:",
        6 => ":white_check_mark:",
        _ => throw new ArgumentException($"Unknown roll value: {value}"),
    };

    public string InterpretIndividualDice(string whatIsRolled) => whatIsRolled switch
    {
        ":white_check_mark:" => "You do the thing",
        ":negative_squared_cross_mark:" => "You fail to do the thing",
        ":skull:" => "You take damage",
        ":slight_smile:" => "You find a piece of information that is useful",
        ":frowning2:" => "You find a piece of information that makes things worse or more difficult",
        ":money_bag:" => "Treasure: you find a useful item",
        ":white_square_button:" => "Makes no difference",
        _ => throw new ArgumentException($"Unknown roll type: {whatIsRolled}"),
    };

    public string MakeRoll(DiscordEmbed embed, Random random, bool withSkill, bool withDanger)
    {

        List<string> rolls = [DiceValueBasic1(random.Next(1, 6)), DiceValueBasic2(random.Next(1, 6))];

        if (withSkill)
            rolls.Add(DiceValueSkill(random.Next(1, 6)));


        if (withDanger)
            rolls.Add(DiceValueDanger(random.Next(1, 6)));

        bool foundTreasure = rolls.Contains(":money_bag:");
        bool foundGoodInfo = rolls.Contains(":slight_smile:");
        bool foundBadInfo = rolls.Contains(":frowning2:");
        int successes = rolls.Count(r => r == ":white_check_mark:");
        int failures = rolls.Count(r => r == ":negative_squared_cross_mark:");
        int damage = rolls.Count(r => r == ":skull:");

        string result = successes > 0 && failures > 0 ? "Yes, but with complications" :
                     successes > 0 ? "Yes" :
                     failures > 0 ? "No" : "No";

        embed.Fields!.AddRange(
            rolls.Select(r => new DiscordEmbedField
            {
                Name = r,
                Value = InterpretIndividualDice(r),
            }).ToArray()
        );

        embed.Fields.Add(new DiscordEmbedField
        {
            Name = "Result",
            Value = result,
        });

        embed.Fields.Add(new DiscordEmbedField
        {
            Name = "Total Damage",
            Value = damage.ToString(),
        });

        return $"Roll completed";
    }

    public Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction)
    {
        bool skill = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "skill").FirstOrDefault()?.Value ?? "false");
        bool danger = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "danger").FirstOrDefault()?.Value ?? "false");

        var embed = new DiscordEmbed
        {
            Title = "Adventure Dice Roll",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        };

        var content = MakeRoll(embed, random, skill, danger);            

        return Task.FromResult(new DiscordInteractionResponse
        {
            Data = new DiscordInteractionResponseData
            {
                Content = content,
                Embeds = new List<DiscordEmbed>() { embed },
            }
        });
    }
}
