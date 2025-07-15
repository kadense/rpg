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
    private readonly DataConnectionClient client = new DataConnectionClient();
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";

        var embeds = new List<DiscordEmbed>();

        DiscordFollowupMessageRequest? followupMessage = null;

        var matchingGames = games.Where(x => x.Name!.ToLowerInvariant() == game.ToLowerInvariant()).ToList();

        if (matchingGames.Count == 0)
            return
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
                };

        var selectedGame = matchingGames.First();

        if (selectedGame.WorldSection == null)
            return
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
                };



        var instance = await client.ReadGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!
        );

        if (instance == null)
            instance = new GameInstance()
            {
                GameName = game
            };
        
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
        
        var content = new StringBuilder();
        selectedGame.WorldSection!.WithFields(content, random);
        instance.WorldSetup = content.ToString();
        
        await client.WriteGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!,
            instance
        );

        return
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
                                    .WithEmoji(new DiscordEmoji { Name = "💭" })
                                .End()
                            .End()
                        .End()
                    .End()
                    .Build(),
                FollowupMessage = followupMessage
            };
    }
}
