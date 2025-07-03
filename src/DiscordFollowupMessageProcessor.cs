using System;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG;

public class DiscordFollowupMessageProcessor
{
    private readonly ILogger<DiscordFollowupMessageProcessor> _logger;
    private DiscordFollowupClient discordFollowupClient;

    public DiscordFollowupMessageProcessor(ILogger<DiscordFollowupMessageProcessor> logger)
    {
        _logger = logger;
        discordFollowupClient = new DiscordFollowupClient();
    }    

    [Function(nameof(DiscordFollowupMessageProcessor))]
    public async Task Run([BlobTrigger("rpg/follow-ups/{name}")] Stream stream, string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var followupMessageRequest = await JsonSerializer.DeserializeAsync<DiscordFollowupMessageRequest>(blobStreamReader.BaseStream);
        await discordFollowupClient.SendFollowupAsync(followupMessageRequest!.Content!, followupMessageRequest.Token!, _logger);

        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {followupMessageRequest.Content}");
    }
}

