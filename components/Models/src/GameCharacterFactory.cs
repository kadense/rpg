namespace Kadense.RPG.Models;

public class GameCharacterFactory<TParent> : GameFactoryBase<TParent, GameCharacter>
    where TParent : GameFactoryBase
{
    public GameCharacterFactory(TParent parent, Game gameDetails) : base(parent)
    {
        GameDetails = gameDetails;
    }

    public Game GameDetails { get; set; }

    public GameCharacterFactory<TParent> WithName(string name)
    {
        this.Instance.Name = name;
        return this;
    }
    public GameCharacterFactory<TParent> WithDescription(string value)
    {
        this.Instance.Description = value;
        return this;
    }
    public GameCharacterFactory<TParent> WithAge(int value)
    {
        this.Instance.Age = value;
        return this;
    }
    public GameCharacterFactory<TParent> WithAgeRange(string value)
    {
        this.Instance.AgeRange = value;
        return this;
    }
    public GameCharacterFactory<TParent> WithDateOfBirth(DateTime value)
    {
        this.Instance.DateOfBirth = value;
        return this;
    }

    public GameCharacterFactory<TParent> WithDied(DateTime value)
    {
        this.Instance.Died = value;
        return this;
    }

    public GameCharacterFactory<TParent> WithDiversityFlag(string value)
    {
        if (this.Instance.DiversityFlags == null)
            this.Instance.DiversityFlags = new List<string>();

        this.Instance.DiversityFlags.Add(value);
        return this;
    }


    public GameCharacterFactory<TParent> WithFactBasedSummary(string value)
    {
        this.Instance.FactBasedSummary = value;
        return this;
    }


    public GameCharacterFactory<TParent> WithImagePath(string value)
    {
        this.Instance.ImagePath.Add(value);
        return this;
    }

    public GameCharacterFactory<TParent> WithPronouns(string value)
    {
        this.Instance.Pronouns = value;
        return this;
    }


    public GameCharacterLocationFactory<GameCharacterFactory<TParent>> WithCharacterLink()
    {
        var factory = new GameCharacterLocationFactory<GameCharacterFactory<TParent>>(this, GameDetails);
        factory.Character = this.Instance;

        if (GameDetails.Locations == null)
            GameDetails.Locations = new List<GameLocation>();

        return factory;
    }

    public GameCharacterFactory<TParent> WithCharacterLink(string locationId)
    {
        var factory = new GameCharacterLocationFactory<GameCharacterFactory<TParent>>(this, GameDetails)
            .WithLocationId(locationId);

        factory.Character = this.Instance;

        if (GameDetails.Characters == null)
            GameDetails.Characters = new List<GameCharacter>();

        return this;
    }
    
    
    public GameCharacterFactory<TParent> WithItemLink(string itemId)
    {
        var factory = new GameItemLinkFactory<GameCharacterFactory<TParent>>(this, GameDetails)
            .WithItemId(itemId);
        factory.Instance.LinkedToType = GameItemLink.LinkType.Character;
        factory.Character = this.Instance;
        GameDetails.ItemLinks.Add(factory.Instance);
        return this;
    }

    public GameItemLinkFactory<GameCharacterFactory<TParent>> WithItemLink()
    {
        var factory = new GameItemLinkFactory<GameCharacterFactory<TParent>>(this, GameDetails);
        factory.Instance.LinkedToType = GameItemLink.LinkType.Character;
        factory.Character = this.Instance;
        GameDetails.ItemLinks.Add(factory.Instance);
        return factory;
    }
}