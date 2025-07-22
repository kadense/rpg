namespace Kadense.RPG.Models;

public class GameLocationFactory<TParent> : GameFactoryBase<TParent, GameLocation>
    where TParent : GameFactoryBase
{
    public GameLocationFactory(TParent parent, Game gameDetails) : base(parent)
    {
        GameDetails = gameDetails;
    }

    public Game GameDetails { get; set; }

    public GameLocationFactory<TParent> WithDescription(string value)
    {
        this.Instance.Description = value;
        return this;
    }

    public GameLocationFactory<TParent> WithName(string value)
    {
        this.Instance.Name = value;
        return this;
    }

    public GameLocationFactory<TParent> WithImagePath(string value)
    {
        this.Instance.ImagePath.Add(value);
        return this;
    }

    public GameCharacterLocationFactory<GameLocationFactory<TParent>> WithCharacterLink()
    {
        var factory = new GameCharacterLocationFactory<GameLocationFactory<TParent>>(this, GameDetails);
        factory.Location = this.Instance;

        return factory;
    }

    public GameLocationFactory<TParent> WithCharacterLink(string characterId)
    {
        var factory = new GameCharacterLocationFactory<GameLocationFactory<TParent>>(this, GameDetails)
            .WithCharacterId(characterId);
        factory.Location = this.Instance;

        return this;
    }



    public GameLocationFactory<TParent> WithItemLink(string itemId)
    {
        var factory = new GameItemLinkFactory<GameLocationFactory<TParent>>(this, GameDetails)
            .WithItemId(itemId);
        factory.Instance.LinkedToType = GameItemLink.LinkType.Location;
        factory.Location = this.Instance;
        GameDetails.ItemLinks.Add(factory.Instance);
        return this;
    }

    public GameItemLinkFactory<GameLocationFactory<TParent>> WithItemLink()
    {
        var factory = new GameItemLinkFactory<GameLocationFactory<TParent>>(this, GameDetails);
        factory.Location = this.Instance;
        GameDetails.ItemLinks.Add(factory.Instance);
        factory.Instance.LinkedToType = GameItemLink.LinkType.Location;
        return factory;
    }

    public GameLocationAreaFactory<GameLocationFactory<TParent>> WithArea()
    {
        var factory = new GameLocationAreaFactory<GameLocationFactory<TParent>>(this, GameDetails, this.Instance);
        this.Instance.Areas.Add(factory.Instance);
        return factory;
    }
}