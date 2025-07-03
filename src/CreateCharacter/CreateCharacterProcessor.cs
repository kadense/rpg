using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.Games;

namespace Kadense.RPG.CreateCharacter;

[DiscordSlashCommand("character", "Create a random character")]
[DiscordSlashCommandOption("game", "Which Game?", true, Choices = new[] { "The witch is dead", "The golden sea", "We three kings", "Adventure Dice", "We that remain", "Honey heist", "Big gay Orcs", "Justified anxiety", "The rapid and the righteous", "Hack the planet", "Genius Loci", "Dead channel", "Trashkin" })]
public partial class CreateCharacterProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";

        var embeds = new List<DiscordEmbed>();

       
        var matchingGames = games.Where(x => x.Name.ToLowerInvariant() == game.ToLowerInvariant()).ToList();
        if (matchingGames.Count > 0)
        {
            var selectedGame = matchingGames.First();
            var fields = new List<DiscordEmbedField>();
            selectedGame.CharacterSection!.AddFields(fields, random);

            embeds.Add(new DiscordEmbed
            {
                Title = $"Character Creation for {selectedGame.Name}",
                Description = $"Generating character for {game}...",
                Fields = fields,
            });
        }

        return Task.FromResult(new DiscordInteractionResponse
        {
            Data = new DiscordInteractionResponseData
            {
                Embeds = embeds,
                Content = $"Generating character for {game}...",
            }
        });
    }
}
