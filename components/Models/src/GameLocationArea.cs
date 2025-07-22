namespace Kadense.RPG.Models;

public class GameLocationArea : GameBase
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public List<string> ImagePath { get; set; } = new List<string>();
}