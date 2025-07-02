using System.Text.RegularExpressions;
using Kadense.Models.Discord;

namespace Kadense.RPG.CreateWorld;

[DiscordSlashCommand("world", "Create a world")]
[DiscordSlashCommandOption("game", "Which Game?", true, Choices = new[] { "The witch is dead", "The golden sea", "We three kings", "Adventure Dice", "We that remain", "Honey heist", "Big gay Orcs", "Justified anxiety", "The rapid and the righteous", "Hack the planet", "Genius Loci", "Dead channel", "Trashkin" })]
public partial class CreateWorldProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction)
    {
        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";

        var embeds = new List<DiscordEmbed>();

        switch (game.ToLowerInvariant())
        {
            case "the witch is dead":
                embeds = CreateTheWitchIsDeadWorld(interaction);
                break;

            case "the golden sea":
                //embeds = CreateTheGoldenSeaCharacter(interaction);
                break;

            case "we three kings":
                //embeds = CreateWeThreeKingsCharacter(interaction);
                break;

            case "adventure dice":
                //embeds = CreateAdventureDiceCharacter(interaction);
                break;

            case "we that remain":
                //embeds = CreateWeThatRemainCharacter(interaction);
                break;

            case "honey heist":
                //embeds = CreateHoneyHeistCharacter(interaction);
                break;
        }

        return Task.FromResult(new DiscordInteractionResponse
                {
                    Data = new DiscordInteractionResponseData
                    {
                        Embeds = embeds,
                        Content = $"Generating world for {game}...",
                    }
                });
    }
}
