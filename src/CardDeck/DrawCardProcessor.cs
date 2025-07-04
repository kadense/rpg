using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace Kadense.RPG.CardDeck;

[DiscordSlashCommand("draw", "Draw from a deck of cards!")]
[DiscordSlashCommandOption("cards", "How many cards?", true, Type = DiscordSlashCommandOptionType.Integer)]
public class DrawCardProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public async Task<(DiscordInteractionResponse, DiscordFollowupMessageRequest?)> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        int cards = int.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "cards").FirstOrDefault()?.Value ?? "1");

        var embed = new DiscordEmbed
        {
            Title = "Adventure Dice Roll",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        };

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;
        //string userName = interaction.User!.Username ?? interaction.User!.GlobalName!.ToString();

        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"{guildId}/{channelId}/deck.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        DeckOfCards? deck = null;

        if (!deckExists)
        {
            // Create a new deck if it doesn't exist
            deck = new DeckOfCards(random);
            await client.UploadAsync(new BinaryData(deck), overwrite: true, cancellationToken: CancellationToken.None);
        }
        else
        {
            // Read the existing deck from Azure Blob Storage
            await using var stream = await client.OpenReadAsync(cancellationToken: CancellationToken.None);
            deck = await System.Text.Json.JsonSerializer.DeserializeAsync<DeckOfCards>(stream);
        }

        var cardsDrawn = deck!.DrawCards(cards);

        await client.UploadAsync(new BinaryData(deck), overwrite: true, cancellationToken: CancellationToken.None);

        var followupClient = new DiscordFollowupClient();
        var followupMessage = followupClient.CreateFollowup(guildId, channelId, $"You drew the following cards: {string.Join(", ", cardsDrawn)}", interaction.Token!, logger);

        return (new DiscordInteractionResponse
        {
            Data = new DiscordInteractionResponseData
            {
                Content = $"Drew {cards} from the deck.",
                Embeds = new List<DiscordEmbed>() { embed },
            }
        }, followupMessage);
    }
}
