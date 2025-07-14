using System.Text;
using Kadense.Models.Discord;
using Kadense.Models.Discord.ResponseBuilders;

namespace Kadense.RPG.Models;

public class GameEntity<T> : GameBase<T>
    where T : GameBase
{
    public GameEntity(T parent) : base(parent)
    {

    }
    public List<GameSelection<GameEntity<T>>> Selections { get; set; } = new List<GameSelection<GameEntity<T>>>();
    public List<GameSelection<GameEntity<T>>> RelationshipSelections { get; set; } = new List<GameSelection<GameEntity<T>>>();

    public string? LlmPrompt { get; set; }

    public DiceRules? DiceRules { get; set; }

    public GameEntity<T> WithDiceRules(DiceRules diceRules)
    {
        DiceRules = diceRules;
        return this;
    }
    public GameEntity<T> WithLlmPrompt(string llmPrompt)
    {
        LlmPrompt = llmPrompt;
        return this;
    }

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

    public GameEntity<T> WithRandomAttribute(string attribute)
    {
        if (!RandomAttributes.Contains(attribute))
        {
            RandomAttributes.Add(attribute);
        }
        return this;
    }

    public GameEntity<T> WithRandomAttributeSplitValue(int value)
    {
        RandomAttributeSplitValue = value;
        return this;
    }
    public GameEntity<T> WithRandomAttributeMinValue(int value)
    {
        RandomAttributeMinValue = value;
        return this;
    }

    public GameSelection<GameEntity<T>> WithSelection(string name, string? description = null)
    {
        var selection = new GameSelection<GameEntity<T>>(this, name, description)
        {
            Name = name,
            Description = description ?? string.Empty
        };
        Selections.Add(selection);
        return selection;
    }

    public GameSelection<GameEntity<T>> WithRelationshipSelection(string name, string? description = null)
    {
        var selection = new GameSelection<GameEntity<T>>(this, name, description)
        {
            Name = name,
            Description = description ?? string.Empty
        };
        RelationshipSelections.Add(selection);
        return selection;
    }
}
