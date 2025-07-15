using System.Text;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;

namespace Kadense.RPG.Models;

public class GameEntity : GameBase
{
    public GameEntity() : base()
    {

    }
    public List<GameSelection> Selections { get; set; } = new List<GameSelection>();
    public List<GameSelection> RelationshipSelections { get; set; } = new List<GameSelection>();

    public string? LlmPrompt { get; set; }

    public DiceRules? DiceRules { get; set; }


    public string? GetLlmPrompt()
    {
        if (LlmPrompt == null)
            return null;

        StringBuilder llmPromptBuilder = new StringBuilder(LlmPrompt);
        Selections.ForEach(s =>
        {
            s.ChosenValues.ForEach(c =>
            {
                if (c.Count > 0)
                {
                    var choice = c.First();
                    var values = c.Select(v => v.GetLlmPrompt()).ToList();
                    llmPromptBuilder.Replace($"{{{s.VariableName}}}", string.Join(", ", values));
                }
            });
        });
        return llmPromptBuilder.ToString();
    }


    public List<string> RandomAttributes { get; set; } = new List<string>();

    public int RandomAttributeSplitValue { get; set; } = 0;
    public int RandomAttributeMinValue { get; set; } = 0;

    

    public void WithFields(StringBuilder builder, KadenseRandomizer random)
    {
        if (this.RandomAttributes.Count > 0)
        {
            var randomAttributeResults = new Dictionary<string, string>();

            if (this.RandomAttributeSplitValue == 0)
            {
                if (this.DiceRules == null)
                    this.DiceRules = new DiceRules(this.RandomAttributes.Count);

                var rolls = this.DiceRules.Roll(random);
                for (int i = 0; i < this.RandomAttributes.Count; i++)
                {
                    randomAttributeResults[this.RandomAttributes[i]] = rolls[i].ToString();
                }
            }
            else
            {
                var items = new Dictionary<string, int>();
                var pointsToAssign = this.RandomAttributeSplitValue;
                this.RandomAttributes.ForEach(attr =>
                {
                    items.Add(attr, this.RandomAttributeMinValue);
                    pointsToAssign -= this.RandomAttributeMinValue;
                });

                for (int i = 0; i < pointsToAssign; i++)
                {
                    var keys = items.Keys.ToArray();
                    random.Shuffle(keys);
                    items[keys.First()] += 1;
                }

                this.RandomAttributes.ForEach(attr =>
                {
                    randomAttributeResults[attr] = items[attr].ToString();
                });
            }

            builder.Append("```");
            var maxKeyLength = randomAttributeResults.ToList().Max(kv => kv.Key.Length);
            var maxValueLength = randomAttributeResults.ToList().Max(kv => kv.Value.Length);
            randomAttributeResults.ToList().ForEach(kv =>
            {
                builder
                    .AppendLine($"{kv.Key.PadRight(maxKeyLength)}: {kv.Value.PadLeft(maxValueLength)}");
            });
            builder.AppendLine("```");

        }

        foreach (var selection in this.Selections)
        {
            selection.WithFields(builder, random, 2);
        }
    }
}
