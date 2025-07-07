using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.Games;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateGame;

[DiscordSlashCommand("game", "Create a game world and characters")]
[DiscordSlashCommandOption("game", "Which Game?", true, AutoChoices = DiscordSlashCommandChoicesMethod.Games)]
[DiscordSlashCommandOption("players", "How many players?", true, Type = DiscordSlashCommandOptionType.Integer)]
[DiscordSlashCommandOption("ai", "Use LLM models?", false, Type = DiscordSlashCommandOptionType.Boolean, Choices = new[] { "true", "false" })]
public partial class CreateGameProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";
        bool ai = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "ai").FirstOrDefault()?.Value ?? "false");
        int players = int.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "players").FirstOrDefault()?.Value ?? "2");

        var embeds = new List<DiscordEmbed>();
        DiscordFollowupMessageRequest? followupMessage = null;


        var matchingGames = games.Where(x => x.Name.ToLowerInvariant() == game.ToLowerInvariant()).ToList();
        if (matchingGames.Count > 0)
        {
            var selectedGame = matchingGames.First();
            var fields = new List<DiscordEmbedField>();

            if (selectedGame.WorldSection != null)
            {
                selectedGame.WorldSection.AddFields(fields, random);

                embeds.Add(new DiscordEmbed
                {
                    Title = $"World Creation for {selectedGame.Name}",
                    Description = $"World character for {game}...",
                    Fields = fields,
                });
            }

            var playerFields = new Dictionary<int, List<DiscordEmbedField>>();

            for (int player = 0; player < players; player++)
            {
                fields = new List<DiscordEmbedField>();
                playerFields[player] = fields;
                embeds.Add(new DiscordEmbed
                {
                    Title = $"Player #{player + 1}",
                    Color = 0xFF00FF,
                    Fields = fields,
                });
                selectedGame.CharacterSection!.AddFields(fields, random);
            }

            if (selectedGame.CharacterSection!.RelationshipSelections.Count > 0)
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
                        playerFields[p1]!.Add(new DiscordEmbedField
                        {
                            Name = relationshipSelection.Name,
                            Value = relationshipString.ToString(),
                        });
                    }
                }
            }


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

        return Task.FromResult(
            new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponse
                {
                    Data = new DiscordInteractionResponseData
                    {
                        Embeds = embeds,
                        Content = $"Generating character for {game}...",
                    }
                },
                FollowupMessage = followupMessage
            }
        );
    }
}
