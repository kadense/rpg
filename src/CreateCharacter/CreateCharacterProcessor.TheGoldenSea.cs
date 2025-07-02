using System.Text.RegularExpressions;
using Kadense.Models.Discord;

namespace Kadense.RPG.CreateCharacter;

public class GoldenSeaCharacterPath
{
    public GoldenSeaCharacterPath(string pathName, Dictionary<string, string> abilities)
    {
        PathName = pathName;
        Abilities = abilities;
    }
    public string PathName { get; set; }
    public Dictionary<string, string> Abilities { get; set; }
}

public partial class CreateCharacterProcessor : IDiscordSlashCommandProcessor
{

    public List<DiscordEmbed> CreateTheGoldenSeaCharacter(DiscordInteraction interaction)
    {
        var embeds = new List<DiscordEmbed>();

        var rolls = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            rolls.Add(random.Next(1, 6));
        }
        rolls.Sort();
        rolls.RemoveAt(0); // Remove the lowest roll
        rolls.RemoveAt(rolls.Count - 1); // Remove the highest roll

        var rollsToBeShuffled = rolls.ToArray();
        random.Shuffle(rollsToBeShuffled);

        int physical = rollsToBeShuffled[0];
        int social = rollsToBeShuffled[1];
        int mental = rollsToBeShuffled[2];

        embeds.Add(
            new DiscordEmbed
            {
                Title = "The Golden Sea Character",
                Description = "Generating character for 'The Golden Sea'...",
                Color = 0x00FF00, // Green color
                Fields = new List<DiscordEmbedField>
                {
                    new DiscordEmbedField
                    {
                        Name = "Physical",
                        Value = physical.ToString()
                    },
                    new DiscordEmbedField
                    {
                        Name = "Social",
                        Value = social.ToString()
                    },
                    new DiscordEmbedField
                    {
                        Name = "Mental",
                        Value = mental.ToString()
                    },
                }
            }
        );
        // chose two paths
        for (int i = 0; i < 2; i++)
        {
            var path = new GoldenSeaCharacterPath("Unknown", new Dictionary<string, string>());
            string ability, abilityDescription;
            int pathNumeric = random.Next(1, 8);
            int abilityNumeric = random.Next(1, 3);
            int abilityNumeric2 = random.Next(1, 3);
            while (abilityNumeric == abilityNumeric2)
            {
                abilityNumeric2 = random.Next(1, 3); // ensure different abilities
            }
            int[] abilities = new int[] { abilityNumeric, abilityNumeric2 };

            switch (pathNumeric)
            {
                case 1:
                    path.PathName = "Agent";
                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Crownsman";
                                abilityDescription = "+2 Social when you intimidate";
                                break;
                            case 2:
                                ability = "Silver Tongue";
                                abilityDescription = "+2 Social when you lie";
                                break;

                            default:
                                ability = "Acquisitions Dept.";
                                abilityDescription = "+2 Physical when you steal";
                                break;
                        }

                        path.Abilities.Add(ability, abilityDescription);
                    }
                    break;
                case 2:
                    path.PathName = "Celebrant";
                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Joybringer";
                                abilityDescription = "+2 Social when you spread joy";
                                break;
                            case 2:
                                ability = "Dervish";
                                abilityDescription = "(Mental/Day) Make 3 attacked per turn";
                                break;

                            default:
                                ability = "Performer";
                                abilityDescription = "(Social/Day) Give all allies +1 next round";
                                break;
                        }
                        path.Abilities.Add(ability, abilityDescription);
                    }
                    break;
                case 3:
                    path.PathName = "Defender";
                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Shield-Bearer";
                                abilityDescription = "+1 Physical when you defend the weak";
                                break;
                            case 2:
                                ability = "Untouchable Faith";
                                abilityDescription = "+2 Stress Boxes";
                                break;

                            default:
                                ability = "Sword of the goddess";
                                abilityDescription = "Inflict +1 Stress when you fight";
                                break;
                        }
                        path.Abilities.Add(ability, abilityDescription);

                    }
                    break;
                case 4:
                    path.PathName = "Guilder";
                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Canny";
                                abilityDescription = "+2 Mental when you analyse information";
                                break;
                            case 2:
                                ability = "Cautious";
                                abilityDescription = "Defend with any stat in combat";
                                break;

                            default:
                                ability = "Golden";
                                abilityDescription = "+2 social when you cut a deal";
                                break;
                        }
                        path.Abilities.Add(ability, abilityDescription);
                    }
                    break;
                case 5:
                    path.PathName = "Oracle";
                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Doomsayer";
                                abilityDescription = "(Mental/Day) Attack at range using mental vs mental";
                                break;
                            case 2:
                                ability = "Divinator";
                                abilityDescription = "+2 Mental when you investigate";
                                break;

                            default:
                                ability = "The Sanguine";
                                abilityDescription = "(Mental/Day) Restore Stress equal to your social on another";
                                break;
                        }
                        path.Abilities.Add(ability, abilityDescription);
                    }
                    break;
                case 6:
                    path.PathName = "Persecutor";

                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Sacred Blade";
                                abilityDescription = "+1 Physical when you bring a criminal to justice";
                                break;
                            case 2:
                                ability = "The inevitable";
                                abilityDescription = "+2 Physical, Mental when speaking to an evildoer";
                                break;

                            default:
                                ability = "Martyr";
                                abilityDescription = "(Mental/Day) Mark 1 stress to reroll a d20";
                                break;
                        }
                        path.Abilities.Add(ability, abilityDescription);
                    }
                    break;
                case 7:
                    path.PathName = "Pilgrim";

                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Captain";
                                abilityDescription = "+1 All when aboard your ship";
                                break;
                            case 2:
                                ability = "Shadow";
                                abilityDescription = "+2 Physical when you're being stealthy";
                                break;

                            default:
                                ability = "Well-travelled";
                                abilityDescription = "+2 Social when you arrive in a port";
                                break;
                        }
                        path.Abilities.Add(ability, abilityDescription);
                    }
                    break;
                case 8:
                    path.PathName = "Witness";

                    foreach (var a in abilities)
                    {
                        switch (a)
                        {
                            case 1:
                                ability = "Living Archive";
                                abilityDescription = "+2 Mental when you're recalling history";
                                break;
                            case 2:
                                ability = "Trailblazer";
                                abilityDescription = "+1 All when you're somewhere folks haven't been for 100+ years";
                                break;

                            default:
                                ability = "Wise";
                                abilityDescription = "(Mental/Day) Add social to an ally's roll as you advise them";
                                break;
                        }
                        path.Abilities.Add(ability, abilityDescription);
                    }
                    break;
            }
            embeds.Add(
                new DiscordEmbed
                {
                    Title = $"Path: {path.PathName}",
                    Color = 0x0000FF, // Blue color
                    Fields = path.Abilities
                        .Select(kvp => new DiscordEmbedField
                        {
                            Name = kvp.Key,
                            Value = kvp.Value
                        }).ToList()
                }
            );
        }


        return embeds;
    }
}
