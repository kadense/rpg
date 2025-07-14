using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kadense.Models.Discord;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using System.Net;
using Discord.Rest;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kadense.RPG
{
    public class DiscordApi
    {
        private readonly ILogger<DiscordApi> _logger;

        public DiscordApi(ILogger<DiscordApi> logger)
        {
            _logger = logger;
        }

        private bool BypassDiscordAuth =>
            bool.TryParse(Environment.GetEnvironmentVariable("BYPASS_DISCORD_AUTH") ?? "false", out var bypass) && bypass;

        private JsonSerializerOptions serializerOptions =>
            new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

        [Function("DiscordAPI")]
        public async Task<DiscordApiResponse> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, CancellationToken cancellationToken)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();

            if (!BypassDiscordAuth)
            {
                // Must reply with 401 unauthorized if no ed25519 signature or timestamp, or if bad validation
                // https://discord.com/developers/docs/interactions/receiving-and-responding#security-and-authorization

                string discordKey = Environment.GetEnvironmentVariable("DISCORD_KEY")!;
                if (BypassDiscordAuth || string.IsNullOrWhiteSpace(discordKey))
                {
                    _logger.LogInformation("DISCORD_KEY environment variable is not set.");
                }

                var headers = req.Headers;
                if (!headers.TryGetValue("X-Signature-Timestamp", out var timestamp) ||
                    !headers.TryGetValue("X-Signature-Ed25519", out var signature))
                {
                    _logger.LogInformation("No timestamp or no sig");
                    return new DiscordApiResponse(new UnauthorizedResult());
                }

                var client = new DiscordRestClient();
                var validated = client.IsValidHttpInteraction(discordKey, signature.First(), timestamp.First(), body);

                if (!validated)
                {
                    _logger.LogInformation("Signature is not valid");
                    return new DiscordApiResponse(new UnauthorizedResult());
                }
            }

            var interaction = JsonSerializer.Deserialize<DiscordInteraction>(body, serializerOptions);
            var processor = new DiscordInteractionProcessor();
            var result = await processor.ExecuteAsync(interaction!, _logger);
            var stringify = JsonSerializer.Serialize(result.Response, serializerOptions);

            var response = new ContentResult()
            {
                Content = stringify,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };

            return new DiscordApiResponse(response, result.FollowupMessage);
        }
    }
}
