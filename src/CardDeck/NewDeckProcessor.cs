using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CardDeck;

[DiscordSlashCommand("new-deck", "Creates a new deck of cards!")]
[DiscordSlashCommandOption("jokers", "Include the jokers in the deck?", true, Type = DiscordSlashCommandOptionType.Integer, Choices = new[] { "true", "false" })]
public class NewDeckProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public async Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        bool jokers = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "jokers").FirstOrDefault()?.Value ?? "false");

        var embed = new DiscordEmbed
        {
            Title = "New deck of cards created for this channel",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        };

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;
        string userName = interaction.User!.Username ?? interaction.User!.GlobalName!.ToString();

        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"{guildId}/{channelId}/deck.json"
        );

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        DeckOfCards? deck = null;

        
        deck = new DeckOfCards(random);

        await client.UploadAsync(new BinaryData(deck), overwrite: true, cancellationToken: CancellationToken.None);
        
        return new DiscordInteractionResponse
        {
            Data = new DiscordInteractionResponseData
            {
                Content = $"{userName} refreshed the deck.",
                Embeds = new List<DiscordEmbed>() { embed },
            }
        };
    }
}
