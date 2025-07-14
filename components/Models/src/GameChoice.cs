using System.Text;

namespace Kadense.RPG.Models;

public class GameChoice<T> : GameBase<T>
    where T : GameBase
{
    public GameChoice(T parent) : base(parent)
    {

    }

    public string? LlmPrompt { get; set; }

    public GameChoice<T> WithLlmPrompt(string llmPrompt)
    {
        LlmPrompt = llmPrompt;
        return this;
    }

    public string GetLlmPrompt()
    {
        if (LlmPrompt == null)
            return Name;

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

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<GameSelection<GameChoice<T>>> Selections { get; set; } = new List<GameSelection<GameChoice<T>>>();
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    public GameSelection<GameChoice<T>> WithSelection(string name, string? description = null)
    {
        var selection = new GameSelection<GameChoice<T>>(this, name, description);
        Selections.Add(selection);
        return selection;
    }

    public GameChoice<T> WithNewSelection(string name, string? description = null)
    {
        var selection = new GameSelection<GameChoice<T>>(this, name, description);
        Selections.Add(selection);
        return this;
    }

    public GameChoice<T> WithAttribute(string key, string value)
    {
        if (Attributes.ContainsKey(key))
        {
            Attributes[key] = value;
        }
        else
        {
            Attributes.Add(key, value);
        }
        return this;
    }
}
