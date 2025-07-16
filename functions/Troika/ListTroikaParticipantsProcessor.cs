using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-list-participants", "List the participants in the Troika game.")]
public class ListTroikaParticipantsProcessor : IDiscordSlashCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();

    [DiscordButtonCommand("refresh_participants", "Refresh the participant list")]
    public async Task<DiscordApiResponseContent> RefreshParticipantsAsync(DiscordInteraction interaction, ILogger logger)
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

        return new DiscordApiResponseContent
        {
            Response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, true)
        };
    }

    [DiscordButtonCommand("add_participant_modal", "Displays a modal that will return configure")]
    public async Task<DiscordApiResponseContent> ShowAddParticipantModalAsync(DiscordInteraction interaction, ILogger logger)
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

        return new DiscordApiResponseContent
        {
            Response = TroikaResponse.AddNonDiscordPlayerModal(guildId, channelId, gameInstance!, logger, true)
        };
    }

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
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

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }

    [DiscordButtonCommand("add_participant", "Adds a participant from modal submission")]
    public async Task<DiscordApiResponseContent> AddParticipantAsync(DiscordInteraction interaction, ILogger logger)
    {
        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;

        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);

        if (gameInstance == null || gameInstance.GameName == null)
        {
            gameInstance = new GameInstance()
            {
                GameName = "Troika"
            };
        }
        
        await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }
}
