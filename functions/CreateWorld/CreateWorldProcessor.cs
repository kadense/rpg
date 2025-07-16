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
    private static readonly DataConnectionClient client = new DataConnectionClient();
    private static readonly KadenseRandomizer random = new KadenseRandomizer();

    public static async Task<DiscordApiResponseContent> GenerateResponseAsync(DiscordInteraction interaction, Game selectedGame, ILogger logger, DiscordFollowupMessageRequest? followupMessage = null)
    {
        var instance = new GameInstance()
        {
            GameName = selectedGame.Name
        };


        var content = new StringBuilder();
        selectedGame.WorldSection!.WithFields(content, random);

        if (selectedGame.WorldSection!.LlmPrompt != null)
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
            instance.WorldLlmPrompt = llmPrompt.ToString();
        }

        instance.WorldSetup = content.ToString();


        logger.LogInformation("Writing back to persistent storage");
        await client.WriteGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!,
            instance
        );

        return GenerateResponse(selectedGame, content.ToString(), logger);
    }

    public static DiscordApiResponseContent GenerateResponse(Game selectedGame, string content, ILogger logger, DiscordFollowupMessageRequest? followupMessage = null, string? story = null)
    {
        logger.LogInformation("Generating Response");

        var container = new DiscordInteractionResponseBuilder()
                    .WithResponseType(DiscordInteractionResponseType.UPDATE_MESSAGE)
                    .WithData()
                        .WithFlags(1 << 15)
                        .WithContainerComponent()
                            .WithTextDisplayComponent()
                                .WithContent($"### {selectedGame.Name} World Creation")
                            .End()
                            .WithTextDisplayComponent()
                                .WithContent(content)
                            .End();

        if(story != null)
            container
                            .WithTextDisplayComponent()
                                .WithContent(story)
                            .End();

        var response = container
                            .WithActionRowComponent()
                                .WithButtonComponent()
                                    .WithLabel("Regenerate World")
                                    .WithCustomId("regenerate_world")
                                    .WithEmoji(new DiscordEmoji { Name = "🌍" })
                                .End()
                                .WithButtonComponent()
                                    .WithLabel("Create Description")
                                    .WithCustomId("generate_description")
                                    .WithEmoji(new DiscordEmoji { Name = "💭" })
                                .End()
                            .End()
                        .End()
                    .End()
                    .Build();
        return
            new DiscordApiResponseContent
            {
                Response = response,
                FollowupMessage = followupMessage
            };
    }

    
    public static DiscordApiResponseContent ErrorResponse(string content, ILogger logger)
    {
        logger.LogError(content);
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

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";

        var embeds = new List<DiscordEmbed>();

        var matchingGames = games.Where(x => x.Name!.ToLowerInvariant() == game.ToLowerInvariant()).ToList();

        if (matchingGames.Count == 0)
            return ErrorResponse("Could not find a game with that name.", logger);

        var selectedGame = matchingGames.First();

        if (selectedGame.WorldSection == null)
            return ErrorResponse("This game does not support world creation.", logger);

        return await CreateWorldProcessor.GenerateResponseAsync(interaction, selectedGame, logger);        
    }
}
