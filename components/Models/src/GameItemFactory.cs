namespace Kadense.RPG.Models;

public class GameItemFactory<TParent> : GameFactoryBase<TParent, GameItem>
    where TParent : GameFactoryBase
{
    public GameItemFactory(TParent parent, Game gameDetails) : base(parent)
    {
        GameDetails = gameDetails;
    }

    public Game GameDetails { get; set; }

    public GameItemFactory<TParent> WithDescription(string value)
    {
        this.Instance.Description = value;
        return this;
    }

    public GameItemFactory<TParent> WithName(string value)
    {
        this.Instance.Name = value;
        return this;
    }

    public GameItemFactory<TParent> WithImagePath(string value)
    {
        this.Instance.ImagePath.Add(value);
        return this;
    }
    public FileAttachmentFactory<GameItemFactory<TParent>> WithAttachment()
    {
        var factory = new FileAttachmentFactory<GameItemFactory<TParent>>(this);
        this.Instance.Attachments.Add(factory.Instance);
        return factory;
    }
}