using Kadense.Models.Discord;
using Kadense.RPG;
using Microsoft.Extensions.Logging;

namespace Kadense.RPC.Tests;

public class WorldCreationTests
{
    [Fact]
    public async Task Test_TheWitchIsDead()
    {
        var result = await TestCharacterCreation("The witch is dead");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.NotNull(result.Data.Content);
    }

    public async Task<DiscordInteractionResponse> TestCharacterCreation(string game)
    {
        var logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        }).CreateLogger<CharacterCreationTests>();

        var interaction = new DiscordInteraction
        {
            Data = new DiscordInteractionData
            {
                Name = "world",
                Options = new List<DiscordInteractionOptions>
                {
                    new DiscordInteractionOptions
                    {
                        Name = "game",
                        Value = game
                    }
                }
            }
        };

        // Create an instance of the processor and execute it
        var processor = new DiscordInteractionProcessor();
        var result = await processor.ExecuteAsync(interaction, logger);
        return result.Response!;
    }
}