using Kadense.Models.Discord;
using Kadense.RPG;
using Microsoft.Extensions.Logging;

namespace Kadense.RPC.Tests;

public class RollDiceTests
{
    [Fact]
    public async Task Test_d6()
    {
        var result = await Test_RollDice("d6");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.Equal("Rolling 1d6", result.Data.Embeds[0].Description);
        Assert.NotNull(result.Data.Content);
    }

    [Fact]
    public async Task Test_1d6()
    {
        var result = await Test_RollDice("1d6");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.Equal("Rolling 1d6", result.Data.Embeds[0].Description);
        Assert.NotNull(result.Data.Content);
    }

    

    [Fact]
    public async Task Test_2d20()
    {
        var result = await Test_RollDice("2d20");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.Equal("Rolling 2d20", result.Data.Embeds[0].Description);
        Assert.NotNull(result.Data.Content);
    }

    
    [Fact]
    public async Task Test_1d6plus1()
    {
        var result = await Test_RollDice("1d6+1");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.Equal("Rolling 1d6 + 1", result.Data.Embeds[0].Description);
        Assert.NotNull(result.Data.Content);
    }

    
    [Fact]
    public async Task Test_1d6plus1plus2()
    {
        var result = await Test_RollDice("1d6+1+2");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.Equal("Rolling 1d6 + 3", result.Data.Embeds[0].Description);
        Assert.NotNull(result.Data.Content);
    }

    
    [Fact]
    public async Task Test_1d6plus1minus2()
    {
        var result = await Test_RollDice("1d6+1-2");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.Equal("Rolling 1d6 - 1", result.Data.Embeds[0].Description);
        Assert.NotNull(result.Data.Content);
    }
    

    
    [Fact]
    public async Task Test_1d6minus1()
    {
        var result = await Test_RollDice("1d6-1");
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Embeds);
        Assert.NotEmpty(result.Data.Embeds);
        Assert.Equal("Rolling 1d6 - 1", result.Data.Embeds[0].Description);
        Assert.NotNull(result.Data.Content);
    }

    public async Task<DiscordInteractionResponse> Test_RollDice(string whatToRoll)
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
                Name = "roll",
                Options = new List<DiscordInteractionOptions>
                {
                    new DiscordInteractionOptions
                    {
                        Name = "whattoroll",
                        Value = whatToRoll
                    }
                }
            }
        };

        var processor = new DiscordInteractionProcessor();
        var result = await processor.ExecuteAsync(interaction, logger);
        return result.Response!;
    }
}