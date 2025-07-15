using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateWorld;

[DiscordButtonCommand("regenerate_world", "Create a world")]
public partial class RegenerateWorldProcessor : IDiscordButtonCommandProcessor
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
        var games = new GamesFactory()
            .EndGames();


        logger.LogInformation($"Getting Game Information");
        var instance = await client.ReadGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!
        );

        if (instance == null || string.IsNullOrEmpty(instance.GameName))
            return ErrorResponse("Could not get the instance name", logger);            

        var matchingGames = games.Where(x => x.Name!.ToLowerInvariant() == instance.GameName).ToList();

        if (matchingGames.Count == 0)
            return ErrorResponse("Could not find a game with that name.", logger);

        var selectedGame = matchingGames.First();
        return await CreateWorldProcessor.GenerateResponseAsync(interaction, selectedGame, logger);
    }
}
