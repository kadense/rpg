namespace Kadense.RPG.Models;

public class Game<T> : GameBase<T>
    where T : GameBase
{
    public Game(T parent, string name, string description) : base(parent)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public GameEntity<Game<T>>? CharacterSection { get; set; }
    public GameEntity<Game<T>>? WorldSection { get; set; }
    public Dictionary<string, Func<List<GameCard>>> CustomDecks { get; set; } = new Dictionary<string, Func<List<GameCard>>>();
    public GameEntity<Game<T>> WithCharacterSection()
    {
        if (CharacterSection == null)
        {
            CharacterSection = new GameEntity<Game<T>>(this);
        }
        return CharacterSection!;
    }

    public Game<T> WithCustomDeck(string name, Func<List<GameCard>> deckCreator)
    {
        if (!CustomDecks.ContainsKey(name))
        {
            CustomDecks.Add(name, deckCreator);
        }
        return this;
    }

    public GameEntity<Game<T>> WithWorldSection()
    {
        if (WorldSection == null)
        {
            WorldSection = new GameEntity<Game<T>>(this);
        }
        return WorldSection!;
    }
}
