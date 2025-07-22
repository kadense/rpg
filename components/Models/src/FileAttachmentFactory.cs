namespace Kadense.RPG.Models;

public class FileAttachmentFactory<TParent> : GameFactoryBase<TParent, FileAttachment>
    where TParent : GameFactoryBase
{
    public FileAttachmentFactory(TParent parent) : base(parent)
    {

    }

    public FileAttachmentFactory<TParent> WithName(string value)
    {
        this.Instance.Name = value;
        return this;
    }

    public FileAttachmentFactory<TParent> WithDescription(string value)
    {
        this.Instance.Description = value;
        return this;
    }

    public FileAttachmentFactory<TParent> WithPath(string value)
    {
        this.Instance.Path = value;
        return this;
    }
}