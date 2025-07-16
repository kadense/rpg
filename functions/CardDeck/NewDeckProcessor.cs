using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
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
        object? jokersOption = interaction.Data?.Options?.Where(opt => opt.Name == "jokers").FirstOrDefault()?.Value ?? "1";

        bool jokers = bool.Parse(jokersOption.ToString()!);
        string type = (interaction.Data?.Options?.Where(opt => opt.Name == "type").FirstOrDefault()?.Value ?? "Standard Deck").ToString()!;

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;

        var deck = new DeckOfCards(random, type, jokers);
        
        await client.WriteDeckAsync(guildId, channelId, deck);

        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponseBuilder()
                .WithData()
                    .WithContent($"The deck has been refreshed for this channel. It now contains {deck.Count()} cards.")
                    .WithEmbed()
                        .WithTitle("New deck of cards created for this channel")
                        .WithColor(0x00FF00) // Green color
                    .End()
                .End()
                .Build(),
        };
    }
}
