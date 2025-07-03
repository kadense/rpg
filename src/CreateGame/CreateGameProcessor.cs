using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.Games;

namespace Kadense.RPG.CreateGame;

[DiscordSlashCommand("game", "Create a game world and characters")]
[DiscordSlashCommandOption("game", "Which Game?", true, Choices = new[] { "The witch is dead", "The golden sea", "We three kings", "Adventure Dice", "We that remain", "Honey heist", "Big gay Orcs", "Justified anxiety", "The rapid and the righteous", "Hack the planet", "Genius Loci", "Dead channel", "Trashkin" })]
[DiscordSlashCommandOption("players", "How many players?", true, Type = DiscordSlashCommandOptionType.Integer)]
public partial class CreateGameProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";
        int players = int.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "players").FirstOrDefault()?.Value ?? "2");

        var embeds = new List<DiscordEmbed>();

       
        var matchingGames = games.Where(x => x.Name.ToLowerInvariant() == game.ToLowerInvariant()).ToList();
        if (matchingGames.Count > 0)
        {
            var selectedGame = matchingGames.First();
            selectedGame.WorldSection.AddEmbeds(embeds, random);
            for (int player = 0; player < players; player++)
            {
                embeds.Add(new DiscordEmbed
                {
                    Title = $"Player #{player + 1}",
                    Color = 0xFF00FF
                });
                selectedGame.CharacterSection.AddEmbeds(embeds, random);
                
            }
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
