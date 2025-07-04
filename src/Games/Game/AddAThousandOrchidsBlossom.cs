using System.Reflection;

namespace Kadense.RPG.Games;

public partial class GamesFactory : GameBase
{
    [Game("A Thousand Orchids Blossom")]
    public GamesFactory AddAThousandOrchidsBlossom()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
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

