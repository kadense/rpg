using System.Reflection;

namespace Kadense.RPG.Games;

public partial class GamesFactory : GameBase
{
    [Game("The Rapid and The Righteous")]
    public GamesFactory TheRapidAndTheRighteous()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithCharacterSection()
                    .WithSelection("Personality")
                        .WithNewChoice("Wisecracking")
                        .WithNewChoice("Grave")
                        .WithNewChoice("Professional")
                        .WithNewChoice("Easy-going")
                        .WithNewChoice("Quiet")
                        .WithNewChoice("Hot-headed")
                    .End()
                    .WithSelection("Role")
                        .WithNewChoice("Ex-cop")
                        .WithNewChoice("Federal officer")
                        .WithNewChoice("Thief")
                        .WithNewChoice("Smuggler")
                        .WithNewChoice("Fixer")
                        .WithNewChoice("Ex-soldier")
                    .End()
                    .WithRandomAttributeSplitValue(3)
                        .WithRandomAttribute("Drive")
                        .WithRandomAttribute("Fight")
                        .WithRandomAttribute("Hack")
                .End()
            .End();
    }
}

