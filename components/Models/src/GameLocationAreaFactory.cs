namespace Kadense.RPG.Models;

public class GameLocationAreaFactory<TParent> : GameFactoryBase<TParent, GameLocationArea>
    where TParent : GameFactoryBase
{
    public GameLocationAreaFactory(TParent parent, Game gameDetails, GameLocation location) : base(parent)
    {
        GameDetails = gameDetails;
        Location = location;
    }

    public Game GameDetails { get; set; }
    public GameLocation Location { get; set; }
    public GameLocationAreaFactory<TParent> WithDescription(string value)
    {
        this.Instance.Description = value;
        return this;
    }

    public GameLocationAreaFactory<TParent> WithName(string value)
    {
        this.Instance.Name = value;
        return this;
    }

    public GameLocationAreaFactory<TParent> WithImagePath(string value)
    {
        this.Instance.ImagePath.Add(value);
        return this;
    }


    public GameLocationAreaFactory<TParent> WithItemLink(string itemId)
    {
        var factory = new GameItemLinkFactory<GameLocationAreaFactory<TParent>>(this, GameDetails)
            .WithItemId(itemId);
        factory.Instance.LinkedToType = GameItemLink.LinkType.Location;
        factory.LocationArea = this.Instance;
        GameDetails.ItemLinks.Add(factory.Instance);
        return this;
    }

    public GameItemLinkFactory<GameLocationAreaFactory<TParent>> WithItemLink()
    {
        var factory = new GameItemLinkFactory<GameLocationAreaFactory<TParent>>(this, GameDetails);
        factory.LocationArea = this.Instance;
        GameDetails.ItemLinks.Add(factory.Instance);
        factory.Instance.LinkedToType = GameItemLink.LinkType.Location;
        return factory;
    }


    public GameLocationAreaFactory<TParent> WithCharacterLink(string characterId)
    {
        var factory = new GameCharacterLocationFactory<GameLocationAreaFactory<TParent>>(this, GameDetails)
            .WithCharacterId(characterId);
        factory.Location = this.Location;
        factory.Area = this.Instance;
        return this;
    }
    
    
    public GameCharacterLocationFactory<GameLocationAreaFactory<TParent>> WithCharacterLink()
    {
        var factory = new GameCharacterLocationFactory<GameLocationAreaFactory<TParent>>(this, GameDetails);
        factory.Location = this.Location;
        factory.Area = this.Instance;
        return factory;
    }
}