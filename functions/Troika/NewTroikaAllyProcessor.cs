using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-new-ally", "Creates an ally for Troika!")]
[DiscordSlashCommandOption("name", "Name of the ally?", true, Type = DiscordSlashCommandOptionType.Integer)]
public class NewTroikaAllyProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    private readonly DataConnectionClient client = new DataConnectionClient();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        string name = interaction.Data?.Options?.Where(opt => opt.Name == "name").FirstOrDefault()?.Value ?? "name";
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
                    Type = "Ally",
                    Attributes = new Dictionary<string, int>
                    {
                    }
                });

                await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);
                result = gameInstance.GetParticipantText(["Initiative"]);
                resultOverall = $"The participant list has been updated, it now contains {gameInstance.Participants.Count()}.";
            }
        }

        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponse
            {
                Data = new DiscordInteractionResponseData
                {
                    Content = $"**{resultOverall}**\n\n{result}",
                }
            }
        };
    }
}
