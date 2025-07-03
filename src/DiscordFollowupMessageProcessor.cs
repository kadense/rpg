using System;
using Azure.Storage.Queues.Models;
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
    public async Task RunAsync([QueueTrigger("discordfollowup")] QueueMessage message)
    {
        var followupMessageRequest = message.Body.ToObjectFromJson<DiscordFollowupMessageRequest>()!;
        await discordFollowupClient.SendFollowupAsync(followupMessageRequest.Content!, followupMessageRequest.Token!, _logger);
        _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
    }
}

