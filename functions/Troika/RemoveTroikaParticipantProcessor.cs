using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-remove-participant", "Removes a participant from the pool!")]
[DiscordSlashCommandOption("id", "Id for the participant?", true, Type = DiscordSlashCommandOptionType.Integer)]
public class RemoveTroikaParticipantProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    private readonly DataConnectionClient client = new DataConnectionClient();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        int id =  int.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "id").FirstOrDefault()?.Value ?? "id");
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
                var participantToRemove = gameInstance.Participants[id - 1];
                gameInstance.Participants.Remove(participantToRemove);

                await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);
                result = gameInstance.GetParticipantText(["Initiative"]);
                resultOverall = $"\"{participantToRemove.Name}\" has been removed from the participant list, it now contains {gameInstance.Participants.Count()}.";
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
