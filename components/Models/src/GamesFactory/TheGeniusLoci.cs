using System.Reflection;

namespace Kadense.RPG.Models;

public partial class GamesFactory : GameFactoryBase
{
    [Game("The Genius Loci")]
    public GamesFactory TheGeniusLoci()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithWorldSection()
                    .WithSelection("The other spirits")
                        .SetNumberToChoose(2)
                        .WithNewChoice("Police Station")
                        .WithNewChoice("Town Hall")
                        .WithNewChoice("Fire Station")
                        .WithNewChoice("Doctor's Office")
                        .WithNewChoice("War Memorial")
                        .WithNewChoice("Forest")
                    .End()
                .End()
                .WithCharacterSection()
                    .WithSelection("Your State")
                        .WithNewChoice("Old")
                        .WithNewChoice("New")
                        .WithNewChoice("Broken")
                        .WithNewChoice("Vibrant")
                        .WithNewChoice("Dangerous")
                        .WithNewChoice("Beautiful")
                    .End()
                    .WithSelection("What you are")
                        .WithNewChoice("Pub")
                        .WithNewChoice("Post Office")
                        .WithNewChoice("School")
                        .WithNewChoice("Park")
                        .WithNewChoice("Corner Shop")
                        .WithNewChoice("Church")
                    .End()
                    .WithSelection("You're scared of")
                        .WithNewChoice("Fire")
                        .WithNewChoice("Strangers")
                        .WithNewChoice("Your inhabitants")
                        .WithNewChoice("Crime")
                        .WithNewChoice("The gods of river and rain")
                        .WithNewChoice("Being forgotten")
                    .End()
                    .WithSelection("What you love (to the point of addiction)")
                        .WithNewChoice("Young people")
                        .WithNewChoice("Photographs")
                        .WithNewChoice("Cider")
                        .WithNewChoice("Pop music")
                        .WithNewChoice("Storms")
                        .WithNewChoice("Prey animals")
                    .End()
                .End()
            .End();
    }
}

