using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using IdentityModel.Client;
using Kadense.Models.Discord;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG;

public class DiscordInteractionProcessor
{
    public DiscordInteractionProcessor()
    {
        Commands = new Dictionary<string, IDiscordSlashCommandProcessor>();

        // Register all commands in the current assembly
        var commands = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute<DiscordSlashCommandAttribute>() != null && typeof(IDiscordSlashCommandProcessor).IsAssignableFrom(t))
            .Select(t => (IDiscordSlashCommandProcessor)Activator.CreateInstance(t)!);

        foreach (var command in commands)
        {
            var attribute = command.GetType().GetCustomAttribute<DiscordSlashCommandAttribute>();
            if (attribute != null)
            {
                Commands[attribute.Name] = command;
            }
        }
    }


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

        while (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            logger.LogInformation($"Requesting {request.Method} {request.RequestUri} is being rate limited. Waiting for retry.");

            var delaySeconds = await response.Content.ReadFromJsonAsync<DiscordRateLimitResponse>();
            var delayMs = (int)(delaySeconds?.RetryAfter ?? 1 * 1000 * 1.2); // Convert seconds to milliseconds and add 20%

            logger.LogInformation($"Waiting for {delayMs} milliseconds before retrying.");

            await Task.Delay(delayMs); // Wait for 1 second before retrying

            request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.SetToken("Bot", token!);

            response = await client.SendAsync(request);
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        logger.LogInformation($"Response: {responseBody}");


        response.EnsureSuccessStatusCode();
        logger.LogInformation($"Registered command {command.Name} with response status: {response.StatusCode}");
        return attribute!.Name;
    }

    public async Task<string[]> RegisterCommandsAsync(ILogger logger)
    {
        var commands = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute<DiscordSlashCommandAttribute>() != null && typeof(IDiscordSlashCommandProcessor).IsAssignableFrom(t))
            .ToArray();


        var commandNames = new List<string>();
        foreach (var commandType in commands)
        {
            var commandName = await RegisterCommandAsync(commandType, logger);
            commandNames.Add(commandName);
        }

        logger.LogInformation($"Registered commands: {string.Join(", ", commandNames)}");
        return commandNames.ToArray();
    }

    protected IDictionary<string, IDiscordSlashCommandProcessor> Commands { get; }

    public async Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction)
    {
        var data = interaction.Data;

        if (data == null)
            return new DiscordInteractionResponse
            {
                Type = 1, // Pong response type
                Data = new DiscordInteractionResponseData
                {
                    Content = "No interaction data provided."
                },
            };

        if (string.IsNullOrEmpty(data.Name))
            return new DiscordInteractionResponse
            {
                Type = 1, // Pong response type
                Data = new DiscordInteractionResponseData
                {
                    Content = "interaction data name is not populated."
                },
            };

        if (!Commands.TryGetValue(data.Name, out var command))
            return new DiscordInteractionResponse
            {
                Type = 1, // Pong response type
                Data = new DiscordInteractionResponseData
                {
                    
                    Content = "interaction data name is not populated."
                },
            };
            
        return await command.ExecuteAsync(interaction);
    }

}
