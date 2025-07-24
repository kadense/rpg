using System.Text.Json;
using System.Text.RegularExpressions;
using Azure;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Kadense.Models.Discord;

namespace Kadense.RPG.DataAccess;

public class DataConnectionClient
{
    public DataConnectionClient()
    {
        ConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING")!;
        ContainerName = "rpg";
    }

    public string ContainerName { get; set; }

    public DataConnectionClient(string connectionString, string containerName = "rpg")
    {
        ConnectionString = connectionString;
        ContainerName = containerName;
    }

    private string ConnectionString { get; set; }
    private KadenseRandomizer random = new KadenseRandomizer();
    public async Task<DeckOfCards?> ReadDeckAsync(string guildId, string channelId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
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
            ConnectionString,
            ContainerName,
            $"decks/{guildId}/{channelId}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        await client.UploadAsync(new BinaryData(deck), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task WriteDiscordInteractionAsync(DiscordInteraction interaction)
    {
        if ((Environment.GetEnvironmentVariable("WRITE_DISCORD_INTERACTIONS") ?? "false") != "true")
        {
            return; // Skip writing if the environment variable is not set to true
        }

        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"discordRequests/{DateTime.UtcNow.ToOADate()}/{interaction.Id}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        await client.UploadAsync(new BinaryData(interaction), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task WriteDiscordInteractionAsync(string interaction)
    {
        if ((Environment.GetEnvironmentVariable("WRITE_DISCORD_INTERACTIONS") ?? "false") != "true")
        {
            return; // Skip writing if the environment variable is not set to true
        }

        var client = new BlobClient(
            ConnectionString,
            "rpg",
            $"discordRequests/{DateTime.UtcNow.ToOADate()}/{Guid.NewGuid()}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        await client.UploadAsync(new BinaryData(interaction), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task WriteDiscordInteractionResponseAsync(string guildId, string channelId, DiscordInteractionResponse response)
    {
        if ((Environment.GetEnvironmentVariable("WRITE_DISCORD_INTERACTIONS") ?? "false") != "true")
        {
            return; // Skip writing if the environment variable is not set to true
        }

        var client = new BlobClient(
            ConnectionString,
            "rpg",
            $"rpgResponses/{guildId}/{channelId}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        await client.UploadAsync(new BinaryData(response), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task<Dictionary<string, DeckOfCards>> ReadDecksAsync()
    {
        BlobContainerClient containerClient = new BlobContainerClient(
            ConnectionString,
            ContainerName
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
            ConnectionString,
            ContainerName,
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
            ConnectionString,
            ContainerName,
            $"gameInstance/{guildId}/{channelId}.json"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var deckExists = await client.ExistsAsync(CancellationToken.None);

        await client.UploadAsync(new BinaryData(gameInstance), overwrite: true, cancellationToken: CancellationToken.None);
    }

    public async Task<Dictionary<string, GameInstance>> ReadGameInstancesAsync()
    {
        BlobContainerClient containerClient = new BlobContainerClient(
            ConnectionString,
            ContainerName
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



    public async Task<string?> ReadGameAssetContentTypeAsync(string gameId, string filePath)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"{gameId}/{filePath}"
        );

        await client.GetParentBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None);

        var exists = await client.ExistsAsync(CancellationToken.None);

        if (!exists)
            return null;

        var properties = await client.GetPropertiesAsync();
        return properties.Value.ContentType;
    }

    public async Task ReadGameAssetToStreamAsync(string gameId, string filePath, Stream targetStream)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"{gameId}/{filePath}"
        );

        await client.DownloadToAsync(targetStream);
    }

    public async Task WriteGameAsync(Game game)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{game.Id}/Game.json"
        );

        await client.UploadAsync(new BinaryData(game), overwrite: true, cancellationToken: CancellationToken.None);
        await WriteGameToIndexAsync(game);
    }

    public async Task<Game?> ReadGameAsync(string gameId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/Game.json"
        );

        var result = await client.DownloadContentAsync(cancellationToken: CancellationToken.None);

        return result.Value.Content.ToObjectFromJson<Game>();
    }

    public async Task<bool> ExistsGameAsync(Game game)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{game.Id}/Game.json"
        );

