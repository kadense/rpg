using System.Reflection;

namespace Kadense.RPG.Models;

public partial class GamesFactory : GameBase
{
    [Game("The Golden Sea")]
    public GamesFactory AddTheGoldenSea()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithWorldSection()
                    .WithLlmPrompt(@"""
                        I would like a brief introduction for a game set in a dystopian future where the world has become a desert world. A once great city, the size of a small country with sky scrapers now lays in ruins after the sands have consumed all but the tallest buildings. The remaining humans have regressed to technology akin to Asia in the 1300s. Beneath the sands lays the realm of the three goddesses, who send omens and crystal miracles up to the surface world. The land is fraught with danger whether it be animals, bandits, warring religious zealots or knights of neighbouring kingdoms. Draw attention to a towering building known as the Maiden's hand, the HQ of the crown. Describe the setting, add random elements for flair. Do not add a title or header.
                    """)
                .End()
                .WithCharacterSection()
                    .WithRandomAttribute("Physical")
                    .WithRandomAttribute("Social")
                    .WithRandomAttribute("Mental")
                    .WithSelection("Path")
                        .SetNumberToChoose(2)
                        .WithChoice("Agent")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Crownsman", "+2 Social when you intimidate")
                                .WithNewChoice("Silver Tongue", "+2 Social when you lie")
                                .WithNewChoice("Acquisitions Dept.", "+2 Physical when you steal")
                            .End()
                        .End()
                        .WithChoice("Celebrant")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Joybringer", "+2 Social when you spread joy")
                                .WithNewChoice("Dervish", "(Mental/Day) Make 3 attacked per turn")
                                .WithNewChoice("Performer", "(Social/Day) Give all allies +1 next round")
                            .End()
                        .End()
                        .WithChoice("Defender")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Shield-Bearer", "+1 Physical when you defend the weak")
                                .WithNewChoice("Untouchable Faith", "+2 Stress Boxes")
                                .WithNewChoice("Sword of the goddess", "Inflict +1 Stress when you fight")
                            .End()
                        .End()
                        .WithChoice("Guilder")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Canny", "+2 Mental when you analyse information")
                                .WithNewChoice("Cautious", "Defend with any stat in combat")
                                .WithNewChoice("Golden", "+2 social when you cut a deal")
                            .End()
                        .End()
                        .WithChoice("Oracle")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Doomsayer", "(Mental/Day) Attack at range using mental vs mental")
                                .WithNewChoice("Divinator", "+2 Mental when you investigate")
                                .WithNewChoice("The Sanguine", "(Mental/Day) Restore Stress equal to your social on another")
                            .End()
                        .End()
                        .WithChoice("Persecutor")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Sacred Blade", "+1 Physical when you bring a criminal to justice")
                                .WithNewChoice("The inevitable", "+2 Physical, Mental when speaking to an evildoer")
                                .WithNewChoice("Martyr", "(Mental/Day) Mark 1 stress to reroll a d20")
                            .End()
                        .End()
                        .WithChoice("Pilgrim")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Captain", "+1 All when aboard your ship")
                                .WithNewChoice("Shadow", "+2 Physical when you're being stealthy")
                                .WithNewChoice("Well-travelled", "+2 Social when you arrive in a port")
                            .End()
                        .End()
                        .WithChoice("Witness")
                            .WithSelection("Ability")
                                .SetColor(0xFFFF00) // Yellow
                                .SetNumberToChoose(2)
                                .WithNewChoice("Living Archive", "+2 Mental when you're recalling history")
                                .WithNewChoice("Trailblazer", "+1 All when you're somewhere folks haven't been for 100+ years")
                                .WithNewChoice("Wise", "(Mental/Day) Add social to an ally's roll as you advise them")
                            .End()
                        .End()
                    .End()
                .End()
            .End();
    }
}

