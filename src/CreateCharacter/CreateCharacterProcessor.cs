﻿using System.Text.RegularExpressions;
using Kadense.Models.Discord;
using Kadense.RPG.Games;
using Microsoft.Extensions.Logging;

namespace Kadense.RPG.CreateCharacter;

[DiscordSlashCommand("character", "Create a random character")]
[DiscordSlashCommandOption("game", "Which Game?", true, AutoChoices = DiscordSlashCommandChoicesMethod.GamesWithWorlds)]
public partial class CreateCharacterProcessor : IDiscordSlashCommandProcessor
{

    private readonly Random random = new Random();

    public Task<DiscordApiResponseContent> ExecuteAsync(DiscordInteraction interaction, ILogger logger)
    {
        var games = new GamesFactory()
            .EndGames();

        string game = interaction.Data?.Options?.Where(opt => opt.Name == "game").FirstOrDefault()?.Value ?? "1d6";

        var embeds = new List<DiscordEmbed>();


        var matchingGames = games.Where(x => x.Name.ToLowerInvariant() == game.ToLowerInvariant()).ToList();
        if (matchingGames.Count > 0)
        {
            var selectedGame = matchingGames.First();
            var fields = new List<DiscordEmbedField>();
            selectedGame.CharacterSection!.AddFields(fields, random);

            embeds.Add(new DiscordEmbed
            {
                Title = $"Character Creation for {selectedGame.Name}",
                Description = $"Generating character for {game}...",
                Fields = fields,
            });
        }



        return Task.FromResult(
            new DiscordApiResponseContent
            {
                Response = new DiscordInteractionResponse
                {
                    Data = new DiscordInteractionResponseData
                    {
                        Embeds = embeds,
                        Content = $"Generating character for {game}...",
                    }
                }
            }
        );
    }
}
