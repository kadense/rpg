using System.Reflection;
using System.Text;
using System.Text.Json;
using Kadense.Models.Discord;

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


    public async Task RegisterCommandAsync(Type commandType)
    {
        var discordApplicationId = Environment.GetEnvironmentVariable("DISCORD_APP_ID");
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
            command.Options!.Add(new DiscordCommandOption
            {
                Name = option.Name,
                Description = option.Description,
                Required = option.Required
            });
        }

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bot {Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN")}");
        using var client = new HttpClient();
        var response = await client.SendAsync(request);
    }

    public async Task RegisterCommandsAsync()
    {
        var commands = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute<DiscordSlashCommandAttribute>() != null && typeof(IDiscordSlashCommandProcessor).IsAssignableFrom(t))
            .ToArray();

        foreach (var commandType in commands)
        {
            await RegisterCommandAsync(commandType);
        }
        
    }

    protected IDictionary<string, IDiscordSlashCommandProcessor> Commands { get; }

    public async Task<DiscordInteractionResponse> ExecuteAsync(DiscordInteraction interaction)
    {
        var data = interaction.Data;

        if (data == null)
            throw new ArgumentException("Interaction data is null", nameof(interaction));

        if (string.IsNullOrEmpty(data.Name))
            throw new ArgumentException("Interaction data name is null", nameof(interaction));

        if (!Commands.TryGetValue(data.Name, out var command))
            throw new ArgumentException($"Command '{data.Name}' not found", nameof(interaction));

        return await command.ExecuteAsync(interaction);
    }

}
