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

    [DiscordButtonCommand("add_player_modal", "Displays a modal that will return configure")]
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

    [DiscordButtonCommand("add_enemy_modal", "Displays a modal that will return configure")]
    public async Task<DiscordApiResponseContent> ShowAddEnemyModalAsync(DiscordInteraction interaction, ILogger logger)
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
            Response = TroikaResponse.AddEnemyModal(guildId, channelId, gameInstance!, logger, true)
        };
    }



    [DiscordButtonCommand("add_ally_modal", "Displays a modal that will return configure")]
    public async Task<DiscordApiResponseContent> ShowAddAllyModalAsync(DiscordInteraction interaction, ILogger logger)
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
            Response = TroikaResponse.AddAllyModal(guildId, channelId, gameInstance!, logger, true)
        };
    }





    [DiscordButtonCommand("add_henchmen_modal", "Displays a modal that will return configure")]
    public async Task<DiscordApiResponseContent> ShowAddHenchmenModalAsync(DiscordInteraction interaction, ILogger logger)
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
            Response = TroikaResponse.AddHenchmenModal(guildId, channelId, gameInstance!, logger, true)
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

    [DiscordButtonCommand("add_player", "Adds a player from modal submission")]
    public async Task<DiscordApiResponseContent> AddPlayerAsync(DiscordInteraction interaction, ILogger logger)
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

        var nameComponent = interaction.Data!.Components!.GetByCustomId<DiscordTextInputComponent>("name");

        var playerName = nameComponent!.Value;

        gameInstance.Participants.Add(
            new GameParticipant()
            {
                Name = playerName,
                Type = "Player"
            }
        );

        await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }



    [DiscordButtonCommand("add_enemy", "Adds a player from modal submission")]
    public async Task<DiscordApiResponseContent> AddEnemyAsync(DiscordInteraction interaction, ILogger logger)
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

        var nameComponent = interaction.Data!.Components!.GetByCustomId<DiscordTextInputComponent>("name");
        var initiativeComponent = interaction.Data!.Components!.GetByCustomId<DiscordTextInputComponent>("initiative");

        var playerName = nameComponent!.Value;
        var initiative = initiativeComponent!.Value;

        gameInstance.Participants.Add(
            new GameParticipant()
            {
                Name = playerName,
                Type = "Enemy",
                Attributes = new Dictionary<string, int>()
                {
                    { "Initiative", int.Parse(initiative!)}
                }
            }
        );

        await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }



    [DiscordButtonCommand("add_ally", "Adds a player from modal submission")]
    public async Task<DiscordApiResponseContent> AddAllyAsync(DiscordInteraction interaction, ILogger logger)
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

        var nameComponent = interaction.Data!.Components!.GetByCustomId<DiscordTextInputComponent>("name");

        var playerName = nameComponent!.Value;

        gameInstance.Participants.Add(
            new GameParticipant()
            {
                Name = playerName,
                Type = "Ally"
            }
        );

        await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }
    
    

    [DiscordButtonCommand("add_henchmen", "Adds a player from modal submission")]
    public async Task<DiscordApiResponseContent> AddHenchmenAsync(DiscordInteraction interaction, ILogger logger)
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

        var nameComponent = interaction.Data!.Components!.GetByCustomId<DiscordTextInputComponent>("name");

        var playerName = nameComponent!.Value;

        gameInstance.Participants.Add(
            new GameParticipant()
            {
                Name = playerName,
                Type = "Henchmen"
            }
        );

        await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }
}
