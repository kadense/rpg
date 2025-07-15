using Kadense.RPG.Models;

namespace Kadense.RPG.Models;

public class GameFactory<TParent> : GameFactoryBase<TParent, Game>
    where TParent : GameFactoryBase
{
    public GameFactory(TParent parent) : base(parent)
    {
    }
    public GameFactory(TParent parent, string name, string? description) : base(parent)
    {
        Instance = new Game(name, description ?? string.Empty);
    }

    public GameFactory<TParent> WithName(string name)
    {
        Instance.Name = name;
        return this;
    }

    public GameFactory<TParent> WithDescription(string description)
    {
        Instance.Description = description;
        return this;
    }


    public GameFactory<TParent> WithCustomDeck(string name, Func<List<GameCard>> deckCreator)
    {
        if (!Instance.CustomDecks.ContainsKey(name))
        {
            Instance.CustomDecks.Add(name, deckCreator);
        }
        return this;
    }

    public GameEntityFactory<GameFactory<TParent>> WithWorldSection()
    {
        var factory = new GameEntityFactory<GameFactory<TParent>>(this);
        Instance.WorldSection = factory.Instance;
        return factory;
    }

    public GameEntityFactory<GameFactory<TParent>> WithCharacterSection()
    {
        var factory = new GameEntityFactory<GameFactory<TParent>>(this);
        Instance.CharacterSection = factory.Instance;
        return factory;
    }
}
