using System.Reflection;

namespace Kadense.RPG.Models;

public partial class GamesFactory : GameBase
{
    [Game("Troika")]
    public GamesFactory AddTroika()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
            .End();
    }
}

