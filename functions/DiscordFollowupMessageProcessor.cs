using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Kadense.RPG.CreateWorld;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace Kadense.RPG;

public class DiscordFollowupMessageProcessor
{
    private readonly DataConnectionClient dataConnectionClient = new DataConnectionClient();
    private readonly ILogger<DiscordFollowupMessageProcessor> _logger;
    private DiscordFollowupClient discordFollowupClient;

    public DiscordFollowupMessageProcessor(ILogger<DiscordFollowupMessageProcessor> logger)
    {
        _logger = logger;
        discordFollowupClient = new DiscordFollowupClient();
    }

    public string QueryLLM(string content)
    {
        if((Environment.GetEnvironmentVariable("OPENAI_BYPASS") ?? "false") == "true")
        {
            _logger.LogInformation("Bypassing LLM for private AI prompt response.");
            return content;
        }

        var client = new ChatClient(
            model: "gpt-4.1",
            apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        );

        var completion = client.CompleteChat(content);
        _logger.LogInformation($"LLM Response: {JsonSerializer.Serialize(completion, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull })}");
        if (completion.Value?.Content == null)
        {
            _logger.LogError("LLM returned null content.");
            return "An error occurred while processing your request.";
        }
        
        return $"**Example Introduction:**\n\n{string.Join("\n\n", completion.Value.Content.Where(c => c.Kind == ChatMessageContentPartKind.Text).Select(c => c.Text))}";
    }

    public DiscordInteractionResponse GetDefaultResponse(string content)
    {
        return new DiscordInteractionResponseBuilder()
            .WithData()
                .WithContainerComponent()
                    .WithTextDisplayComponent()
                        .WithContent(content)
                    .End()
                .End()
            .End()
            .Build();
    }

    [Function(nameof(DiscordFollowupMessageProcessor))]
    public async Task Run([QueueTrigger("kadense-rpg-followup-messages", Connection = "AzureWebJobsStorage")] QueueMessage message)
    {
        bool useOriginalMessage = false;
        var followupMessageRequest = message.Body.ToObjectFromJson<DiscordFollowupMessageRequest>();
        string content = message.Body.ToString();


        DiscordInteractionResponse? response = null;
        bool isPrivate = true;
        switch (followupMessageRequest!.Type)
        {
            case FollowupProcessorType.PrivateAiPromptResponse:
                content = QueryLLM(content);
                isPrivate = true;
                break;
            case FollowupProcessorType.PublicAiPromptResponse:
                content = QueryLLM(content);
                isPrivate = false;
                break;

            case FollowupProcessorType.GameWorldAiPromptResponse:
                isPrivate = false;
                content = QueryLLM(content);
                useOriginalMessage = true;

                _logger.LogInformation($"Getting Game Instance: {content}");

                var instance = await dataConnectionClient.ReadGameInstanceAsync(followupMessageRequest.GuildId!, followupMessageRequest.ChannelId!);

                if (instance == null || string.IsNullOrEmpty(instance.GameName))
                {
                    _logger.LogError("Could not get game name");
                    break;
                }

                var selectedGame = new GamesFactory()
                    .EndGames()
                    .Where(x => x.Name!.ToLowerInvariant() == instance.GameName!.ToLowerInvariant())
                    .First();

                response = CreateWorldProcessor.GenerateResponse(selectedGame, instance.WorldSetup!, _logger, story: content).Response;
                break;

            case FollowupProcessorType.PrivateResponse:
                isPrivate = true;
                break;

            case FollowupProcessorType.PublicResponse:
                isPrivate = false;
                break;

            default:
                throw new ArgumentException($"Unknown followup message type: {followupMessageRequest.Type}");
        }

        if (response == null)
            response = this.GetDefaultResponse(content);
        
        if (Environment.GetEnvironmentVariable("DISCORD_SEND_FOLLOWUP") == "false")
            {
                _logger.LogInformation($"Skipping Discord followup message: {content}");
                return;
            }

        _logger.LogInformation($"Followup Response: {content}");

        await discordFollowupClient.SendFollowupAsync(response.Data!, followupMessageRequest.Token!, _logger, privateMessage: isPrivate, useOriginalMessage: useOriginalMessage);

        _logger.LogInformation($"C# Queue trigger function Processed ID: {message.MessageId} \n Data: {followupMessageRequest.Content}");
    }
}

