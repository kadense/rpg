namespace Kadense.RPG.Models;

public class GameEntityFactory<TParent> : GameFactoryBase<TParent, GameEntity>
    where TParent : GameFactoryBase
{
    public GameEntityFactory(TParent parent) : base(parent)
    {

    }

    public GameEntityFactory<TParent> WithDiceRules(DiceRules diceRules)
    {
        Instance.DiceRules = diceRules;
        return this;
    }
    public GameEntityFactory<TParent> WithLlmPrompt(string llmPrompt)
    {
        Instance.LlmPrompt = llmPrompt;
        return this;
    }

    public GameEntityFactory<TParent> WithRandomAttribute(string attribute)
    {
        if (!Instance.RandomAttributes.Contains(attribute))
        {
            Instance.RandomAttributes.Add(attribute);
        }
        return this;
    }

    public GameEntityFactory<TParent> WithRandomAttributeSplitValue(int value)
    {
        Instance.RandomAttributeSplitValue = value;
        return this;
    }
    public GameEntityFactory<TParent> WithRandomAttributeMinValue(int value)
    {
        Instance.RandomAttributeMinValue = value;
        return this;
    }

    public GameSelectionFactory<GameEntityFactory<TParent>> WithSelection(string name, string? description = null)
    {
        var selection = new GameSelectionFactory<GameEntityFactory<TParent>>(this, name, description ?? string.Empty);
        Instance.Selections.Add(selection.Instance);
        return selection;
    }

    public GameSelectionFactory<GameEntityFactory<TParent>> WithRelationshipSelection(string name, string? description = null)
    {
        var selection = new GameSelectionFactory<GameEntityFactory<TParent>>(this, name, description ?? string.Empty);
        Instance.RelationshipSelections.Add(selection.Instance);
        return selection;
    }
}