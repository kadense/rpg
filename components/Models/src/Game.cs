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
    public List<GameLocation> Locations { get; set; } = new List<GameLocation>();
    public List<GameCharacterLocation> CharacterLocations { get; set; } = new List<GameCharacterLocation>();
    public List<GameCharacter> Characters { get; set; } = new List<GameCharacter>();
    public List<GameItem> Items { get; set; } = new List<GameItem>();
    public List<GameItemLink> ItemLinks { get; set; } = new List<GameItemLink>();
    public List<string> ImagePath { get; set; } = new List<string>();
    public bool IsDynamic { get; set; } = false;

    [JsonIgnore]
    public Dictionary<string, Func<List<GameCard>>> CustomDecks { get; set; } = new Dictionary<string, Func<List<GameCard>>>();

    public List<GameCharacter> GetCharactersForLocation(string locationId)
    {
        var characterIds = CharacterLocations.Where(link => link.LocationId == locationId).Select(link => link.CharacterId).ToList();
        return Characters.Where(c => characterIds.Contains(c.Id)).OrderBy(c => c.Name).ToList();
    }
    public List<GameLocation> GetLocationsForCharacter(string characterId)
    {
        var locationIds = CharacterLocations.Where(link => link.CharacterId == characterId).Select(link => link.LocationId).ToList();
        return Locations.Where(l => locationIds.Contains(l.Id)).OrderBy(c => c.Name).ToList();
    }
    public List<GameItem> GetItemsForCharacter(string id)
    {
        var ids = ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Character && link.LinkedToId == id).Select(link => link.ItemId).ToList();
        return Items.Where(l => ids.Contains(l.Id)).OrderBy(c => c.Name).ToList();
    }
    public List<GameItem> GetItemsForLocation(string id)
    {
        var ids = ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Location && link.LinkedToId == id).Select(link => link.ItemId).ToList();
        return Items.Where(l => ids.Contains(l.Id)).OrderBy(c => c.Name).ToList();
    }
    public List<GameCharacter> GetCharactersForItem(string id)
    {
        var ids = ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Character && link.ItemId == id).Select(link => link.LinkedToId).ToList();
        return Characters.Where(i => ids.Contains(i.Id)).OrderBy(i => i.Name).ToList();
    }
    public List<GameLocation> GetLocationsForItem(string id)
    {
        var ids = ItemLinks.Where(link => link.LinkedToType == GameItemLink.LinkType.Location && link.ItemId == id).Select(link => link.LinkedToId).ToList();
        return Locations.Where(i => ids.Contains(i.Id)).OrderBy(i => i.Name).ToList();
    }
}
