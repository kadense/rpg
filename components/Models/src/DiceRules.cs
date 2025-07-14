
namespace Kadense.RPG.Models;

public class DiceRules
{
    public DiceRules(int faces)
    {
        MaxValue = faces;
    }

    public DiceRules(string[] attributes, int faces = 6)
    {
        MaxValue = faces;
        NumberToRoll = attributes.Count() + 2;
        RemoveLowest = true;
        RemoveHighest = true;
    }

    public int MinValue { get; set; } = 1;

    public int MaxValue { get; set; }

    public int NumberToRoll { get; set; } = 5;

    public bool RemoveLowest { get; set; } = false;

    public bool RemoveHighest { get; set; } = false;

    public int[] Roll(KadenseRandomizer random)
    {
        var rolls = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            rolls.Add(random.Roll(MinValue, MaxValue));
        }
        rolls.Sort();
        if (RemoveLowest)
            rolls.RemoveAt(0); // Remove the lowest roll

        if (RemoveHighest)
            rolls.RemoveAt(rolls.Count - 1); // Remove the highest roll

        var rollArray = rolls.ToArray();
        random.Shuffle(rollArray);
        
        return rollArray;
    }
}