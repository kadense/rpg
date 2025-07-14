namespace Kadense.RPG.Models;

public abstract class GameBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public abstract class GameBase<T> : GameBase
    where T : GameBase
{
    public GameBase(T parent)
    {
        this.Parent = parent;
    }
    public T Parent { get; set; }

    public T End()
    {
        return Parent;
    }
}
