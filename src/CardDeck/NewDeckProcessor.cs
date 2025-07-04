using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CardDeck;

[DiscordSlashCommand("new-deck", "Creates a new deck of cards!")]
[DiscordSlashCommandOption("type", "Type of deck to create", false, Choices = new string[] { "Standard Deck" }, AutoChoices = DiscordSlashCommandChoicesMethod.GamesWithCustomDecks)]
[DiscordSlashCommandOption("jokers", "Include the jokers in the deck?", false, Type = DiscordSlashCommandOptionType.Integer, Choices = new[] { "true", "false" })]
public class NewDeckProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public async Task<(DiscordInteractionResponse, DiscordFollowupMessageRequest?)> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
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

        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"{guildId}/{channelId}/deck.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        DeckOfCards? deck = null;

        deck = new DeckOfCards(random, type, jokers);

        await client.UploadAsync(new BinaryData(deck), overwrite: true, cancellationToken: CancellationToken.None);
        
        return (new DiscordInteractionResponse
        {
            Data = new DiscordInteractionResponseData
            {
                Content = $"The deck has been refreshed for this channel. It now contains {deck.Count()} cards.",
                Embeds = new List<DiscordEmbed>() { embed },
            }
        }, null);
    }
}
