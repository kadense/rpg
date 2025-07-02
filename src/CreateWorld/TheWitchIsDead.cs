using System.Text.RegularExpressions;
using Kadense.Models.Discord;

namespace Kadense.RPG.CreateWorld;

public partial class CreateWorldProcessor : IDiscordSlashCommandProcessor
{

    public List<DiscordEmbed> CreateTheWitchIsDeadWorld(DiscordInteraction interaction)
    {
        var embeds = new List<DiscordEmbed>();

        var theVillageIsNumeric = random.Next(1, 10);
        var theWitchHunterIsNumeric = random.Next(1, 10);
        var theTwistNumeric = random.Next(1, 10);

        string? theVillageIs, theTwist, theWitchHunterIs;
        switch (theVillageIsNumeric)
        {
            case 1:
                theVillageIs = "Under the thumb of the Baron";
                break;

            case 2:
                theVillageIs = "Filled with cheery Gnomes";
                break;

            case 3:
                theVillageIs = "Controlled by a creepy cult";
                break;

            case 4:
                theVillageIs = "Devolutly religious";
                break;

            case 5:
                theVillageIs = "Incredibly superstitious";
                break;

            case 6:
                theVillageIs = "At war with forest tribes";
                break;

            case 7:
                theVillageIs = "Built around a wizard's college";
                break;

            case 8:
                theVillageIs = "Full of hardy mining folk";
                break;

            case 9:
                theVillageIs = "Shady and dangerous";
                break;

            case 10:
                theVillageIs = "Oppressively perfect";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(theVillageIsNumeric), "Invalid value for theVillageIsNumeric");
        }

        switch (theWitchHunterIsNumeric)
        {
            case 1:
                theWitchHunterIs = "Armoured and tough";
                break;

            case 2:
                theWitchHunterIs = "Wizened and wise";
                break;

            case 3:
                theWitchHunterIs = "Drunk and violent";
                break;

            case 4:
                theWitchHunterIs = "Pios and aggressive";   
                break;

            case 5:
                theWitchHunterIs = "Guarded and cowardly";
                break;

            case 6:
                theWitchHunterIs = "Magical and jealous";
                break;

            case 7:
                theWitchHunterIs = "Clever and cruel";
                break;

            case 8:
                theWitchHunterIs = "Duplicitous and hidden";
                break;

            case 9:
                theWitchHunterIs = "Jolly and well-meaning";
                break;

            case 10:
                theWitchHunterIs = "Headstrong and wild";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(theWitchHunterIs), "Invalid value for theWitchHunterIs");   
        }

        switch (theTwistNumeric)
                {
                    case 1:
                        theTwist = "The village are in on it";
                        break;

                    case 2:
                        theTwist = "A rival witch set her up";
                        break;

                    case 3:
                        theTwist = "The witch-hunter didn't do it";
                        break;

                    case 4:
                        theTwist = "The witch-hunter is waiting for you";
                        break;

                    case 5:
                        theTwist = "The village folk are having a festival";
                        break;

                    case 6:
                        theTwist = "The witch-hunter died, and is being buried";
                        break;

                    case 7:
                        theTwist = "There are two (rival) witch-hunters in town";
                        break;

                    case 8:
                        theTwist = "The village is abandoned";
                        break;

                    case 9:
                        theTwist = "The witch-hunter has dragged a suspected up for interrogation";
                        break;

                    case 10:
                        theTwist = "The village hates him";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(theTwistNumeric), "Invalid value for theTwistNumeric");
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
                    Name = "The village is",
                    Value = theVillageIs!
                },
                new DiscordEmbedField
                {
                    Name = "The witch-hunter is",
                    Value = theWitchHunterIs
                },
                new DiscordEmbedField
                {
                    Name = "The twist is",
                    Value = theTwist
                }
            }
        });

        return embeds;

        
    }
}
