namespace Kadense.RPG.Models;

public class GameSelectionFactory<TParent> : GameFactoryBase<TParent, GameSelection>
    where TParent : GameFactoryBase
{
    public GameSelectionFactory(TParent parent, string name, string? description) : base(parent)
    {
        Instance = new GameSelection(name, description);
    }

    public GameSelectionFactory<TParent> WithName(string name)
    {
        Instance.Name = name;
        return this;
    }

    public GameSelectionFactory<TParent> WithDescription(string? description)
    {
        Instance.Description = description;
        return this;
    }

    
    public GameSelectionFactory<TParent> SetColor(int number)
    {
        Instance.Color = number;
        return this;
    }

    public GameSelectionFactory<TParent> SetNumberToChoose(int number)
    {
        Instance.NumberToChoose = number;
        return this;
    }
    public GameSelectionFactory<TParent> WithVariableName(string variableName)
    {
        Instance.VariableName = variableName;
        return this;
    }


    public GameChoiceFactory<GameSelectionFactory<TParent>> WithChoice(string name, string? description = null)
    {
        var choice = new GameChoiceFactory<GameSelectionFactory<TParent>>(this, name, description);
        Instance.Choices.Add(choice.Instance);
        return choice;
    }

    public GameSelectionFactory<TParent> SetIsMutuallyExclusive(bool isMutuallyExclusive)
    {
        Instance.MutuallyExclusive = isMutuallyExclusive;
        return this;
    }

    public GameSelectionFactory<TParent> WithNewChoice(string name, string? description = null)
    {
        var choice = new GameChoiceFactory<GameSelectionFactory<TParent>>(this, name, description);
        Instance.Choices.Add(choice.Instance);
        return this;
    }
}