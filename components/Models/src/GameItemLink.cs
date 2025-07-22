namespace Kadense.RPG.Models;

public class GameItemLink : GameBase
{
    public enum LinkType
    {
        Location,
        LocationArea,
        Character,
        Item
    }

    public string? ItemId { get; set; }

    public LinkType? LinkedToType { get; set; }
    
    public string? LinkedToId { get; set; }
}