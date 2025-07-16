using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-list-participants", "List the participants in the Troika game.")]
public class ListTroikaParticipantsProcessor : IDiscordSlashCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();


    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
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

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }
}
