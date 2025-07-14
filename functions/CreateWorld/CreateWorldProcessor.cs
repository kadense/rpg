using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateWorld;

[DiscordSlashCommand("world", "Create a world")]
[DiscordSlashCommandOption("game", "Which Game?", true, AutoChoices = DiscordSlashCommandChoicesMethod.GamesWithWorlds)]
[DiscordSlashCommandOption("ai", "Use LLM models?", false, Type = DiscordSlashCommandOptionType.Boolean, Choices = new[] { "true", "false" })]
public partial class CreateWorldProcessor : IDiscordSlashCommandProcessor
{

    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";
        bool ai = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "ai").FirstOrDefault()?.Value ?? "false");

        var embeds = new List<DiscordEmbed>();

        DiscordFollowupMessageRequest? followupMessage = null;

        var matchingGames = games.Where(x => x.Name.ToLowerInvariant() == game.ToLowerInvariant()).ToList();


        if (matchingGames.Count == 0)
            return Task.FromResult(
                new DiscordApiResponseContent
                {
                    Response = new DiscordInteractionResponseBuilder()
                        .WithData()
                            .WithEmbed()
                                .WithTitle("World Creation")
                                .WithDescription("Could not find a game with that name.")
                                .WithColor(0xFF0000) // Red color
                            .End()
                        .End()
                        .Build()
                }
            );

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


        if (ai && selectedGame.WorldSection!.LlmPrompt != null)
        {
            var llmPrompt = new StringBuilder(selectedGame.WorldSection.GetLlmPrompt()!);
            selectedGame.WorldSection.Selections
                    .Where(s => s.VariableName != null)
                    .ToList().ForEach(s =>
                    {
                        if (s.ChosenValues.Count > 0)
                        {
                            llmPrompt.Replace($"{{{s.VariableName}}}", string.Join(", ", s.ChosenValues.First().Select(v => v.Name)));
                        }
                    });

            logger.LogInformation("LLM Prompt: {Prompt}", llmPrompt);

            followupMessage = new DiscordFollowupMessageRequest
            {
                Type = FollowupProcessorType.PublicAiPromptResponse,
                Content = llmPrompt.ToString(),
                Token = interaction.Token,
                ChannelId = interaction.ChannelId ?? interaction.Channel!.Id!,
                GuildId = interaction.GuildId ?? interaction.Guild!.Id!,
            };
        }


        return Task.FromResult(
            new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponseBuilder()
                    .WithData()
                        .WithEmbed()
                            .WithTitle($"The world of {game}")
                            .WithFields(selectedGame.WorldSection, random)
                            .WithColor(0x00FF00) // Green color
                        .End()
                    .End()
                    .Build(),
                FollowupMessage = followupMessage
            }
        );
    }
}
