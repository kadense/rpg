using System.Reflection;

namespace Kadense.RPG.Games;

public partial class GamesFactory : GameBase
{
    [Game("We Three Kings")]
    public GamesFactory AddWeThreeKings()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
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

