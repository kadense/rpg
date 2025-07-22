namespace Kadense.RPG.Models;

public partial class GamesFactory : GameFactoryBase
{
    public GamesFactory()
    {
        this.GetType().GetMethods().ToList().ForEach(m =>
        {
            var attributes = m.GetCustomAttributes(false);
            if (attributes.Any(attr => attr is GameAttribute))
            {
                m.Invoke(this, null);
            }
        });
    }
    public List<Game> Games { get; set; } = new List<Game>();

    protected List<GameFactoryBase> FactoryParts { get; set; } = new List<GameFactoryBase>();

    public override void AddChild(GameFactoryBase child)
    {
        FactoryParts.Add(child);
    }
    public GameFactory<GamesFactory> WithGame(string name, string description)
    {
        var game = new GameFactory<GamesFactory>(this, name, description ?? string.Empty);
        Games.Add(game.Instance);
        return game;
    }

    public List<Game> EndGames()
    {
        foreach (var part in FactoryParts)
        {
            part.FinalizeFactory();
        }
        return Games;
    }
}
