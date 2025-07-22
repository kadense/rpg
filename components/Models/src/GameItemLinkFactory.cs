namespace Kadense.RPG.Models;

public class GameItemLinkFactory<TParent> : GameFactoryBase<TParent, GameItemLink>
    where TParent : GameFactoryBase
{
    public GameItemLinkFactory(TParent parent, Game gameDetails) : base(parent)
    {
        GameDetails = gameDetails;
        if (GameDetails.ItemLinks == null)
            GameDetails.ItemLinks = new List<GameItemLink>();

        GameDetails.ItemLinks.Add(Instance);
    }

    public GameCharacter? Character { get; set; }
    public Game? GameDetails { get; set; }
    public GameLocation? Location { get; set; }
    public GameLocationArea? LocationArea { get; set; }

    public GameItem? Item { get; set; }

    public GameItemLinkFactory<TParent> WithItemId(string value)
    {
        this.Instance.ItemId = value;
        return this;
    }
    public GameItemLinkFactory<TParent> WithCharacterId(string value)
    {
        this.Instance.LinkedToId = value;
        this.Instance.LinkedToType = GameItemLink.LinkType.Character;
        return this;
    }

    public GameItemLinkFactory<TParent> WithLocationId(string value)
    {
        this.Instance.LinkedToId = value;
        this.Instance.LinkedToType = GameItemLink.LinkType.Location;
        return this;
    }

    public GameItemLinkFactory<TParent> WithLocationAreaId(string value)
    {
        this.Instance.LinkedToId = value;
        this.Instance.LinkedToType = GameItemLink.LinkType.Location;
        return this;
    }

    public GameCharacterFactory<GameItemLinkFactory<TParent>> WithCharacter()
    {
        var factory = new GameCharacterFactory<GameItemLinkFactory<TParent>>(this, GameDetails!);
        if (GameDetails!.Characters == null)
            GameDetails!.Characters = new List<GameCharacter>();
        GameDetails!.Characters.Add(factory.Instance);
        Character = factory.Instance;
        return factory;
    }

    
    public GameItemFactory<GameItemLinkFactory<TParent>> WithItem()
    {
        var factory = new GameItemFactory<GameItemLinkFactory<TParent>>(this, GameDetails!);
        if (GameDetails!.Items == null)
            GameDetails!.Items = new List<GameItem>();

        GameDetails!.Items.Add(factory.Instance);
        Item = factory.Instance;
        return factory;
    }

    public GameLocationFactory<GameItemLinkFactory<TParent>> WithLocation()
    {
        var factory = new GameLocationFactory<GameItemLinkFactory<TParent>>(this, GameDetails!);
        if (GameDetails!.Locations == null)
            GameDetails!.Locations = new List<GameLocation>();
        GameDetails!.Locations.Add(factory.Instance);
        Location = factory.Instance;
        return factory;
    }

    public override void FinalizeFactory()
    {
        base.FinalizeFactory();

        if (Item != null)
        {
            Instance.ItemId = Item.Id;
        }

        if (Character != null)
        {
            Instance.LinkedToId = Character.Id;
            Instance.LinkedToType = GameItemLink.LinkType.Character;
        }

        if (Location != null && LocationArea != null)
        {
            Instance.LinkedToId = $"{Location.Id}/{LocationArea.Id}";
            Instance.LinkedToType = GameItemLink.LinkType.LocationArea;
        }
        else if (Location != null)
        {
            Instance.LinkedToId = Location.Id;
            Instance.LinkedToType = GameItemLink.LinkType.Location;
        }
    }
}