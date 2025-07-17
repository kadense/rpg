using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

public static class TroikaResponse
{
    public static DiscordInteractionResponse AddNonDiscordPlayerModal(string guildId, string channelId, GameInstance gameInstance, ILogger logger, bool useOriginalMessage = true)
    {
        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(DiscordInteractionResponseType.MODAL)
            .WithData()
                .WithTitle("Add Player")
                .WithCustomId("troika-initiative:add_player")
                .WithActionRowComponent()
                    .WithTextInputComponent()
                        .WithCustomId("name")
                        .WithLabel("Character Name")
                        .WithPlaceholder("John Smith")
                        .WithStyle(1)
                        .WithMinLength(1)
                    .End()
                .End()
            .End()
            .Build();

        return response;
    }

    public static DiscordInteractionResponse AddAllyModal(string guildId, string channelId, GameInstance gameInstance, ILogger logger, bool useOriginalMessage = true)
    {
        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(DiscordInteractionResponseType.MODAL)
            .WithData()
                .WithTitle("Add Ally")
                .WithCustomId("troika-initiative:add_ally")
                .WithActionRowComponent()
                    .WithTextInputComponent()
                        .WithCustomId("name")
                        .WithLabel("Ally Name")
                        .WithPlaceholder("Robin")
                        .WithStyle(1)
                        .WithMinLength(1)
                    .End()
                .End()
            .End()
            .Build();

        return response;
    }

    public static DiscordInteractionResponse RemoveParticipantModal(string guildId, string channelId, GameInstance gameInstance, ILogger logger, bool useOriginalMessage = true)
    {
        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(DiscordInteractionResponseType.MODAL)
            .WithData()
                .WithTitle("Remove Participant")
                .WithCustomId("troika-initiative:remove_participant")
                .WithActionRowComponent()
                    .WithTextInputComponent()
                        .WithCustomId("name")
                        .WithLabel("Name")
                        .WithPlaceholder("Jeff")
                        .WithStyle(1)
                        .WithMinLength(1)
                    .End()
                .End()
            .End()
            .Build();

        return response;
    }

    public static DiscordInteractionResponse AddHenchmenModal(string guildId, string channelId, GameInstance gameInstance, ILogger logger, bool useOriginalMessage = true)
    {
        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(DiscordInteractionResponseType.MODAL)
            .WithData()
                .WithTitle("Add Henchmen")
                .WithCustomId("troika-initiative:add_henchmen")
                .WithActionRowComponent()
                    .WithTextInputComponent()
                        .WithCustomId("name")
                        .WithLabel("Name")
                        .WithPlaceholder("Igor")
                        .WithStyle(1)
                        .WithMinLength(1)
                    .End()
                .End()
            .End()
            .Build();

        return response;
    }


    public static DiscordInteractionResponse AddEnemyModal(string guildId, string channelId, GameInstance gameInstance, ILogger logger, bool useOriginalMessage = true)
    {
        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(DiscordInteractionResponseType.MODAL)
            .WithData()
                .WithTitle("Add Enemy")
                .WithCustomId("troika-initiative:add_enemy")
                .WithActionRowComponent()
                    .WithTextInputComponent()
                        .WithCustomId("name")
                        .WithLabel("Type / Name")
                        .WithPlaceholder("Owl")
                        .WithStyle(1)
                        .WithMinLength(1)
                    .End()
                .End()
                .WithActionRowComponent()
                    .WithTextInputComponent()
                        .WithCustomId("initiative")
                        .WithLabel("Initiative")
                        .WithPlaceholder("2")
                        .WithStyle(1)
                        .WithMinLength(1)
                        .WithValue("2")
                    .End()
                .End()
            .End()
            .Build();

        return response;
    }


