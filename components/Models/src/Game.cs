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
    public Dictionary<string, Func<List<GameCard>>> CustomDecks { get; set; } = new Dictionary<string, Func<List<GameCard>>>();
}