        var result = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        return result.Value;
    }

    public async Task<Dictionary<string, GameIndexItem>> ReadGameIndexAsync()
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        return index;
    }

    public async Task WriteGameToIndexAsync(Game game)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        index[game.Id] = new GameIndexItem()
        {
            Name = game.Name,
            ImagePath = game.ImagePath.FirstOrDefault()
        };

        await client.UploadAsync(new BinaryData(index), overwrite: true, cancellationToken: CancellationToken.None);
    }


    #region " Game Location "
    public async Task WriteGameLocationAsync(string gameId, GameLocation location)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/locations/{location.Id}/Location.json"
        );

        await client.UploadAsync(new BinaryData(location), overwrite: true, cancellationToken: CancellationToken.None);
        await WriteGameLocationToIndexAsync(gameId, location);
    }

    public async Task<GameLocation?> ReadGameLocationAsync(string gameId, string locationId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/locations/{locationId}/Location.json"
        );

        var result = await client.DownloadContentAsync(cancellationToken: CancellationToken.None);

        return result.Value.Content.ToObjectFromJson<GameLocation>();
    }

    public async Task<bool> ExistsGameLocationAsync(string gameId, string locationId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/locations/{locationId}/Location.json"
        );

        var result = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        return result.Value;
    }

    public async Task<Dictionary<string, GameIndexItem>> ReadGameLocationIndexAsync(string gameId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/locations/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        return index;
    }

    public async Task WriteGameLocationToIndexAsync(string gameId, GameLocation location)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/locations/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        index[location.Id] = new GameIndexItem()
        {
            Name = location.Name,
            ImagePath = location.ImagePath.FirstOrDefault()
        };

        await client.UploadAsync(new BinaryData(index), overwrite: true, cancellationToken: CancellationToken.None);
    }

    #endregion



    #region " Game Character "
    public async Task WriteGameCharacterAsync(string gameId, GameCharacter character)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/characters/{character.Id}/Character.json"
        );

        await client.UploadAsync(new BinaryData(character), overwrite: true, cancellationToken: CancellationToken.None);
        await WriteGameCharacterToIndexAsync(gameId, character);
    }

    public async Task<GameCharacter?> ReadGameCharacterAsync(string gameId, string characterId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/characters/{characterId}/Character.json"
        );

        var result = await client.DownloadContentAsync(cancellationToken: CancellationToken.None);

        return result.Value.Content.ToObjectFromJson<GameCharacter>();
    }

    public async Task<bool> ExistsGameCharacterAsync(string gameId, string characterId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/characters/{characterId}/Character.json"
        );

        var result = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        return result.Value;
    }

    public async Task<Dictionary<string, GameIndexItem>> ReadGameCharacterIndexAsync(string gameId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/characters/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        return index;
    }

    public async Task WriteGameCharacterToIndexAsync(string gameId, GameCharacter character)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/characters/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        index[character.Id] = new GameIndexItem()
        {
            Name = character.Name,
            ImagePath = character.ImagePath.FirstOrDefault(),
            AdditionalInformation = character.Pronouns
        };

        await client.UploadAsync(new BinaryData(index), overwrite: true, cancellationToken: CancellationToken.None);
    }

    #endregion

    

    #region " Game Item "
    public async Task WriteGameItemAsync(string gameId, GameItem item)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/items/{item.Id}/Item.json"
        );

        await client.UploadAsync(new BinaryData(item), overwrite: true, cancellationToken: CancellationToken.None);
        await WriteGameItemToIndexAsync(gameId, item);
    }

    public async Task<GameItem?> ReadGameItemAsync(string gameId, string itemId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/items/{itemId}/Item.json"
        );

        var result = await client.DownloadContentAsync(cancellationToken: CancellationToken.None);

        return result.Value.Content.ToObjectFromJson<GameItem>();
    }

    public async Task<bool> ExistsGameItemAsync(string gameId, string itemId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/items/{itemId}/Item.json"
        );

        var result = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        return result.Value;
    }

    public async Task<Dictionary<string, GameIndexItem>> ReadGameItemIndexAsync(string gameId)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/items/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        return index;
    }

    public async Task WriteGameItemToIndexAsync(string gameId, GameItem item)
    {
        var client = new BlobClient(
            ConnectionString,
            ContainerName,
            $"games/{gameId}/items/Index.json"
        );
        Dictionary<string, GameIndexItem> index = new Dictionary<string, GameIndexItem>();
        var exists = await client.ExistsAsync(cancellationToken: CancellationToken.None);
        if (exists.Value)
        {
            var read = await client.DownloadContentAsync();
            index = read.Value.Content.ToObjectFromJson<Dictionary<string, GameIndexItem>>()!;
        }
        index[item.Id] = new GameIndexItem()
        {
            Name = item.Name,
            ImagePath = item.ImagePath.FirstOrDefault()
        };

        await client.UploadAsync(new BinaryData(index), overwrite: true, cancellationToken: CancellationToken.None);
    }
    
    #endregion

}