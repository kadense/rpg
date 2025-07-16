using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

public static class TroikaResponse
{
    public static DiscordInteractionResponse ListParticipantResponse(string guildId, string channelId, GameInstance gameInstance, ILogger logger, bool useOriginalMessage = true)
    {
        if (gameInstance.GameName != "Troika")
        {
            return GetErrorResponse("This game instance is not a Troika game. Please create a Troika game first.", logger, true).Response!;
        }

        var result = gameInstance.GetParticipantText(["Initiative"]);

        logger.LogInformation($"Participant List: {result}");

        return new DiscordInteractionResponseBuilder()
            .WithResponseType(useOriginalMessage ? DiscordInteractionResponseType.UPDATE_MESSAGE : DiscordInteractionResponseType.CHANNEL_MESSAGE_WITH_SOURCE)
            .WithData()
                .WithFlags(1 << 15)
                .WithContainerComponent()
                    .WithAccentColor(0x00FF00)
                    .WithTextDisplayComponent()
                        .WithContent("Troika Initiative")
                    .End()
                    .WithTextDisplayComponent()
                        .WithContent(result)
                    .End()
                    .WithActionRowComponent()
                        .WithButtonComponent()
                            .WithCustomId("troika_refresh_participant_list")
                            .WithLabel("Refresh")
                        .End()
                    .End()
                    .WithActionRowComponent()
                        .WithUserSelectComponent()
                            .WithCustomId("troika_add_participants_via_select")
                            .WithPlaceholder("Players")
                        .End()
                    .End()
                .End()
            .End()
            .Build();
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