using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordButtonCommand("troika_add_participants_via_select", "Add the participant")]
public partial class AddParticipantsViaSelectProcessor : IDiscordButtonCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();


    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        logger.LogInformation("Loading game information from persistent storage");

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;

        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);


        if (gameInstance == null || gameInstance.GameName == null)
        {
            gameInstance = new GameInstance()
            {
                GameName = "Troika"
            };

            await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);
        }

        if (interaction.Data == null || interaction.Data.Resolved == null || interaction.Data.Resolved.Users == null || interaction.Data.Resolved.Users.Count() == 0)
        {
            logger.LogWarning("No users selected");
        }
        else
        {
            interaction.Data.Resolved.Users.ToList().ForEach(kv =>
            {
                if (!gameInstance.Participants.Any(p => p.Id == kv.Key))
                    gameInstance.Participants.Add(new GameParticipant
                    {
                        Id = kv.Key,
                        Name = kv.Value.Nick ?? kv.Value.GlobalName ?? kv.Value.Username,
                        Type = "Player"
                    });
            });
            await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);
        }

        return new DiscordApiResponseContent
            {
                Response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger)
            };        
    }
}
