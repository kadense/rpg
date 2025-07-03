
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IdentityModel.Client;
using Kadense.Models.Discord;

namespace Kadense.RPG;
public class DiscordFollowupClient
{
    public DiscordFollowupClient()
    {
        var discordApplicationId = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID");
        BaseUrl = $"https://discord.com/api/v10/webhooks/{discordApplicationId}";
    }

    public string BaseUrl;

    public async Task SendFollowupAsync(string content, string interactionToken)
    {
        string url = $"{BaseUrl}/{interactionToken}?wait=true";
        var responseData = new DiscordInteractionResponseData
        {
            Content = content,
            Flags = 64, // Ephemeral message
        };

        var json = JsonSerializer.Serialize(responseData, new JsonSerializerOptions
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

        var token = Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET");
        request.SetToken("Bot", token!);

        using var client = new HttpClient();
        var response = await client.SendAsync(request);
        

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {

            var delaySeconds = await response.Content.ReadFromJsonAsync<DiscordRateLimitResponse>();
            var delayMs = (int)(delaySeconds?.RetryAfter ?? 1 * 1000 * 1.2); // Convert seconds to milliseconds and add 20%

            
            await Task.Delay(delayMs); // Wait for 1 second before retrying

            response = await client.SendAsync(request);
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

            var token = Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET");
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