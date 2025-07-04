using Kadense.Models.Discord;
using Kadense.RPG.Dice;

namespace Kadense.RPG.Games;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class GameAttribute : Attribute
{
    public GameAttribute(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
    public string? Description { get; set; }
}
public abstract class GameBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public abstract class GameBase<T> : GameBase
    where T : GameBase
{
    public GameBase(T parent)
    {
        this.Parent = parent;
    }
    public T Parent { get; set; }


    public T End()
    {
        return Parent;
    }

}

public class Game<T> : GameBase<T>
    where T : GameBase
{
    public Game(T parent, string name, string description) : base(parent)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public GameEntity<Game<T>>? CharacterSection { get; set; }
    public GameEntity<Game<T>>? WorldSection { get; set; }

    public GameEntity<Game<T>> WithCharacterSection()
    {
        if (CharacterSection == null)
        {
            CharacterSection = new GameEntity<Game<T>>(this);
        }
        return CharacterSection!;
    }

    public GameEntity<Game<T>> WithWorldSection()
    {
        if (WorldSection == null)
        {
            WorldSection = new GameEntity<Game<T>>(this);
        }
        return WorldSection!;
    }
}

public class GameEntity<T> : GameBase<T>
    where T : GameBase
{
    public GameEntity(T parent) : base(parent)
    {

    }
    public List<GameSelection<GameEntity<T>>> Selections { get; set; } = new List<GameSelection<GameEntity<T>>>();
    public List<GameSelection<GameEntity<T>>> RelationshipSelections { get; set; } = new List<GameSelection<GameEntity<T>>>();

    public DiceRules? DiceRules { get; set; }

    public GameEntity<T> WithDiceRules(DiceRules diceRules)
    {
        DiceRules = diceRules;
        return this;
    }

    public void AddFields(IList<DiscordEmbedField> fields, Random random)
    {
        if (RandomAttributes.Count > 0)
        {
            if (this.DiceRules == null)
                this.DiceRules = new DiceRules(RandomAttributes.Count);

            var rolls = this.DiceRules.Roll(random);
            for (int i = 0; i < RandomAttributes.Count; i++)
            {
                fields.Add(new DiscordEmbedField
                {
                    Name = RandomAttributes[i],
                    Value = rolls[i].ToString()
                });
            }
        }

        foreach (var selection in Selections)
        {
            selection.AddFields(fields, random);
        }
    }

    public List<string> RandomAttributes { get; set; } = new List<string>();

    public GameEntity<T> WithRandomAttribute(string attribute)
    {
        if (!RandomAttributes.Contains(attribute))
        {
            RandomAttributes.Add(attribute);
        }
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

    public void AddFields(IList<DiscordEmbedField> fields, Random random)
    {

        foreach (var choice in Choose(random))
        {
            fields.Add(new DiscordEmbedField
            {
                Name = string.IsNullOrEmpty(choice.Description) ? Name : $"{Name}: {choice.Name}",
                Value = choice.Description ?? choice.Name,
            });

            choice.Attributes.Select(attr =>
                new DiscordEmbedField
                {
                    Name = attr.Key,
                    Value = attr.Value
                }
            ).ToList().ForEach(newField =>
            {
                fields.Add(newField);
            });


            if (choice.Selections.Count > 0)
            {
                foreach (var selection in choice.Selections)
                {
                    selection.AddFields(fields, random);
                }
            }
        }
    }

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

    public List<GameChoice<GameSelection<T>>> Choose(Random random)
    {
        var choices = Choices.ToArray();
        random.Shuffle(choices);

        var takenItems = choices.Take(NumberToChoose).ToList();

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
}

public class GameChoice<T> : GameBase<T>
    where T : GameBase
{
    public GameChoice(T parent) : base(parent)
    {

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

public partial class GamesFactory : GameBase
{
    public GamesFactory()
    {
        this.GetType().GetMethods().ToList().ForEach(m =>
        {
            var attributes = m.GetCustomAttributes(false);
            if (attributes.Any(attr => attr is GameAttribute))
            {
                m.Invoke(this, null);
            }
        });
    }
    public List<Game<GamesFactory>> Games { get; set; } = new List<Game<GamesFactory>>();
    public Game<GamesFactory> WithGame(string name, string description)
    {
        var game = new Game<GamesFactory>(this, name, description)
        {
            Name = name,
            Description = description ?? string.Empty
        };
        Games.Add(game);
        return game;
    }

    public List<Game<GamesFactory>> EndGames()
    {
        return Games;
    }
}

