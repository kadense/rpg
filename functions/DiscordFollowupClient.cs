
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IdentityModel.Client;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG;
public class DiscordFollowupClient
{
    public DiscordFollowupClient()
    {
        var discordApplicationId = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID");
        BaseUrl = $"https://discord.com/api/v10/webhooks/{discordApplicationId}";
    }

    public string QueueName = "discordfollowup";
    public string BaseUrl;

    public DiscordFollowupMessageRequest? CreateFollowup(string guildId, string channelId, string content, string interactionToken, ILogger logger)
    {
        logger.LogInformation($"Creating followup message for guild {guildId}, channel {channelId} with content: {content}");
        return new DiscordFollowupMessageRequest
        {
            GuildId = guildId,
            ChannelId = channelId,
            Content = content,
            Token = interactionToken
        };
    }

    public async Task SendFollowupAsync(DiscordInteractionResponseData responseData, string interactionToken, ILogger logger, bool privateMessage = false, bool useOriginalMessage = false)
    {
        string url = !useOriginalMessage ? $"{BaseUrl}/{interactionToken}?wait=true" : $"{BaseUrl}/{interactionToken}/messages/@original"; 
        
        var json = JsonSerializer.Serialize(responseData, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = !useOriginalMessage ? HttpMethod.Post : HttpMethod.Patch,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        request.SetToken("Bot", token!);

        using var client = new HttpClient();
        logger.LogInformation($"Requesting {request.Method} {request.RequestUri} using secret starting \"{token!.Substring(0, 5)}****\" with body: {json}");
        var response = await client.SendAsync(request);

        int attempts = 0;
        while (attempts < 5 && (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests || response.StatusCode == System.Net.HttpStatusCode.NotFound))
        {
            int delayMs = 1000;
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                logger.LogInformation($"Requesting {request.Method} {request.RequestUri} is being rate limited. Waiting for retry.");
                var delaySeconds = await response.Content.ReadFromJsonAsync<DiscordRateLimitResponse>();
                delayMs = (int)(delaySeconds?.RetryAfter ?? 1 * 1000 * 1.2); // Convert seconds to milliseconds and add 20%
            }

            logger.LogInformation($"Waiting for {delayMs} milliseconds before retrying.");
            await Task.Delay(delayMs);


            request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.SetToken("Bot", token!);

            logger.LogInformation($"Retrying {request.Method} {request.RequestUri} with body: {json}");
            response = await client.SendAsync(request);
            attempts++;
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }

    /* 
        public async Task<string> RegisterCommandAsync(Type commandType, ILogger logger)
        {
            var discordApplicationId = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID");
            string url = $"https://discord.com/api/v10/applications/{discordApplicationId}/commands";


            var attribute = commandType.GetCustomAttribute<DiscordSlashCommandAttribute>();

            var command = new DiscordCommand
            {
                Name = attribute!.Name,
                Description = attribute!.Description,
            };

            var attributeOptions = commandType.GetCustomAttributes<DiscordSlashCommandOptionAttribute>().ToList();
            foreach (var option in attributeOptions)
            {
                var commandOption = new DiscordCommandOption
                {
                    Name = option.Name,
                    Description = option.Description,
                    Required = option.Required,
                    Choices = option.Choices.Select(choice => new DiscordCommandOptionChoice
                    {
                        Name = choice,
                        Value = choice
                    }).ToList()
                };
                command.Options!.Add(commandOption);
            }
            var json = JsonSerializer.Serialize(command, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            request.SetToken("Bot", token!);
            logger.LogInformation($"Requesting {request.Method} {request.RequestUri} using secret starting \"{token!.Substring(0, 5)}****\" with body: {json}");

            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            logger.LogInformation($"Response: {responseBody}");

            response.EnsureSuccessStatusCode();
            logger.LogInformation($"Registered command {command.Name} with response status: {response.StatusCode}");
            return attribute!.Name;
        } */
}