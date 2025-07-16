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
                .WithCustomId("troika-list-participants:add_participant")
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
                            .WithCustomId("troika-list-participants:refresh_participants")
                            .WithLabel("Refresh")
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "🔃"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-list-participants:add_participant_modal")
                            .WithLabel("Add Participant")
                            .WithStyle(2)
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "👤"
                                }
                            )
                        .End()
                        .WithButtonComponent()
                            .WithCustomId("troika-list-participants:remove_participant_modal")
                            .WithLabel("Remove Participant")
                            .WithStyle(4) // danger
                            .WithEmoji(
                                new DiscordEmoji()
                                {
                                    Name = "❌"
                                }
                            )
                        .End()
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