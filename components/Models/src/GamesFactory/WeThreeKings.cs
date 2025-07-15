using System.Reflection;

namespace Kadense.RPG.Models;

public partial class GamesFactory : GameFactoryBase
{
    [Game("We Three Kings")]
    public GamesFactory AddWeThreeKings()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithWorldSection()
                    .WithLlmPrompt(@"""
                    I would like a brief narrative introduction for a game set in Judea in roman times. The troops of the nefarious King Herod roam the land causing terror to the people. On the very east of the land are three key settings, in the first first a man wielding his sword surrounded by snakes in a cloud of incense. The second, a sorceror rouses in the remains of a building that has been destroyed by lightning, marking the site of a battle with Herod's men, those that remain are dazed but the danger has not passed for the sorcerer. The third a man is in the custody of herod's men and is about to be buried alive. Describe the locations and the setting. Do not introduce the characters, just describe the setting. Do not give a title or heading. Add random elements for flair.
                    """)
                .End()
                .WithCharacterSection()
                    .WithSelection("Character")
                        .WithChoice("Balthazar, the Ghost-Speaker", "Begins play being buried alive by King Herod's soldiers")
                            .WithAttribute("Fleet foot", "3")
                            .WithAttribute("Strong arm", "4")
                            .WithAttribute("Keen eye", "3")
                            .WithAttribute("Quick tongue", "5")
                            .WithAttribute("Sound mind", "2")
                        .End()
                        .WithChoice("Kaspar, the Sorceror", "Begins play with the roof-top he's fighting on being destroyed by Lightning")
                            .WithAttribute("Fleet foot", "3")
                            .WithAttribute("Strong arm", "2")
                            .WithAttribute("Keen eye", "4")
                            .WithAttribute("Quick tongue", "3")
                            .WithAttribute("Sound mind", "5")
                        .End()
                        .WithChoice("Melchior, the Sword-master", "Begins play being surrounded by giant snakes and clouds of incense")
                            .WithAttribute("Fleet foot", "3")
                            .WithAttribute("Strong arm", "4")
                            .WithAttribute("Keen eye", "3")
                            .WithAttribute("Quick tongue", "5")
                            .WithAttribute("Sound mind", "2")
                        .End()
                    .End()
                .End()
            .End();
    }
}

