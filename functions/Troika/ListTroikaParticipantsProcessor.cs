using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-list-participants", "List the participants in the Troika game.")]
public class ListTroikaParticipantsProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    private readonly DataConnectionClient client = new DataConnectionClient();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;

        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);

        string? result = null;

        if (gameInstance == null)
        {
            result = "No game instance found for this channel. Please create a game first.";
        }
        else
        {
            if (gameInstance.GameName != "Troika")
            {
                result = "This game instance is not a Troika game. Please create a Troika game first.";
            }
            else
            {
               
                result = gameInstance.GetParticipantText();
            }
        }

        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponse
            {
                Data = new DiscordInteractionResponseData
                {
                    Content = result,
                }
            }
        };
    }
}
