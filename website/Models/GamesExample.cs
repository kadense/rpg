namespace Kadense.RPG.Website;

public static class GamesExample
{

    public static List<Game>? GameDetails { get; set; }
    public static List<Game> GetGames()
    {
        if (GameDetails != null)
            return GameDetails;

        GameDetails = new GamesFactory()
            .EndGames();
        
        return GameDetails.OrderBy(g => g.Name).ToList();
    }
}