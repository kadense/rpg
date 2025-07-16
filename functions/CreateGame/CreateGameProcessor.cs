using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateGame;

[DiscordSlashCommand("game", "Create a game world and characters")]
[DiscordSlashCommandOption("game", "Which Game?", true, AutoChoices = DiscordSlashCommandChoicesMethod.Games)]
[DiscordSlashCommandOption("players", "How many players?", true, Type = DiscordSlashCommandOptionType.Integer)]
[DiscordSlashCommandOption("ai", "Use LLM models?", false, Type = DiscordSlashCommandOptionType.Boolean, Choices = new[] { "true", "false" })]
public partial class CreateGameProcessor : IDiscordSlashCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();

    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = (interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6").ToString()!;
        bool ai = bool.Parse((interaction.Data?.Options?.Where(opt => opt.Name == "ai").FirstOrDefault()?.Value ?? "false").ToString()!);
        int players = int.Parse((interaction.Data?.Options?.Where(opt => opt.Name == "players").FirstOrDefault()?.Value ?? "2").ToString()!);

        DiscordFollowupMessageRequest? followupMessage = null;

        var matchingGames = games.Where(x => x.Name!.ToLowerInvariant() == game.ToLowerInvariant()).ToList();
        if (matchingGames.Count == 0)
        {
            return new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponseBuilder()
                    .WithData()
                        .WithContent($"Unable to generate character for {game}... couldn't match to a game")
                    .End()
                    .Build()
            };
        }

        var selectedGame = matchingGames.First();
        var responseData = new DiscordInteractionResponseBuilder()
            .WithData()
                .WithContent($"Generated Game of {game} for {players} players")
                .WithEmbed()
                    .WithTitle($"The world of {game}")
                    .WithDescription(selectedGame.WorldSection == null ? "No world section defined" : "World details follow:")
                    .WithFields(selectedGame.WorldSection, random)
                .End();
        
        var playerFields = new Dictionary<int, DiscordEmbedBuilder>();

        if (selectedGame.CharacterSection != null)
        {

            for (int player = 0; player < players; player++)
            {
                playerFields[player] = responseData
                    .WithEmbed()
                        .WithTitle($"Player #{player + 1}")
                        .WithColor(0xFF00FF)
                        .WithFields(selectedGame.CharacterSection, random);                
            }

            if (selectedGame.CharacterSection.RelationshipSelections.Count > 0)
            {
                foreach (var relationshipSelection in selectedGame.CharacterSection.RelationshipSelections)
                {
                    for (int p1 = 0; p1 < players; p1++)
                    {
                        var relationshipString = new StringBuilder();
                        for (int p2 = 0; p2 < players; p2++)
                        {
                            if (p1 == p2) continue; // Skip self-relationships
                            foreach (var chosen in relationshipSelection.Choose(random))
                            {
                                relationshipString.Append($"{chosen.Name} Player #{p2 + 1}");
                                if (chosen.Description != null)
                                {
                                    relationshipString.Append($" - {chosen.Description}");
                                }
                                relationshipString.AppendLine();
                            }
                        }
                        playerFields[p1]!
                            .WithField()
                                .WithName(relationshipSelection.Name!)
                                .WithValue(relationshipString.ToString());
                    }
                }
            }
        }

        if (selectedGame.CharacterSection != null)
        {

            if (ai && selectedGame.WorldSection!.LlmPrompt != null)
            {
                var llmPrompt = new StringBuilder(selectedGame.WorldSection.LlmPrompt);
                selectedGame.WorldSection.Selections
                    .Where(s => s.VariableName != null)
                    .ToList().ForEach(s =>
                    {
                        if (s.ChosenValues.Count > 0)
                        {
                            llmPrompt.Replace($"{{{s.VariableName}}}", string.Join(", ", s.ChosenValues.First().Select(v => v.Name)));
                        }
                    });

                logger.LogInformation("LLM Prompt: {Prompt}", llmPrompt.ToString());

                followupMessage = new DiscordFollowupMessageRequest
                {
                    Type = FollowupProcessorType.PublicAiPromptResponse,
                    Content = llmPrompt.ToString(),
                    Token = interaction.Token,
                    ChannelId = interaction.ChannelId ?? interaction.Channel!.Id!,
                    GuildId = interaction.GuildId ?? interaction.Guild!.Id!,
                };
            }
        }

        var instance = new GameInstance()
        {
            GameName = game
        };

        await client.WriteGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!,
            instance
        );

        return new DiscordApiResponseContent
        {
            Response = responseData
                .End()
                .Build(),
            FollowupMessage = followupMessage
        };
    }
}
