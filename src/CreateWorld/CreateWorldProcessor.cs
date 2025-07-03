using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.Games;

namespace Kadense.RPG.CreateWorld;

[DiscordSlashCommand("world", "Create a world")]
[DiscordSlashCommandOption("game", "Which Game?", true, Choices = new[] { "The witch is dead", "We that remain", "Honey heist", "Big gay Orcs", "Justified anxiety", "The rapid and the righteous", "Hack the planet", "Genius Loci", "Dead channel", "Trashkin" })]
public partial class CreateWorldProcessor : IDiscordSlashCommandProcessor
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
                Title = $"World Creation for {selectedGame.Name}",
                Description = $"Generating World for {game}...",
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
