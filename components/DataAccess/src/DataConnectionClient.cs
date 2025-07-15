using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Kadense.Models.Discord;

namespace Kadense.RPG.DataAccess;

public class DataConnectionClient
{
    private KadenseRandomizer random = new KadenseRandomizer();
    public async Task<DeckOfCards?> ReadDeckAsync(string guildId, string channelId)
    {
        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"decks/{guildId}/{channelId}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        if (!deckExists)
        {
            // Create a new deck if it doesn't exist
            var deck = new DeckOfCards(random);
            await client.UploadAsync(new BinaryData(deck), overwrite: true, cancellationToken: CancellationToken.None);
            return deck;
        }
        else
        {
            // Read the existing deck from Azure Blob Storage
            await using var stream = await client.OpenReadAsync(cancellationToken: CancellationToken.None);
            return await JsonSerializer.DeserializeAsync<DeckOfCards>(stream)!;
        }
    }

    public async Task WriteDeckAsync(string guildId, string channelId, DeckOfCards deck)
    {
        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"decks/{guildId}/{channelId}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        await client.UploadAsync(new BinaryData(deck), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task WriteDiscordInteractionAsync(DiscordInteraction interaction)
    {
        if((Environment.GetEnvironmentVariable("WRITE_DISCORD_INTERACTIONS") ?? "false") != "true")
        {
            return; // Skip writing if the environment variable is not set to true
        }

        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"discordRequests/{DateTime.UtcNow.ToOADate()}/{interaction.Id}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        await client.UploadAsync(new BinaryData(interaction), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task WriteDiscordInteractionAsync(string interaction)
    {
        if((Environment.GetEnvironmentVariable("WRITE_DISCORD_INTERACTIONS") ?? "false") != "true")
        {
            return; // Skip writing if the environment variable is not set to true
        }

        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"discordRequests/{DateTime.UtcNow.ToOADate()}/{Guid.NewGuid()}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        await client.UploadAsync(new BinaryData(interaction), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task<Dictionary<string, DeckOfCards>> ReadDecksAsync()
    {
        BlobContainerClient containerClient = new BlobContainerClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg"
        );

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        Dictionary<string, DeckOfCards> deckList = new Dictionary<string, DeckOfCards>();
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: "decks/"))
        {
            Regex regex = new Regex(@"decks/(?<guildId>[^/]+)/(?<channelId>[^/]+)\.json");
            Match match = regex.Match(blobItem.Name);
            if (match.Success)
            {
                string guildId = match.Groups["guildId"].Value;
                string channelId = match.Groups["channelId"].Value;
                if (!blobItem.Deleted)
                {
                    var deck = await ReadDeckAsync(guildId, channelId);
                    if (deck != null)
                    {
                        deckList.Add($"{guildId}/{channelId}", deck);
                    }
                }
            }
        }
        return deckList;
    }

    
    public async Task<GameInstance?> ReadGameInstanceAsync(string guildId, string channelId)
    {
        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"gameInstance/{guildId}/{channelId}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var exists = await client.ExistsAsync(CancellationToken.None);

        if (!exists)
            return null;

        // Read the existing item from Azure Blob Storage
        await using var stream = await client.OpenReadAsync(cancellationToken: CancellationToken.None);
        return await JsonSerializer.DeserializeAsync<GameInstance>(stream)!;
    }

    public async Task WriteGameInstanceAsync(string guildId, string channelId, GameInstance gameInstance)
    {
        var client = new BlobClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg",
            $"gameInstance/{guildId}/{channelId}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        await client.UploadAsync(new BinaryData(gameInstance), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task<Dictionary<string, GameInstance>> ReadGameInstancesAsync()
    {
        BlobContainerClient containerClient = new BlobContainerClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!,
            "rpg"
        );

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        var resultList = new Dictionary<string, GameInstance>();
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: "gameInstance/"))
        {
            Regex regex = new Regex(@"gameInstance/(?<guildId>[^/]+)/(?<channelId>[^/]+)\.json");
            Match match = regex.Match(blobItem.Name);
            if (match.Success)
            {
                string guildId = match.Groups["guildId"].Value;
                string channelId = match.Groups["channelId"].Value;
                if (!blobItem.Deleted)
                {
                    var item = await ReadGameInstanceAsync(guildId, channelId);
                    if (item != null)
                    {
                        resultList.Add($"{guildId}/{channelId}", item);
                    }
                }
            }
        }
        return resultList;
    }
}