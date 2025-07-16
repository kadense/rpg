using System.Text;
using System.Text.RegularExpressions;
using Discord;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordButtonCommand("troika_add_participants_via_input", "Add the participant")]
public partial class AddParticipantsViaInputProcessor : IDiscordButtonCommandProcessor
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

        if (interaction.Message == null || interaction.Message.Components == null || interaction.Message.Components.Count() == 0)
        {
            logger.LogWarning("No Components");
        }
        else if(interaction.Message.Components.First() is DiscordContainerComponent containerComponent && containerComponent.Components != null)
        {
            var textInputComponent = containerComponent.Components.GetByCustomId<DiscordTextInputComponent>("troika_add_participants_via_input")!;

            foreach(string name in textInputComponent.Value!.Split("\n"))
                if (!gameInstance.Participants.Any(p => p.Name == name))
                    gameInstance.Participants.Add(new GameParticipant
                    {
                        Name = name,
                        Type = "Player"
                    });

            await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);
        }
        else
        {
            logger.LogError("First component is not a populated Container as expected");
        }

        return new DiscordApiResponseContent
        {
            Response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger)
        };        
    }
}
