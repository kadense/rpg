using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-delay-move", "Delays the last move of a Troika game participant.")]
public class DelayTroikaMoveProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    private readonly DataConnectionClient client = new DataConnectionClient();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;


        string? result = null;
        var deck = await client.ReadDeckAsync(guildId, channelId);

        if (deck == null)
        {
            result = "No game instance found for this channel. Please create a game first.";
        }
        else
        {
            if (deck.Name != "Troika Initiative")
            {
                result = "This deck is not a Troika deck. Please start a new one.";
            }
            else
            {
                if (deck.DrawnCards.Count == 0)
                {
                    result = "No cards have been drawn yet, you must draw a card first.";
                }
                else
                {
                    var lastDrawn = deck.DrawnCards.Last();
                    deck.DrawnCards.Remove(lastDrawn);
                    deck.Cards.Add(lastDrawn);
                    deck.Shuffle(random);
                    result = $"The last move has been delayed for {lastDrawn.Name}.";
                    await client.WriteDeckAsync(guildId, channelId, deck);
                }
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
