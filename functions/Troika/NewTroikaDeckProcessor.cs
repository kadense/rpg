using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.Troika;

[DiscordSlashCommand("troika-new-round", "Creates a new deck of troika cards for initiative!")]
[DiscordSlashCommandOption("players", "How many players?", false, Type = DiscordSlashCommandOptionType.Integer)]
[DiscordSlashCommandOption("enemies", "How many enemy tokens?", false, Type = DiscordSlashCommandOptionType.Integer)]
[DiscordSlashCommandOption("henchmen", "How many henchmen?", false, Type = DiscordSlashCommandOptionType.Integer)]
public class NewTroikaDeckProcessor : IDiscordSlashCommandProcessor
{
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    private readonly DataConnectionClient client = new DataConnectionClient();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        int players = int.Parse((interaction.Data?.Options?.Where(opt => opt.Name == "players").FirstOrDefault()?.Value ?? "0").ToString()!);
        int enemies = int.Parse((interaction.Data?.Options?.Where(opt => opt.Name == "enemies").FirstOrDefault()?.Value ?? "0").ToString()!);
        int henchmen = int.Parse((interaction.Data?.Options?.Where(opt => opt.Name == "henchmen").FirstOrDefault()?.Value ?? "0").ToString()!);

        var deck = new DeckOfCards();
        deck.Name = "Troika Initiative";

        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;
        if (players == 0 && enemies == 0 && henchmen == 0)
        {
            // if nothing specified, then use the list of participants in the channel
            var gameInstance = await client.ReadGameInstanceAsync(guildId, channelId);

            if (gameInstance == null)
            {
                return new DiscordApiResponseContent
                {
                    Response = new DiscordInteractionResponse
                    {
                        Data = new DiscordInteractionResponseData
                        {
                            Content = "No game instance found for this channel. Please create a game first."
                        }
                    }
                };
            }

            if(gameInstance.GameName != "Troika")
            {
                return new DiscordApiResponseContent
                {
                    Response = new DiscordInteractionResponse
                    {
                        Data = new DiscordInteractionResponseData
                        {
                            Content = "This game instance is not a Troika game. Please create a Troika game first."
                        }
                    }
                };
            }

            foreach (var participant in gameInstance.Participants)
            {
                switch(participant.Type?.ToLower())
                {
                    case "player":
                        for (int i = 0; i < 2; i++)
                        {
                            deck.AddCard($"Player: {participant.Name ?? "Unknown"}");
                        }
                        break;
                    case "enemy":
                        var points = participant.Attributes.GetValueOrDefault("Initiative", 1);
                        for(int i = 0; i < points * 2; i++)
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
        }
        else
        {
            for (int p = 0; p < players; p++)
            {
                for (int i = 0; i < 2; i++)
                {
                    deck.AddCard($"Player {p + 1}");
                }
            }

            for (int e = 0; e < enemies; e++)
            {
                deck.AddCard($"Enemy");
            }

            for (int h = 0; h < enemies; h++)
            {
                deck.AddCard($"Henchmen");
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

        
        var embed = new DiscordEmbed
        {
            Title = "New deck of cards created for this channel",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        };


        return new DiscordApiResponseContent
        {
            Response = new DiscordInteractionResponse
            {
                Data = new DiscordInteractionResponseData
                {
                    Content = $"The deck has been refreshed for this channel. It now contains {deck.Count()} cards.",
                    Embeds = new List<DiscordEmbed>() { embed },
                }
            }
        };
    }
}
