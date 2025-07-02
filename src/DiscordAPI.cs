using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kadense.Models.Discord;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using System.Net;
using Discord.Rest;

namespace Kadense.RPG
{
    public class DiscordAPI
    {
        private readonly ILogger<DiscordAPI> _logger;

        public DiscordAPI(ILogger<DiscordAPI> logger)
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
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
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
                if (!headers.TryGetValues("X-Signature-Timestamp", out var timestamp) ||
                    !headers.TryGetValues("X-Signature-Ed25519", out var signature))
                {
                    _logger.LogInformation("No timestamp or no sig");
                    return req.CreateResponse(HttpStatusCode.Unauthorized);
                }

                var client = new DiscordRestClient();
                var validated = client.IsValidHttpInteraction(discordKey, signature.First(), timestamp.First(), body);

                if (!validated)
                {
                    _logger.LogInformation("Signature is not valid");
                    return req.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
                
            var interaction = JsonSerializer.Deserialize<DiscordInteraction>(body, serializerOptions);
            var processor = new DiscordInteractionProcessor();
            var result = await processor.ExecuteAsync(interaction!);
            var stringify = JsonSerializer.Serialize(result, serializerOptions);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(stringify);

            _logger.LogInformation("Discord interaction processed successfully.");
            return response;
        }
    }
}
