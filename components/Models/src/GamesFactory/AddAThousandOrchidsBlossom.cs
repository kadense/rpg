using System.Reflection;

namespace Kadense.RPG.Models;

public partial class GamesFactory : GameFactoryBase
{
    [Game("A Thousand Orchids Blossom")]
    public GamesFactory AddAThousandOrchidsBlossom()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithWorldSection()
                    .WithLlmPrompt(@"""
                        I would like a brief narrative introduction for a game set in a fictional realm of Orcs. A fortress sits as the last line of defence between the invading forces that are hell bent on destroying your home city. An overwhelming force that is about to arrive and it's unlikely that help will arrive in time. It seems hopeless, but some of the greatest champions of the kingdom are among you. Describe the setting. Randomise the enemy and add other elements for flair. Do not introduce any of the characters. Do not add headers or titles.
                    """)
                .End()
                .WithCharacterSection()
                    .WithSelection("Name")
                        .WithNewChoice("Grahk")
                        .WithNewChoice("Duroth")
                        .WithNewChoice("Moon-tooth")
                        .WithNewChoice("Kilnash")
                        .WithNewChoice("Zogoth")
                        .WithNewChoice("Sallach")
                    .End()
                    .WithSelection("Reputation")
                        .WithNewChoice("Pious")
                        .WithNewChoice("Brutal")
                        .WithNewChoice("Heartless")
                        .WithNewChoice("Fearless")
                        .WithNewChoice("Furious")
                        .WithNewChoice("Destructive")
                    .End()
                    .WithSelection("Secret")
                        .WithNewChoice("Caring")
                        .WithNewChoice("Creative")
                        .WithNewChoice("Forgiving")
                        .WithNewChoice("Intellectual")
                        .WithNewChoice("Fearful")
                        .WithNewChoice("Romantic")
                    .End()
                    .WithSelection("Role")
                        .WithNewChoice("Wilderness Ranger")
                        .WithNewChoice("Omen-Scryer")
                        .WithNewChoice("Warlord's ex-bodyguard")
                        .WithNewChoice("Master weapon smith")
                        .WithNewChoice("Beast-speaker")
                        .WithNewChoice("Axe-thrower Sergeant")
                    .End()
                    .WithSelection("Motivation")
                        .WithNewChoice("To run away with someone")
                        .WithNewChoice("To take over as a warlord")
                        .WithNewChoice("To be regarded as a hero")
                        .WithNewChoice("To die in battle")
                        .WithNewChoice("To get revenge")
                        .WithNewChoice("To finally tell someone how you feel")
                    .End()
                    .WithRelationshipSelection("Feelings")
                        .SetIsMutuallyExclusive(false)
                        .WithNewChoice(":rage: Despises")
                        .WithNewChoice(":rolling_eyes: Frustrated by")
                        .WithNewChoice(":angry: Jealous of")
                        .WithNewChoice(":fist: Respects")
                        .WithNewChoice(":wink: Fancies")
                        .WithNewChoice(":heart: Loves")
                    .End()
                .End()
            .End();
    }
}

