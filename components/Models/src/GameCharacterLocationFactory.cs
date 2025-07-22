using System.Formats.Asn1;

namespace Kadense.RPG.Models;

public class GameCharacterLocationFactory<TParent> : GameFactoryBase<TParent, GameCharacterLocation>
    where TParent : GameFactoryBase
{
    public GameCharacterLocationFactory(TParent parent, Game gameDetails) : base(parent)
    {
        GameDetails = gameDetails;
        if (GameDetails.CharacterLocations == null)
            GameDetails.CharacterLocations = new List<GameCharacterLocation>();

        GameDetails.CharacterLocations.Add(Instance);
    }

    public GameCharacter? Character { get; set; }
    public Game? GameDetails { get; set; }
    public GameLocation? Location { get; set; }
    public GameLocationArea? Area { get; set; }

    public GameCharacterLocationFactory<TParent> WithCharacterId(string value)
    {
        this.Instance.CharacterId = value;
        return this;
    }

    public GameCharacterLocationFactory<TParent> WithLocationId(string value)
    {
        this.Instance.LocationId = value;
        return this;
    }

    public GameCharacterFactory<GameCharacterLocationFactory<TParent>> WithCharacter()
    {
        var factory = new GameCharacterFactory<GameCharacterLocationFactory<TParent>>(this, GameDetails!);
        if (GameDetails!.Characters == null)
            GameDetails!.Characters = new List<GameCharacter>();
        GameDetails!.Characters.Add(factory.Instance);
        Character = factory.Instance;
        return factory;
    }

    public GameLocationFactory<GameCharacterLocationFactory<TParent>> WithLocation()
    {
        var factory = new GameLocationFactory<GameCharacterLocationFactory<TParent>>(this, GameDetails!);
        if (GameDetails!.Locations == null)
            GameDetails!.Locations = new List<GameLocation>();
        GameDetails!.Locations.Add(factory.Instance);
        Location = factory.Instance;
        return factory;
    }

    public override void FinalizeFactory()
    {
        base.FinalizeFactory();

        if (Character != null)
            Instance.CharacterId = Character.Id;

        if (Location != null)
            Instance.LocationId = Location.Id;

        if (Area != null)
            Instance.AreaId = Area.Id;
    }
}