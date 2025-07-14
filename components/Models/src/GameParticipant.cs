public class GameParticipant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Type { get; set; } // e.g., "Player", "NPC", "Monster", etc.

    public Dictionary<string, int> Attributes { get; set; } = new Dictionary<string, int>();
}