using System.Reflection;

namespace Kadense.RPG.Games;

public partial class GamesFactory : GameBase
{
    [Game("Justified Anxiety")]
    public GamesFactory AddJustifiedAnxiety()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithCharacterSection()
                    .WithSelection("Traits")
                        .SetNumberToChoose(3)
                        .WithNewChoice("Argumentative")
                        .WithNewChoice("Arrogant")
                        .WithNewChoice("Avaricious")
                        .WithNewChoice("Bootlicker")
                        .WithNewChoice("Cowardly")
                        .WithNewChoice("Cruel")
                        .WithNewChoice("Distrustful")
                        .WithNewChoice("Know-it-all")
                        .WithNewChoice("Liar")
                        .WithNewChoice("Scheming")
                        .WithNewChoice("Secretive")
                        .WithNewChoice("Violent")
                    .End()
                    .WithSelection("Secret Society")
                        .SetIsMutuallyExclusive(false)
                        .WithNewChoice("Not applicable") // 1
                        .WithNewChoice("Not applicable") // 2
                        .WithNewChoice("Not applicable") // 3
                        .WithNewChoice("Not applicable") // 4
                        .WithNewChoice("Not applicable") // 5
                        .WithNewChoice("Not applicable") // 6
                        .WithNewChoice("Not applicable") // 7
                        .WithNewChoice("Not applicable") // 8
                        .WithNewChoice("Not applicable") // 9
                        .WithNewChoice("Not applicable") // 10
                        .WithNewChoice("None, join the society of the character on your left")
                        .WithNewChoice("None, join the society of the character on your right")
                        .WithNewChoice("The Psychoactive Congregation")
                        .WithNewChoice("Organised Crime Syndicate")
                        .WithNewChoice("Hack the complex")
                        .WithNewChoice("Anti-robot Alliance")
                        .WithNewChoice("Abberants Uber Alles")
                        .WithNewChoice("Internal Affairs")
                        .WithNewChoice("Project Rockstar")
                        .WithNewChoice("The Escapists")
                    .End()
                    .WithSelection("Abberant Power")
                        .SetIsMutuallyExclusive(false)
                        .WithNewChoice("Not applicable") // 1
                        .WithNewChoice("Not applicable") // 2
                        .WithNewChoice("Not applicable") // 3
                        .WithNewChoice("Not applicable") // 4
                        .WithNewChoice("Not applicable") // 5
                        .WithNewChoice("Not applicable") // 6
                        .WithNewChoice("Not applicable") // 7
                        .WithNewChoice("Not applicable") // 8
                        .WithNewChoice("Not applicable") // 9
                        .WithNewChoice("Not applicable") // 10
                        .WithNewChoice("Regeneration")
                        .WithNewChoice("Chameleon Skin")
                        .WithNewChoice("Read minds")
                        .WithNewChoice("Implant thoughts")
                        .WithNewChoice("Enhanced senses")
                        .WithNewChoice("Erase memories")
                        .WithNewChoice("Start fires")
                        .WithNewChoice("Teleport")
                        .WithNewChoice("Machine curse")
                        .WithNewChoice("Super strength")
                    .End()
                .End()
            .End();
    }
}

