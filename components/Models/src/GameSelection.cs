using System.Text;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;

namespace Kadense.RPG.Models;

public class GameSelection<T> : GameBase<T>
    where T : GameBase
{
    public GameSelection(T parent, string name, string? description) : base(parent)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public List<GameChoice<GameSelection<T>>> Choices { get; set; } = new List<GameChoice<GameSelection<T>>>();

    public bool MutuallyExclusive { get; set; } = true; // If true, only one choice can be selected at a time
    public int Color { get; set; } = 0x0000FF; // Default to blue color
    public int NumberToChoose { get; set; } = 1;

    public string? VariableName { get; set; }

    public List<List<GameChoice<GameSelection<T>>>> ChosenValues { get; set; } = new List<List<GameChoice<GameSelection<T>>>>();


    public GameSelection<T> SetColor(int number)
    {
        Color = number;
        return this;
    }

    public GameSelection<T> SetNumberToChoose(int number)
    {
        NumberToChoose = number;
        return this;
    }
    public GameSelection<T> WithVariableName(string variableName)
    {
        VariableName = variableName;
        return this;
    }

    public List<GameChoice<GameSelection<T>>> Choose(KadenseRandomizer random)
    {
        var choices = Choices.ToArray();
        random.Shuffle(choices);

        var takenItems = choices.Take(NumberToChoose).ToList();
        ChosenValues.Add(takenItems);
        if (MutuallyExclusive)
        {
            foreach (var takenItem in takenItems)
            {
                Choices.Remove(takenItem);
            }
        }

        return takenItems;
    }

    public GameChoice<GameSelection<T>> WithChoice(string name, string? description = null)
    {
        var choice = new GameChoice<GameSelection<T>>(this)
        {
            Name = name,
            Description = description
        };
        Choices.Add(choice);
        return choice;
    }

    public GameSelection<T> SetIsMutuallyExclusive(bool isMutuallyExclusive)
    {
        MutuallyExclusive = isMutuallyExclusive;
        return this;
    }

    public GameSelection<T> WithNewChoice(string name, string? description = null)
    {
        var choice = new GameChoice<GameSelection<T>>(this)
        {
            Name = name,
            Description = description
        };
        Choices.Add(choice);
        return this;
    }
    
    
    public void WithFields(StringBuilder builder, KadenseRandomizer random, int level)
    {
        var results = new Dictionary<string, string>();
        var prefix = "".PadLeft(level, '#');
        foreach (var choice in this.Choose(random))
        {
            builder.Append(prefix);
            builder.AppendLine(string.IsNullOrEmpty(choice.Description) ? this.Name : $"**{this.Name}:** {choice.Name}");
            
            builder.AppendLine(choice.Description ?? choice.Name);

            choice.Attributes.ToList().ForEach(attr =>
                results[attr.Key] = attr.Value
            );

            if (results.Count > 0)
            {
                builder.Append("```");
                var maxKeyLength = results.ToList().Max(kv => kv.Key.Length);
                var maxValueLength = results.ToList().Max(kv => kv.Value.Length);
                results.ToList().ForEach(kv =>
                {
                    builder
                        .AppendLine($"{kv.Key.PadRight(maxKeyLength)}: {kv.Value.PadLeft(maxValueLength)}");
                });
                builder.Append("```");
            }

            if (choice.Selections.Count > 0)
                {
                    foreach (var s in choice.Selections)
                    {
                        s.WithFields(builder, random, level + 1);
                    }
                }
        }
    }
}
