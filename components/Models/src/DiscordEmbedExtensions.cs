using Kadense.Models.Discord.ResponseBuilders;

namespace Kadense.RPG.Models;

public static class DiscordEmbedExtensions
{
    public static DiscordEmbedBuilder WithFields(this DiscordEmbedBuilder builder, GameEntity? entity, KadenseRandomizer random)
    {
        if (entity == null)
            return builder;

        if (entity.RandomAttributes.Count > 0)
        {
            if (entity.RandomAttributeSplitValue == 0)
            {
                if (entity.DiceRules == null)
                    entity.DiceRules = new DiceRules(entity.RandomAttributes.Count);

                var rolls = entity.DiceRules.Roll(random);
                for (int i = 0; i < entity.RandomAttributes.Count; i++)
                {
                    builder
                        .WithField()
                            .WithName(entity.RandomAttributes[i])
                            .WithValue(rolls[i].ToString())
                        .End();
                }
            }
            else
            {
                var items = new Dictionary<string, int>();
                var pointsToAssign = entity.RandomAttributeSplitValue;
                entity.RandomAttributes.ForEach(attr =>
                {
                    items.Add(attr, entity.RandomAttributeMinValue);
                    pointsToAssign -= entity.RandomAttributeMinValue;
                });

                for (int i = 0; i < pointsToAssign; i++)
                {
                    var keys = items.Keys.ToArray();
                    random.Shuffle(keys);
                    items[keys.First()] += 1;
                }

                entity.RandomAttributes.ForEach(attr =>
                {
                    builder
                        .WithField()
                            .WithName(attr)
                            .WithValue(items[attr].ToString())
                        .End();
                });
            }
        }

        foreach (var selection in entity.Selections)
        {
            builder.WithFields(selection, random);
        }
        return builder;
    }

    
    public static DiscordEmbedBuilder WithFields(this DiscordEmbedBuilder builder, GameSelection? selection, KadenseRandomizer random)
    {
        if(selection == null)
            return builder;

        foreach (var choice in selection.Choose(random))
        {
            var choiceName = string.IsNullOrEmpty(choice.Description) ? selection.Name : $"{selection.Name}: {choice.Name}";
            builder
                .WithField()
                    .WithName(choiceName!)
                    .WithValue(choice.Description ?? choice.Name)
                .End();

            choice.Attributes.ToList().ForEach(attr =>
                builder
                    .WithField()
                        .WithName(attr.Key)
                        .WithValue(attr.Value)
                    .End()
            );

            if (choice.Selections.Count > 0)
            {
                foreach (var s in choice.Selections)
                {
                    builder.WithFields(s, random);
                }
            }
        }
        return builder;
    }
}