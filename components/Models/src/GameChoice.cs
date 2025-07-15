using System.Text;

namespace Kadense.RPG.Models;

public class GameChoice : GameBase
{
    public GameChoice() : base()
    {

    }
    public GameChoice(string name, string? description = null) : base()
    {
        Name = name;
        Description = description;
    }

    public string? LlmPrompt { get; set; }

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
    public List<GameSelection> Selections { get; set; } = new List<GameSelection>();
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
}
