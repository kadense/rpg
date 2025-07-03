using Kadense.Models.Discord;
using Kadense.RPG;

namespace Kadense.RPC.Tests;

public class GameCreationTests
{
    [Theory]
    [InlineData("The witch is dead", 2)]
    [InlineData("The witch is dead", 4)]
    [InlineData("The witch is dead", 10)]


    public void Test_TheWitchIsDead(string game, int players)
    {
        var result = TestGameCreation(game, players);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.NotNull(result.Data.Content);
    }

    public DiscordInteractionResponse TestGameCreation(string game, int players)
    {
        var interaction = new DiscordInteraction
        {
            Data = new DiscordInteractionData
            {
                Name = "game",
                Options = new List<DiscordInteractionOptions>
                {
                    new DiscordInteractionOptions
                    {
                        Name = "game",
                        Value = game
                    },
                    new DiscordInteractionOptions
                    {
                        Name = "players",
                        Value = players.ToString()
                    }
                }
            }
        };

        // Create an instance of the processor and execute it
        var processor = new DiscordInteractionProcessor();
        return processor.ExecuteAsync(interaction).Result;
    }
}