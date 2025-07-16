using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using IdentityModel.Client;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core.Tokens;

namespace Kadense.RPG;

public class DiscordSubCommand
{
    public DiscordSubCommand(Type commandType, MethodInfo commandMethod)
    {
        this.CommandType = commandType;
        this.CommandMethod = commandMethod;
    }
    public Type CommandType { get; }

    public MethodInfo CommandMethod { get;  }
}

public class DiscordInteractionProcessor
{
    public DataConnectionClient DataConnection { get; } = new DataConnectionClient();
    public DiscordInteractionProcessor()
    {
        Commands = new Dictionary<string, IDiscordCommandProcessor>();
        SubCommands = new Dictionary<string, DiscordSubCommand>();
        GetSlashCommands();
        GetButtonCommands();
    }

    public void GetButtonCommands()
    {
        // Register all commands in the current assembly
        var commands = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute<DiscordButtonCommandAttribute>() != null && typeof(IDiscordCommandProcessor).IsAssignableFrom(t))
            .Select(t => (IDiscordCommandProcessor)Activator.CreateInstance(t)!);

        foreach (var command in commands)
        {
            var attribute = command.GetType().GetCustomAttribute<DiscordButtonCommandAttribute>();
            if (attribute != null)
            {
                Commands[attribute.Name] = command;
            }

            var methods = command.GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<DiscordButtonCommandAttribute>() != null)
                .ToArray();

            foreach (var method in methods)
            {
                var methodAttribute = method.GetCustomAttribute<DiscordButtonCommandAttribute>();
                this.SubCommands[$"{attribute!.Name}:{methodAttribute!.Name}"] = new DiscordSubCommand(command.GetType(), method);
            }
        }
    }

    public void GetSlashCommands()
    {
        // Register all commands in the current assembly
        var commands = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute<DiscordSlashCommandAttribute>() != null && typeof(IDiscordCommandProcessor).IsAssignableFrom(t))
            .Select(t => (IDiscordCommandProcessor)Activator.CreateInstance(t)!);

        foreach (var command in commands)
        {
            var attribute = command.GetType().GetCustomAttribute<DiscordSlashCommandAttribute>();
            if (attribute != null)
            {
                Commands[attribute.Name] = command;
            }

            var methods = command.GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<DiscordButtonCommandAttribute>() != null)
                .ToArray();

            foreach (var method in methods)
            {
                var methodAttribute = method.GetCustomAttribute<DiscordButtonCommandAttribute>();
                this.SubCommands[$"{attribute!.Name}:{methodAttribute!.Name}"] = new DiscordSubCommand(command.GetType(), method);
            }
        }
    }


    public async Task<string> RegisterCommandAsync(Type commandType, ILogger logger)
    {
        var games = new GamesFactory().EndGames();
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
            var choices = option.Choices.Select(choice => new DiscordCommandOptionChoice
            {
                Name = choice,
                Value = choice
            }).ToList();

            switch (option.AutoChoices)
            {
                case DiscordSlashCommandChoicesMethod.GamesWithWorlds:
                    choices.AddRange(games.Where(g => g.WorldSection != null).Select(g => new DiscordCommandOptionChoice
                    {
                        Name = g.Name,
                        Value = g.Name
                    }));
                    break;

                case DiscordSlashCommandChoicesMethod.GamesWithCharacters:
                    choices.AddRange(games.Where(g => g.CharacterSection != null).Select(g => new DiscordCommandOptionChoice
                    {
                        Name = g.Name,
                        Value = g.Name
                    }));
                    break;

                case DiscordSlashCommandChoicesMethod.Games:
                    choices.AddRange(games.Select(g => new DiscordCommandOptionChoice
                    {
                        Name = g.Name,
                        Value = g.Name
                    }));
                    break;



                case DiscordSlashCommandChoicesMethod.GamesWithCustomDecks:
                    choices.AddRange(games.Where(g => g.CustomDecks.Count() > 0).Select(g => new DiscordCommandOptionChoice
                    {
                        Name = g.Name,
                        Value = g.Name
                    }));
                    break;

                case DiscordSlashCommandChoicesMethod.Manual:
                default:
                    // Use the manually defined choices
                    break;
            }


            var commandOption = new DiscordCommandOption
            {
                Name = option.Name,
                Description = option.Description,
                Required = option.Required,
                Choices = choices.OrderBy(c => c.Name).ToList(),
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

    protected IDictionary<string, IDiscordCommandProcessor> Commands { get; }

    protected IDictionary<string, DiscordSubCommand> SubCommands { get; }

    protected DiscordApiResponseContent ErrorResult(string content, ILogger logger)
    {
        logger.LogInformation(content);
        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponseBuilder()
                .WithResponseType(DiscordInteractionResponseType.CHANNEL_MESSAGE_WITH_SOURCE)
                .WithData()
                    .WithContent(content)
                .End()
                .Build()
        };
    }

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var data = interaction.Data;

        if (data == null)
            return new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponse
                {
                    Type = 1, // Pong response type
                    Data = new DiscordInteractionResponseData
                    {
                        Content = "No interaction data provided."
                    },
                }
            };
        if (interaction.Type == 2) // APPLICATION_COMMAND
        {
            if (string.IsNullOrEmpty(data.Name))
                return ErrorResult("Command name is null", logger);


            logger.LogInformation($"trying to determine command:{data.Name}");
            if (!Commands.TryGetValue(data.Name, out var command))
                return ErrorResult($"command {data.Name} is not recognised", logger);

            await DataConnection.WriteDiscordInteractionAsync(interaction);

            return await command.ExecuteAsync(interaction, logger);
        }
        else if (interaction.Type == 3 || interaction.Type == 5) // MESSAGE_COMPONENT
        {
            if (string.IsNullOrEmpty(data.CustomId))
                return new DiscordApiResponseContent
                {
                    Response = new DiscordInteractionResponse
                    {
                        Type = 1, // Pong response type
                        Data = new DiscordInteractionResponseData
                        {
                            Content = "interaction data name is not populated."
                        },
                    }
                };

            if (!Commands.TryGetValue(data.CustomId, out var command))
            {
                if (!SubCommands.TryGetValue(data.CustomId, out var subCommand))
                    return ErrorResult($"command {data.CustomId} is not a recognised command.", logger);

                var subCommandInstance = Activator.CreateInstance(subCommand!.CommandType);
                var subCommandResult = (Task<DiscordApiResponseContent>)subCommand!.CommandMethod!.Invoke(subCommandInstance, new object?[] { interaction, logger })!;
                return await subCommandResult;
            }

            await DataConnection.WriteDiscordInteractionAsync(interaction);

            return await command.ExecuteAsync(interaction, logger);
        }
        else
        {
            await DataConnection.WriteDiscordInteractionAsync(interaction);
            return ErrorResult("interaction data custom id is not populated.", logger);
        }
    }

}
