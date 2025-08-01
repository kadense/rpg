using System.Reflection;

namespace Kadense.RPG.Models;

public partial class GamesFactory : GameFactoryBase
{
    [Game("Dead Channel")]
    public GamesFactory DeadChannel()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithWorldSection()
                    .WithLlmPrompt(@"""Generate a brief introduction to a horror game, where the players are trapped in a {location} surrounded by {threat}. The {threat} is vulnerable to {threatWeakness}. The players must survive, kill all of the {threat} in order to escape. There is a plot twist however, which is that {plotTwist}. Do not reveal this plot twist but do set it up, along with a number of other random elements to throw off the players. Do not include a header or title.""")
                    .WithSelection("Location")
                        .WithVariableName("location")
                        .WithNewChoice("School")
                        .WithNewChoice("Remote Cabin")
                        .WithNewChoice("Warehouse Rave")
                        .WithNewChoice("Large Family Home")
                        .WithNewChoice("Hospital")
                        .WithNewChoice("Fairground")
                    .End()
                    .WithSelection("Threat")
                        .WithVariableName("threat")
                        .WithNewChoice("Vampires")
                        .WithNewChoice("Werewolves")
                        .WithNewChoice("Witches")
                        .WithNewChoice("Demons")
                        .WithNewChoice("Possesor Spirit")
                        .WithNewChoice("Creepy Cult")
                    .End()
                    .WithSelection("Threat Weakness (or use the traditional ones)")
                        .WithVariableName("threatWeakness")
                        .WithNewChoice("Sunlight")
                        .WithNewChoice("Silver")
                        .WithNewChoice("Fire")
                        .WithNewChoice("Magic")
                        .WithNewChoice("Exorcism")
                        .WithNewChoice("Salt")
                    .End()
                    .WithSelection("Plot twist")
                        .WithVariableName("plotTwist")
                        .WithNewChoice("The threat is summoning a bigger threat")
                        .WithChoice("Threat is actually")
                            .WithLlmPrompt("Threat is actually {newThreat}")
                            .WithSelection("New Threat")
                                .WithVariableName("newThreat")
                                .WithNewChoice("Vampires")
                                .WithNewChoice("Werewolves")
                                .WithNewChoice("Witches")
                                .WithNewChoice("Demons")
                                .WithNewChoice("Possesor Spirit")
                                .WithNewChoice("Creepy Cult")
                            .End()
                        .End()
                        .WithNewChoice("The government did it")
                        .WithNewChoice("This is all your fault")
                        .WithNewChoice("One of the players is actually behind the threat")
                        .WithNewChoice("Your shelter burns, floods, etc")
                    .End()
                .End()
                .WithCharacterSection()
                    .WithSelection("Primary")
                        .WithChoice("Strong")
                            .WithSelection("Secondary")
                                .WithNewChoice("Morbid Curiousity")
                                .WithNewChoice("Over-Protective")
                                .WithNewChoice("Something to prove")
                            .End()
                        .End()
                        .WithChoice("Weird")
                            .WithSelection("Secondary")
                                .WithNewChoice("Too Dumb to be Scared")
                                .WithNewChoice("Over-Protective")
                                .WithNewChoice("Something to prove")
                            .End()
                        .End()
                        .WithChoice("Smart")
                            .WithSelection("Secondary")
                                .WithNewChoice("Too Dumb to be Scared")
                                .WithNewChoice("Morbid Curiousity")
                                .WithNewChoice("Something to prove")
                            .End()
                        .End()
                        .WithChoice("Hot")
                            .WithSelection("Secondary")
                                .WithNewChoice("Too Dumb to be Scared")
                                .WithNewChoice("Morbid Curiousity")
                                .WithNewChoice("Over-Protective")
                            .End()
                        .End()
                    .End()
                    .WithSelection("I just want")
                        .WithNewChoice("Hot makeouts")
                        .WithNewChoice("Recreational drugs")
                        .WithNewChoice("Fun memories")
                        .WithNewChoice("To make a true friend")
                        .WithNewChoice("Respect")
                        .WithNewChoice("Everyone to love me")
                    .End()
                .End()
            .End();
    }
}