    public static DiscordInteractionResponse NewDeckResponse(string guildId, string channelId, GameInstance gameInstance, DeckOfCards deck, ILogger logger, bool useOriginalMessage = true)
    {
        if (gameInstance.GameName != "Troika")
        {
            return GetErrorResponse("This game instance is not a Troika game. Please create a Troika game first.", logger, true).Response!;
        }


        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(useOriginalMessage ? DiscordInteractionResponseType.UPDATE_MESSAGE : DiscordInteractionResponseType.CHANNEL_MESSAGE_WITH_SOURCE)
            .WithData()
                .WithFlags(1 << 15)
                .WithContainerComponent()
                    .WithAccentColor(0x00FF00)
                    .WithTextDisplayComponent()
                        .WithContent("## Troika Deck")
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent($"A new deck with {deck.Count()} cards has been created and shuffled.")
                    .End()
                    .WithActionRowComponent()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:refresh_participants")
                            .WithLabel("Configure Deck")
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "💪"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:draw_initiative")
                            .WithLabel("Draw")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🎴"
                                }
                            )
                        .End()
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent("*Kadense-RPG is an independent production by Kadense Limited and is not affiliated with the Melsonian Arts Council*")
                    .End()
                .End()
            .End()
            .Build();

        logger.LogInformation($"Response: {response}");
        return response;
    }

    
    public static DiscordInteractionResponse DelayTurnResponse(string guildId, string channelId, GameInstance gameInstance, DeckOfCards deck, GameCard drawnCard, ILogger logger, bool useOriginalMessage = true)
    {
        if (gameInstance.GameName != "Troika")
        {
            return GetErrorResponse("This game instance is not a Troika game. Please create a Troika game first.", logger, true).Response!;
        }


        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(useOriginalMessage ? DiscordInteractionResponseType.UPDATE_MESSAGE : DiscordInteractionResponseType.CHANNEL_MESSAGE_WITH_SOURCE)
            .WithData()
                .WithFlags(1 << 15)
                .WithContainerComponent()
                    .WithAccentColor(0x00FF00)
                    .WithTextDisplayComponent()
                        .WithContent("## Troika Initiative")
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent($"***{drawnCard}*** has been returned to the deck and the {deck.Count()} cards have been reshuffled.")
                    .End()
                    .WithActionRowComponent()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:refresh_participants")
                            .WithLabel("End Round")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🏁"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:draw_initiative")
                            .WithLabel("Draw")
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🎴"
                                }
                            )
                        .End()
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent("*Kadense-RPG is an independent production by Kadense Limited and is not affiliated with the Melsonian Arts Council*")
                    .End()
                .End()
            .End()
            .Build();

        logger.LogInformation($"Response: {response}");
        return response;
    }

    public static DiscordInteractionResponse DrawInitiativeResponse(string guildId, string channelId, GameInstance gameInstance, DeckOfCards deck, GameCard drawnCard, ILogger logger, bool useOriginalMessage = true)
    {
        if (gameInstance.GameName != "Troika")
        {
            return GetErrorResponse("This game instance is not a Troika game. Please create a Troika game first.", logger, true).Response!;
        }


        var builder = new DiscordInteractionResponseBuilder()
            .WithResponseType(useOriginalMessage ? DiscordInteractionResponseType.UPDATE_MESSAGE : DiscordInteractionResponseType.CHANNEL_MESSAGE_WITH_SOURCE)
            .WithData()
                .WithFlags(1 << 15)
                .WithContainerComponent()
                    .WithAccentColor(0x00FF00)
                    .WithTextDisplayComponent()
                        .WithContent($"## You've drawn {drawnCard.Name}")
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent(drawnCard.Name == "End of Round" ? $"The round has ended" : $"There are {deck.Count()} remaining in the deck")
                    .End()
                    .WithActionRowComponent()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:refresh_participants")
                            .WithLabel("End Round")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🏁"
                                }
                            )
                        .End();
        if (drawnCard.Name != "End of Round")
        {
            builder
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:delay_turn")
                            .WithLabel("Delay Turn")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "⏸️"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:draw_initiative")
                            .WithLabel("Draw Again")
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🎴"
                                }
                            )
                        .End();
        }
        var response = builder
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent("*Kadense-RPG is an independent production by Kadense Limited and is not affiliated with the Melsonian Arts Council*")
                    .End()
                .End()
            .End()
            .Build();

        logger.LogInformation($"Response: {response}");
        return response;
    }


    public static DiscordInteractionResponse ListParticipantResponse(string guildId, string channelId, GameInstance gameInstance, ILogger logger, bool useOriginalMessage = true)
    {
        if (gameInstance.GameName != "Troika")
        {
            return GetErrorResponse("This game instance is not a Troika game. Please create a Troika game first.", logger, true).Response!;
        }

        var result = gameInstance.GetParticipantText(["Initiative"]);


        var response = new DiscordInteractionResponseBuilder()
            .WithResponseType(useOriginalMessage ? DiscordInteractionResponseType.UPDATE_MESSAGE : DiscordInteractionResponseType.CHANNEL_MESSAGE_WITH_SOURCE)
            .WithData()
                .WithFlags(1 << 15)
                .WithContainerComponent()
                    .WithAccentColor(0x00FF00)
                    .WithTextDisplayComponent()
                        .WithContent("## Players")
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent(result)
                    .End()
                    .WithActionRowComponent()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:add_player_modal")
                            .WithLabel("Add Player")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "👤"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:add_ally_modal")
                            .WithLabel("Add Ally")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "💪"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:add_enemy_modal")
                            .WithLabel("Add Enemy")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "👹"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:add_henchmen_modal")
                            .WithLabel("Add Henchmen")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "👺"
                                }
                            )
                        .End()
                    .End()
                    .WithActionRowComponent()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:remove_participant_modal")
                            .WithLabel("Remove")
                            .WithStyle(2) // danger
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "❌"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:remove_npcs")
                            .WithLabel("Remove NPCs")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🤖"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-initiative:new_round")
                            .WithLabel("New Round")
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🎲"
                                }
                            )
                        .End()
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent("*Kadense-RPG is an independent production by Kadense Limited and is not affiliated with the Melsonian Arts Council*")
                    .End()
                .End()
            /* 
            .WithContainerComponent()
                .WithTextDisplayComponent()
                    .WithContent("Add Player from Discord:")
                .End()
                .WithActionRowComponent()
                    .WithUserSelectComponent()
                        .WithCustomId("troika_add_participants_via_select")
                        .WithPlaceholder("Players")
                    .End()
                .End()
            .End() 
            */
            .End()
            .Build();

        logger.LogInformation($"Response: {response}");
        return response;
    }

    public static DiscordApiResponseContent GetErrorResponse(string errorText, ILogger logger, bool useOriginalMessage = false)
    {
        logger.LogError(errorText);
        return new DiscordApiResponseContent()
        {
            Response = new DiscordInteractionResponseBuilder()
                .WithResponseType(useOriginalMessage ? DiscordInteractionResponseType.UPDATE_MESSAGE : DiscordInteractionResponseType.CHANNEL_MESSAGE_WITH_SOURCE)
                .WithData()
                    .WithFlags(1 << 15)
                    .WithContainerComponent()
                        .WithTextDisplayComponent()
                            .WithContent(errorText)
                        .End()
                        .WithAccentColor(0xFF0000)
                    .End()
                .End()
                .Build()
        };
    }

}