using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-initiative", "Setup Troika Initiative.")]
public class TroikaInitiativeProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();
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

    [DiscordButtonCommand("remove_participant_modal", "Displays a modal that will return configure")]
    public async Task<DiscordApiResponseContent> ShowRemoveParticipantModalAsync(DiscordInteraction interaction, ILogger logger)
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
            Response = TroikaResponse.RemoveParticipantModal(guildId, channelId, gameInstance!, logger, true)
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

    [DiscordButtonCommand("remove_npcs", "Remove a participant from modal submission")]
    public async Task<DiscordApiResponseContent> RemoveNPCsAsync(DiscordInteraction interaction, ILogger logger)
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

        var participantsToRemove = gameInstance.Participants.RemoveAll(p => p.Type != "Player");

        await client.WriteGameInstanceAsync(guildId, channelId, gameInstance);

        var response = TroikaResponse.ListParticipantResponse(guildId, channelId, gameInstance!, logger, false);

        await client.WriteDiscordInteractionResponseAsync(guildId, channelId, response);

        return new DiscordApiResponseContent
        {
            Response = response
        };
    }

    [DiscordButtonCommand("remove_participant", "Remove a participant from modal submission")]
    public async Task<DiscordApiResponseContent> RemoveParticipantAsync(DiscordInteraction interaction, ILogger logger)
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

        var participantsToRemove = gameInstance.Participants.RemoveAll(p => p.Name!.ToLowerInvariant() == playerName!.ToLowerInvariant());

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

    [DiscordButtonCommand("new_round", "Prepares the deck from the current participants list")]
    public async Task<DiscordApiResponseContent> NewRoundAsync(DiscordInteraction interaction, ILogger logger)
    {
        var deck = new DeckOfCards();
        deck.Name = "Troika Initiative";

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;
        // if nothing specified, then use the list of participants in the channel
        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);

        if (gameInstance == null)
            return TroikaResponse.GetErrorResponse("No game instance found for this channel. Please create a game first.", logger);

        if (gameInstance.GameName != "Troika")
            return TroikaResponse.GetErrorResponse("This game instance is not a Troika game. Please create a Troika game first.", logger);

        foreach (var participant in gameInstance.Participants)
        {
            switch (participant.Type?.ToLower())
            {
                case "player":
                    for (int i = 0; i < 2; i++)
                    {
                        deck.AddCard($"Player: {participant.Name ?? "Unknown"}");
                    }
                    break;
                case "enemy":
                    var points = participant.Attributes.GetValueOrDefault("Initiative", 1);
                    for (int i = 0; i < points * 2; i++)
                    {
                        deck.AddCard($"Enemy: {participant.Name ?? "Unknown"}");
                    }
                    break;
                case "henchmen":
                    deck.AddCard($"Henchmen: {participant.Name ?? "Unknown"}");
                    break;
                case "ally":
                    deck.AddCard($"Ally: {participant.Name ?? "Unknown"}");
                    break;
                default:
                    deck.AddCard(participant.Name ?? "Unknown");
                    break;
            }
        }

        deck.AddCard($"End of Round");

        if (deck.Cards.Count > 1)
        {
            do
            {
                deck.Shuffle(random);
            } while (deck.Cards.First().Name == "End of Round");
        }
        await client.WriteDeckAsync(guildId, channelId, deck);



        return new DiscordApiResponseContent
        {
            Response = TroikaResponse.NewDeckResponse(guildId, channelId, gameInstance, deck, logger)
        };
    }

    [DiscordButtonCommand("delay_turn", "Place the last token back in the bag and reshuffle")]
    public async Task<DiscordApiResponseContent> DelayTurnAsync(DiscordInteraction interaction, ILogger logger)
    {
        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;
        //string userName = interaction.User!.Username ?? interaction.User!.GlobalName!.ToString();

        var deck = await client.ReadDeckAsync(guildId, channelId);
        if (deck == null)
        {
            throw new InvalidOperationException("Deck not found. Please create a deck first.");
        }


        var cardsDrawn = deck!.UndoLastDrawnCard(random, 1);
        
        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);

        await client.WriteDeckAsync(guildId, channelId, deck);

        return new DiscordApiResponseContent
        {
            Response = TroikaResponse.DelayTurnResponse(guildId, channelId, gameInstance!, deck, cardsDrawn.First(), logger)
        };
    }

    [DiscordButtonCommand("draw_initiative", "Draw from the initiative deck")]
    public async Task<DiscordApiResponseContent> DrawInitiativeAsync(DiscordInteraction interaction, ILogger logger)
    {
        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;
        //string userName = interaction.User!.Username ?? interaction.User!.GlobalName!.ToString();

        var deck = await client.ReadDeckAsync(guildId, channelId);
        if (deck == null)
        {
            throw new InvalidOperationException("Deck not found. Please create a deck first.");
        }


        var cardsDrawn = deck!.DrawCards(1);
        
        var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);

        await client.WriteDeckAsync(guildId, channelId, deck);

        return new DiscordApiResponseContent
        {
            Response = TroikaResponse.DrawInitiativeResponse(guildId, channelId, gameInstance!, deck, cardsDrawn.First(), logger)
        };
    }
}
