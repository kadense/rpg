using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-new-enemy", "Creates a player for Troika!")]
[DiscordSlashCommandOption("name", "Name of the player?", true, Type = DiscordSlashCommandOptionType.Integer)]
[DiscordSlashCommandOption("points", "Number of initiative points?", true, Type = DiscordSlashCommandOptionType.Integer)]
public class NewTroikaEnemyProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    private readonly DataConnectionClient client = new DataConnectionClient();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        string name = interaction.Data?.Options?.Where(opt => opt.Name == "name").FirstOrDefault()?.Value ?? "name";
        int points = int.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "points").FirstOrDefault()?.Value ?? "0");
        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;

        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);

        string? result = null;
        string? resultOverall = null;

        if (gameInstance == null)
        {
            result = "No game instance found for this channel. Please create a game first.";
            resultOverall = "The participant list could not be updated.";
        }
        else
        {
            if (gameInstance.GameName != "Troika")
            {
                result = "This game instance is not a Troika game. Please create a Troika game first.";
                resultOverall = "The participant list could not be updated.";
            }
            else
            {
                gameInstance.Participants.Add(new GameParticipant
                {
                    Name = name,
                    Type = "Enemy",
                    Attributes = new Dictionary<string, int>
                    {
                        { "Initiative", points }
                    }
                });

                await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);
                result = gameInstance.GetParticipantText();
                resultOverall = $"The participant list has been updated, it now contains {gameInstance.Participants.Count()}.";
            }
        }

        var embed = new DiscordEmbed
        {
            Title = result,
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        };

        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponse
            {
                Data = new DiscordInteractionResponseData
                {
                    Content = resultOverall,
                    Embeds = new List<DiscordEmbed>() { embed },
                }
            }
        };
    }
}
