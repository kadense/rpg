using System.Text.RegularExpressions;
using Kadense.Models.Discord;

namespace Kadense.RPG.CreateCharacter;

public partial class CreateCharacterProcessor : IDiscordSlashCommandProcessor
{

    public List<DiscordEmbed> CreateTheWitchIsDeadCharacter(DiscordInteraction interaction)
    {
        var embeds = new List<DiscordEmbed>();

        var speciesNumeric = random.Next(1, 10);
        var spellNumeric = random.Next(1, 10);
        string? species, spell;
        int? clever, fierce, sly, quick; 
        switch (speciesNumeric)
        {
            case 1:
                species = "Fox";
                clever = 2;
                fierce = 2;
                sly = 1;
                quick = 1;
                break;

            case 2:
                species = "Cat";
                clever = 0;
                fierce = 1;
                sly = 3;
                quick = 2;
                break;

            case 3:
                species = "Toad";
                clever = 1;
                fierce = 0;
                sly = 2;
                quick = 1;
                break;

            case 4:
                species = "Spider";
                clever = 2;
                fierce = 0;
                sly = 3;
                quick = 1;
                break;

            case 5:
                species = "Owl";
                clever = 3;
                fierce = 1;
                sly = 1;
                quick = 2;
                break;

            case 6:
                species = "Hare";
                clever = 0;
                fierce = 0;
                sly = 2;
                quick = 3;
                break;

            case 7:
                species = "Magpie";
                clever = 2;
                fierce = 1;
                sly = 1;
                quick = 2;
                break;

            case 8:
                species = "Crow";
                clever = 2;
                fierce = 1;
                sly = 2;
                quick = 1;
                break;

            case 9:
                species = "Dog";
                clever = 1;
                fierce = 3;
                sly = 0;
                quick = 1;
                break;

            case 10:
                species = "Rat";
                clever = 1;
                fierce = 0;
                sly = 2;
                quick = 2;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(species), "Invalid value for speciesNumeric");
        }

        switch(spellNumeric)
        {
            case 1:
                spell = "Unseen Hand";
                break;
                
            case 2:
                spell = "Conjure Light";
                break;

            case 3:
                spell = "Speak Human";
                break;
                
            case 4:
                spell = "Lock / Unlock, Open / Close";
                break;

            case 5:
                spell = "Conjure Dinner";
                break;

            case 6:
                spell = "Make Flame";
                break;

            case 7:
                spell = "Tidy, Clean, and Mend";
                break;

            case 8:
                spell = "Plant Growth";
                break;

            case 9:
                spell = "Distract / Confuse";
                break;

            case 10:
                spell = "Make book read itself out loud";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(spellNumeric), "Invalid value for spellNumeric");
        }

        // Example character creation logic for "The Witch is Dead"
        embeds.Add(new DiscordEmbed
        {
            Title = "The Witch is Dead Character",
            Description = "Generating character for 'The Witch is Dead'...",
            Color = 0x00FF00, // Green color
            Fields = new List<DiscordEmbedField>
            {
                new DiscordEmbedField
                {
                    Name = "Species",
                    Value = species
                },
                new DiscordEmbedField
                {
                    Name = "Clever",
                    Value = clever?.ToString() ?? "0"
                },
                new DiscordEmbedField
                {
                    Name = "Fierce",
                    Value = fierce?.ToString() ?? "0"
                },
                new DiscordEmbedField
                {
                    Name = "Sly",
                    Value = sly?.ToString() ?? "0"
                },
                new DiscordEmbedField
                {
                    Name = "Quick",
                    Value = quick?.ToString() ?? "0"
                },
                new DiscordEmbedField
                {
                    Name = "Spell",
                    Value = spell!
                }
            }
        });

        return embeds;

        
    }
}
