namespace Kadense.RPG.Models;

public partial class GamesFactory : GameBase
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
    public List<Game<GamesFactory>> Games { get; set; } = new List<Game<GamesFactory>>();
    public Game<GamesFactory> WithGame(string name, string description)
    {
        var game = new Game<GamesFactory>(this, name, description)
        {
            Name = name,
            Description = description ?? string.Empty
        };
        Games.Add(game);
        return game;
    }

    public List<Game<GamesFactory>> EndGames()
    {
        return Games;
    }
}
