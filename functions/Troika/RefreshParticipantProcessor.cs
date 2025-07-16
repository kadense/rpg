using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordButtonCommand("troika-refresh-participant-list", "Refresh the participant list")]
public partial class RefreshParticipantProcessor : IDiscordButtonCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();


    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        logger.LogInformation("Loading game information from persistent storage");

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;

        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);


        if (gameInstance == null || gameInstance.GameName == null)
        {
            gameInstance = new GameInstance()
            {
                GameName = "Troika"
            };

            await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);
        }

        return new DiscordApiResponseContent
        {
            Response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, true)
        };        
    }
}
