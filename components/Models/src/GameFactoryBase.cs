namespace Kadense.RPG.Models;

public abstract class GameFactoryBase
{
    public abstract void AddChild(GameFactoryBase child);

    public virtual void FinalizeFactory()
    {

    }
}

public abstract class GameFactoryBase<TParent, T> : GameFactoryBase
    where TParent : GameFactoryBase
    where T : GameBase
{
    public GameFactoryBase(TParent parent)
    {
        this.Parent = parent;
        Parent.AddChild(this);
    }

    public TParent Parent { get; set; }

    public override void AddChild(GameFactoryBase child)
    {
        Parent.AddChild(child);
    }

    public T Instance { get; set; } = Activator.CreateInstance<T>();

    public virtual TParent End()
    {
        return Parent;
    }

    public TReturn WithAnnotation<TReturn>(string value)
        where TReturn : GameFactoryBase<TParent, T>
    {
        Instance.Annotations.Add(value);
        return (TReturn)this;
    }

    public GameFactoryBase<TParent, T> WithId(string value)
    {
        Instance.Id = value;
        return this;
    }

    public GameFactoryBase<TParent, T> WithAnnotation(string value)
    {
        Instance.Annotations.Add(value);
        return this;
    }

    public TReturn WithId<TReturn>(string value)
        where TReturn : GameFactoryBase<TParent, T>
    {
        Instance.Id = value;
        return (TReturn)this;
    }
}