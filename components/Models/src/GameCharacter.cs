namespace Kadense.RPG.Models;

public class GameCharacter : GameBase
{
    public bool IsPlayer { get; set; } = false;
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? FactBasedSummary { get; set; }

    public List<string>? DiversityFlags { get; set; } = new List<string>();

    public DateTime? DateOfBirth { get; set; }

    public int? Age { get; set; }

    public DateTime? Died { get; set; }

    public string? AgeRange { get; set; }

    public string? Pronouns { get; set; }

    public List<string> ImagePath { get; set; } = new List<string>();
    public int? GetAge(DateTime dateInput)
    {
        if (DateOfBirth.HasValue)
        {
            return (int)(dateInput.Subtract(DateOfBirth.Value!).TotalDays % 365);
        }
        else if (Age.HasValue)
        {
            return Age.Value;
        }
        return null;
    }
}