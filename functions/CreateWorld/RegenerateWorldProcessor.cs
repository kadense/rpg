using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateWorld;

[DiscordButtonCommand("regenerate_world", "Create a world")]
public partial class RegenerateWorldProcessor : IDiscordButtonCommandProcessor
{

    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public DiscordApiResponseContent ErrorResponse(string content)
    {
        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponseBuilder()
                .WithData()
                    .WithEmbed()
                        .WithTitle("World Creation")
                        .WithDescription(content)
                        .WithColor(0xFF0000) // Red color
                    .End()
                .End()
                .Build()
        };
    }

    public Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();


        if (interaction.Message == null || interaction.Message.Components == null || interaction.Message.Components.Count == 0)
            return Task.FromResult(ErrorResponse("Unable to identify game name"));

        
        var containerComponent = (DiscordContainerComponent)interaction.Message.Components.First();
        if (containerComponent == null || containerComponent.Components == null || containerComponent.Components.Count == 0)
            return Task.FromResult(ErrorResponse("Unable to identify game name"));

        var textDisplayComponent = (DiscordTextDisplayComponent)containerComponent.Components.First();
        if (textDisplayComponent == null || textDisplayComponent.Content == null)
            return Task.FromResult(ErrorResponse("Unable to identify game name"));

        var match = Regex.Match(textDisplayComponent.Content, "### (?<game>.*) World Creation");
        if (match == null)
            return Task.FromResult(ErrorResponse("Unable to identify game name"));


        var game = match!.Groups["game"].Value;
        var matchingGames = games.Where(x => x.Name!.ToLowerInvariant() == game.ToLowerInvariant()).ToList();

        if (matchingGames.Count == 0)
            return Task.FromResult(ErrorResponse($"Unable to identify game called {game}"));

        var selectedGame = matchingGames.First();

        if (selectedGame.WorldSection == null)
            return Task.FromResult(
                new DiscordApiResponseContent
                {
                    Response = new DiscordInteractionResponseBuilder()
                        .WithData()
                            .WithEmbed()
                                .WithTitle("World Creation")
                                .WithDescription("This game does not support world creation.")
                                .WithColor(0xFF0000) // Red color
                            .End()
                        .End()
                        .Build()
                }
            );


        var content = new StringBuilder();
        selectedGame.WorldSection!.WithFields(content, random);

        return Task.FromResult(
            new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponseBuilder()
                    .WithData()
                        .WithFlags(1 << 15)
                        .WithContainerComponent()
                            .WithTextDisplayComponent()
                                .WithContent($"### {game} World Creation")
                            .End()
                            .WithTextDisplayComponent()
                                .WithContent(content.ToString())
                            .End()
                            .WithActionRowComponent()
                                .WithButtonComponent()
                                    .WithLabel("Regenerate World")
                                    .WithCustomId("regenerate_world")
                                    .WithEmoji(new DiscordEmoji { Name = "🌍" })
                                .End()
                                .WithButtonComponent()
                                    .WithLabel("Create Description")
                                    .WithCustomId("generate_description")
                                .End()
                            .End()
                        .End()
                    .End()
                    .Build(),
            }
        );
    }
}
