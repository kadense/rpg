namespace Kadense.RPG.Models;

public abstract class GameBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public List<string> Annotations { get; set; } = new List<string>();
}

