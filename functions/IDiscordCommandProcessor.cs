using Kadense.Models.Discord;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG;

public interface IDiscordCommandProcessor
{
    public Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger);
}

public interface IDiscordSlashCommandProcessor : IDiscordCommandProcessor
{

}
public interface IDiscordButtonCommandProcessor : IDiscordCommandProcessor
{

}