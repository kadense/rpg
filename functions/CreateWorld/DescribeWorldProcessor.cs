using System.Text;
using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateWorld;

[DiscordButtonCommand("generate_description", "Describe a world using AI")]
public partial class DescribeWorldProcessor : IDiscordButtonCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();

    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public DiscordApiResponseContent ErrorResponse(string content)
    {
        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponseBuilder()
                .WithData()
                    .WithEmbed()
                        .WithTitle("World Creation")
                        .WithDescription(content)
                        .WithColor(0xFF0000) // Red color
                    .End()
                .End()
                .Build()
        };
    }

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        
        var instance = await client.ReadGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!
        );

        if (instance == null)
            return ErrorResponse("Cannot find LLM Prompt");


        var content = new StringBuilder();
        instance.WorldSetup = content.ToString();
        
        await client.WriteGameInstanceAsync(
            interaction.GuildId ?? interaction.Guild!.Id!,
            interaction.ChannelId ?? interaction.Channel!.Id!,
            instance
        );

        var followupMessage = new DiscordFollowupMessageRequest
        {
            Type = FollowupProcessorType.PublicAiPromptResponse,
            Content = instance.WorldLlmPrompt,
            Token = interaction.Token,
            ChannelId = interaction.ChannelId ?? interaction.Channel!.Id!,
            GuildId = interaction.GuildId ?? interaction.Guild!.Id!,
        };

        return
            new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponseBuilder()
                    .WithResponseType(DiscordInteractionResponseType.UPDATE_MESSAGE)
                    .WithData()
                        .WithFlags(1 << 15)
                        .WithContainerComponent()
                            .WithTextDisplayComponent()
                                .WithContent($"### {instance.GameName} World Creation")
                            .End()
                            .WithTextDisplayComponent()
                                .WithContent(instance.WorldSetup)
                            .End()
                            .WithActionRowComponent()
                                .WithButtonComponent()
                                    .WithLabel("Regenerate World")
                                    .WithCustomId("regenerate_world")
                                    .WithEmoji(new DiscordEmoji { Name = "🌍" })
                                .End()
                                .WithButtonComponent()
                                    .WithLabel("Create Description")
                                    .WithCustomId("generate_description")
                                    .WithEmoji(new DiscordEmoji { Name = "💭" })
                                .End()
                            .End()
                        .End()
                    .End()
                    .Build(),
                    FollowupMessage = followupMessage
            };
    }
}
