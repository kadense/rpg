using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateWorld;

[DiscordButtonCommand("generate_description", "Describe a world using AI")]
public partial class DescribeWorldProcessor : IDiscordButtonCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();

    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public DiscordApiResponseContent ErrorResponse(string content, ILogger logger)
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
        logger.LogInformation("Loading game information from persistent storage");
        var instance = await client.ReadGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!
        );

        if (instance == null)
            return ErrorResponse("Cannot find LLM Prompt", logger);


        logger.LogInformation("Generating LLM Prompt");
        var followupMessage = new DiscordFollowupMessageRequest
        {
            Type = FollowupProcessorType.PublicAiPromptResponse,
            Content = instance.WorldLlmPrompt,
            Token = interaction.Token,
            ChannelId = interaction.ChannelId ?? interaction.Channel!.Id!,
            GuildId = interaction.GuildId ?? interaction.Guild!.Id!,
        };

        var matchingGames = new GamesFactory()
            .EndGames()
            .Where(x => x.Name!.ToLowerInvariant() == instance.GameName)
            .ToList();

        if (matchingGames.Count == 0)
            return ErrorResponse("Could not find a game with that name.", logger);

        var selectedGame = matchingGames.First();

        return CreateWorldProcessor.GenerateResponse(selectedGame, instance.WorldSetup!, logger, followupMessage);
    }
}
