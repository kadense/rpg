using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.Games;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateWorld;

[DiscordSlashCommand("world", "Create a world")]
[DiscordSlashCommandOption("game", "Which Game?", true, AutoChoices = DiscordSlashCommandChoicesMethod.GamesWithWorlds )]
[DiscordSlashCommandOption("ai", "Use LLM models?", false, Type = DiscordSlashCommandOptionType.Boolean, Choices = new[] { "true", "false" })]
public partial class CreateWorldProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";
        bool ai = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "ai").FirstOrDefault()?.Value ?? "false");

        var embeds = new List<DiscordEmbed>();

        DiscordFollowupMessageRequest? followupMessage = null;
        
        var matchingGames = games.Where(x => x.Name.ToLowerInvariant() == game.ToLowerInvariant()).ToList();
        if (matchingGames.Count > 0)
        {
            var selectedGame = matchingGames.First();
            var fields = new List<DiscordEmbedField>();
            selectedGame.WorldSection!.AddFields(fields, random);

            embeds.Add(new DiscordEmbed
            {
                Title = $"World Creation for {selectedGame.Name}",
                Description = $"Generating World for {game}...",
                Fields = fields,
            });

            if (ai && selectedGame.WorldSection!.LlmPrompt != null)
            {
                var llmPrompt = selectedGame.WorldSection.GetLlmPrompt();

                logger.LogInformation("LLM Prompt: {Prompt}", llmPrompt);

                followupMessage = new DiscordFollowupMessageRequest
                {
                    Type = FollowupProcessorType.PublicAiPromptResponse,
                    Content = llmPrompt,
                    Token = interaction.Token,
                    ChannelId = interaction.ChannelId ?? interaction.Channel!.Id!,
                    GuildId = interaction.GuildId ?? interaction.Guild!.Id!,
                };
            }
        }
        else
        {
            embeds.Add(new DiscordEmbed
            {
                Title = "World Creation",
                Description = "This game does not support world creation.",
                Color = 0xFF0000, // Red color
            });
        }

        return Task.FromResult(
            new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponse
                {
                    Data = new DiscordInteractionResponseData
                    {
                        Embeds = embeds,
                        Content = $"Generating world for {game}...",
                    }
                },
                FollowupMessage = followupMessage
            }
        );
    }
}
