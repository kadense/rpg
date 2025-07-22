namespace Kadense.RPG.Models;

public class GameLocation : GameBase
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public List<string> ImagePath { get; set; } = new List<string>();

    public List<GameLocationArea> Areas { get; set; } = new List<GameLocationArea>();
}