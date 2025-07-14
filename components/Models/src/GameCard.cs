namespace Kadense.RPG.Models;

public class GameCard
{
    public GameCard()
    {
        Id = Guid.NewGuid();
    }

    public GameCard(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public override string ToString()
    {
        return Name ?? "Unnamed Card";
    }
}