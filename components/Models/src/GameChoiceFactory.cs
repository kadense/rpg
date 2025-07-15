namespace Kadense.RPG.Models;

public class GameChoiceFactory<TParent> : GameFactoryBase<TParent, GameChoice>
    where TParent : GameFactoryBase
{
    public GameChoiceFactory(TParent parent) : base(parent)
    {
    }
    public GameChoiceFactory(TParent parent, string name, string? description = null) : base(parent)
    {
        Instance = new GameChoice(name, description);
    }

    public GameChoiceFactory<TParent> WithName(string name)
    {
        Instance.Name = name;
        return this;
    }

    public GameChoiceFactory<TParent> WithDescription(string? description)
    {
        Instance.Description = description;
        return this;
    }

    public GameChoiceFactory<TParent> WithLlmPrompt(string llmPrompt)
    {
        Instance.LlmPrompt = llmPrompt;
        return this;
    }

    public GameSelectionFactory<GameChoiceFactory<TParent>> WithSelection(string name, string? description = null)
    {
        var selection = new GameSelectionFactory<GameChoiceFactory<TParent>>(this, name, description);
        Instance.Selections.Add(selection.Instance);
        return selection;
    }

    public GameChoiceFactory<TParent> WithNewSelection(string name, string? description = null)
    {
        var selection = new GameSelectionFactory<GameChoiceFactory<TParent>>(this, name, description);
        Instance.Selections.Add(selection.Instance);
        return this;
    }

    public GameChoiceFactory<TParent> WithAttribute(string key, string value)
    {
        if (Instance.Attributes.ContainsKey(key))
        {
            Instance.Attributes[key] = value;
        }
        else
        {
            Instance.Attributes.Add(key, value);
        }
        return this;
    }
}