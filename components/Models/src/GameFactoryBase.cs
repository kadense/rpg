namespace Kadense.RPG.Models;

public abstract class GameFactoryBase
{

}

public abstract class GameFactoryBase<TParent, T> : GameFactoryBase
    where TParent : GameFactoryBase
    where T : GameBase
{
    public GameFactoryBase(TParent parent)
    {
        this.Parent = parent;
    }
    
    public TParent Parent { get; set; }

    public T Instance { get; set; } = Activator.CreateInstance<T>();

    public TParent End()
    {
        return Parent;
    }
}