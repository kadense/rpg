using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Kadense.RPG.DataAccess;

namespace Kadense.RPG.CardDeck;

[DiscordSlashCommand("draw", "Draw from a deck of cards!")]
[DiscordSlashCommandOption("cards", "How many cards?", false, Type = DiscordSlashCommandOptionType.Integer)]
[DiscordSlashCommandOption("public", "Draw publicly?", false, Type = DiscordSlashCommandOptionType.Boolean, Choices = ["true","false"])]
public class DrawCardProcessor : IDiscordSlashCommandProcessor
{
    private readonly DataConnectionClient client = new DataConnectionClient();
    private readonly KadenseRandomizer random = new KadenseRandomizer();

    public async Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        int cards = int.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "cards").FirstOrDefault()?.Value ?? "1");
        bool publicDraw = bool.Parse(interaction.Data?.Options?.Where(opt => opt.Name == "public").FirstOrDefault()?.Value ?? "false");


        string guildId = interaction.GuildId ?? interaction.Guild!.Id!;
        string channelId = interaction.ChannelId ?? interaction!.Channel!.Id!;
        //string userName = interaction.User!.Username ?? interaction.User!.GlobalName!.ToString();

        var deck = await client.ReadDeckAsync(guildId, channelId);
        if (deck == null)
        {
            throw new InvalidOperationException("Deck not found. Please create a deck first.");
        }


        var embed = new DiscordEmbed
        {
            Title = $"Drawn card from {deck.Name} Deck",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>()
        };

        var cardsDrawn = deck!.DrawCards(cards);
        bool deckReset = false;
        if (deck.Name == "Troika Initiative")
        {
            publicDraw = true; // Always public draw for Troika Initiative
            if (cardsDrawn.Any(c => c.Name == "End of Round"))
            {
                deck.ResetDeck(random);
                deckReset = true;
            }
        }

        await client.WriteDeckAsync(guildId, channelId, deck);

        if (!publicDraw)
        {
            var followupClient = new DiscordFollowupClient();
            var followupMessage = followupClient.CreateFollowup(guildId, channelId, $"You drew the following card(s): \n - {string.Join("\n - ", cardsDrawn.Select(c => c.Name))}", interaction.Token!, logger);

            return new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponse
                {
                    Data = new DiscordInteractionResponseData
                    {
                        Content = $"Drew {cards} from the deck, {(deckReset ? "the deck has been reset and reshuffled" : $"there are {deck.Count()} remaining in the deck.")}",
                        Embeds = new List<DiscordEmbed>() { embed },
                    }
                },
                FollowupMessage = followupMessage
            };
        }
        else
        {
            embed.Fields.Add(new DiscordEmbedField
            {
                Name = "Drawn Cards",
                Value = string.Join("\n", cardsDrawn.Select(card => $" - {card}"))
            });

            return new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponse
                {
                    Data = new DiscordInteractionResponseData
                    {
                        Content = $"Drew {cards} from the deck, {(deckReset ? "the deck has been reset and reshuffled" : $"there are {deck.Count()} remaining in the deck.")}",
                        Embeds = new List<DiscordEmbed>() { embed },
                    }
                }
            };
        }
    }
}
