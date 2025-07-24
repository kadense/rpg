using System.Text.Json.Serialization;

namespace Kadense.RPG.Models;

public class Game : GameBase
{
    public Game() : base()
    {

    }

    public Game(string name, string description) : base()
    {
        Name = name;
        Description = description;
    }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public GameEntity? CharacterSection { get; set; }
    public GameEntity? WorldSection { get; set; }

    [JsonIgnore]
    public List<GameLocation> Locations { get; set; } = new List<GameLocation>();
    public List<GameCharacterLocation> CharacterLocations { get; set; } = new List<GameCharacterLocation>();

    [JsonIgnore]
    public List<GameCharacter> Characters { get; set; } = new List<GameCharacter>();
    [JsonIgnore]
    public List<GameItem> Items { get; set; } = new List<GameItem>();
    public List<GameItemLink> ItemLinks { get; set; } = new List<GameItemLink>();
    public List<string> ImagePath { get; set; } = new List<string>();
    public bool IsDynamic { get; set; } = false;

    [JsonIgnore]
    public Dictionary<string, Func<List<GameCard>>> CustomDecks { get; set; } = new Dictionary<string, Func<List<GameCard>>>();

    public List<string> GetCharactersForLocation(string locationId)
    {
        return CharacterLocations.Where(link => link.LocationId == locationId).Select(link => link.CharacterId).ToList()!;
    }
    public List<string> GetLocationsForCharacter(string characterId)
    {
        return CharacterLocations.Where(link => link.CharacterId == characterId).Select(link => link.LocationId).ToList()!;
    }
    public List<string> GetItemsForCharacter(string id)
    {
        return ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Character && link.LinkedToId == id).Select(link => link.ItemId).ToList()!;
    }
    public List<string> GetItemsForLocation(string id)
    {
        return ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Location && link.LinkedToId == id).Select(link => link.ItemId).ToList()!;
    }
    public List<string> GetCharactersForItem(string id)
    {
        return ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Character && link.ItemId == id).Select(link => link.LinkedToId).ToList()!;
    }
    public List<string> GetLocationsForItem(string id)
    {
        return ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Location && link.ItemId == id).Select(link => link.LinkedToId).ToList()!;
    }
}
