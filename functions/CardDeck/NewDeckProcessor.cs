using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CardDeck;

[DiscordSlashCommand("new-deck", "Creates a new deck of cards!")]
[DiscordSlashCommandOption("type", "Type of deck to create", false, Choices = new string[] { "Standard Deck" }, AutoChoices = DiscordSlashCommandChoicesMethod.GamesWithCustomDecks)]
[DiscordSlashCommandOption("jokers", "Include the jokers in the deck?", false, Type = DiscordSlashCommandOptionType.Integer, Choices = new[] { "true", "false" })]
public class NewDeckProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();
    private readonly DataConnectionClient client = new DataConnectionClient();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        bool jokers = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "jokers").FirstOrDefault()?.Value ?? "false");
        string type = interaction.Data?.Options?.Where(opt => opt.Name == "type").FirstOrDefault()?.Value ?? "Standard Deck";

        var embed = new DiscordEmbed
        {
            Title = "New deck of cards created for this channel",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        };

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;

        var deck = new DeckOfCards(random, type, jokers);
        
        await client.WriteDeckAsync(guildId, channelId, deck);

        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponse
            {
                Data = new DiscordInteractionResponseData
                {
                    Content = $"The deck has been refreshed for this channel. It now contains {deck.Count()} cards.",
                    Embeds = new List<DiscordEmbed>() { embed },
                }
            }
        };
    }
}
