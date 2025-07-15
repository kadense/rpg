namespace Kadense.RPG.Models;

public abstract class GameBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

