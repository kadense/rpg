using System.Text.RegularExpressions;
using Azure;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateCharacter;

[DiscordSlashCommand("character", "Create a random character")]
[DiscordSlashCommandOption("game", "Which Game?", true, AutoChoices = DiscordSlashCommandChoicesMethod.GamesWithWorlds)]
public partial class CreateCharacterProcessor : IDiscordSlashCommandProcessor
{

    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";

        var embeds = new List<DiscordEmbed>();


        var matchingGames = games.Where(x => x.Name.ToLowerInvariant() == game.ToLowerInvariant()).ToList();
        if (matchingGames.Count == 0)
        {
            return Task.FromResult(
                new DiscordApiResponseContent
                {
                    Response = new DiscordInteractionResponseBuilder()
                        .WithData()
                            .WithContent($"Unable to generate character for {game}... couldn't match to a game")
                        .End()
                        .Build()
                }
            );
        }
        var selectedGame = matchingGames.First();

        if (selectedGame.CharacterSection == null)
        {
            return Task.FromResult(
                new DiscordApiResponseContent
                {
                    Response = new DiscordInteractionResponseBuilder()
                        .WithData()
                            .WithContent($"Unable to generate character for {game}... no character section defined")
                        .End()
                        .Build()
                }
            );
        }
        
        return Task.FromResult(
            new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponseBuilder()
                    .WithData()
                        .WithContent($"Generated character for {game}...")
                        .WithEmbed()
                            .WithFields(selectedGame.CharacterSection!, random)
                        .End()
                    .End()
                    .Build()
            }
        );
    }
}
