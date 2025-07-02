using Kadense.Models.Discord;
using Kadense.RPG;

namespace Kadense.RPC.Tests;

public class CharacterCreationTests
{
    [Fact]
    public void Test_TheWitchIsDead()
    {
        var result = TestCharacterCreation("The witch is dead");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.NotNull(result.Data.Content);
    }
    [Fact]
    public void Test_TheGoldenSea()
    {
        var result = TestCharacterCreation("The golden sea");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.NotNull(result.Data.Content);
    }

    public DiscordInteractionResponse TestCharacterCreation(string game)
    {
        var interaction = new DiscordInteraction
        {
            Data = new DiscordInteractionData
            {
                Name = "character",
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
        return processor.ExecuteAsync(interaction).Result;
    }
}