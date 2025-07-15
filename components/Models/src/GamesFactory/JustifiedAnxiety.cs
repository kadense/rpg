using System.Reflection;

namespace Kadense.RPG.Models;

public partial class GamesFactory : GameFactoryBase
{
    [Game("Justified Anxiety")]
    public GamesFactory AddJustifiedAnxiety()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithWorldSection()
                    .WithLlmPrompt(@"""
                        I would like a brief introduction for a game set in a dystopian future where the world is ruled by a malfunctioning AI construct that manages the complex where you live, deep underground. Propaganda tells you that the complex is under constant attack from socialists and from their propaganda, as well as from aberrants who possess special powers, often these aberrants hide their nature as the AI construct imprisons and uses them for its own nefarious purposes. To protect itself and the complex, the AI construct has implemented a series of complex and multi-faceted rules concerning hierarchy and permission. It compartmentalises information as well, limiting you to what you need to know. The players are cloned agents for the AI construct, even death is not an escape as you will be downloaded to a newly cloned body and this hurts. You and your team have to suppress the rebellion while trying to survive in a world where trust is discouraged, your actions are constantly scrutinised and are living in fear is the norm. add random elements for flair. Do not add a title or header.
                    """)
                .End()
                .WithCharacterSection()
                    .WithSelection("Traits")
                        .SetNumberToChoose(3)
                        .WithNewChoice("Argumentative")
                        .WithNewChoice("Arrogant")
                        .WithNewChoice("Avaricious")
                        .WithNewChoice("Bootlicker")
                        .WithNewChoice("Cowardly")
                        .WithNewChoice("Cruel")
                        .WithNewChoice("Distrustful")
                        .WithNewChoice("Know-it-all")
                        .WithNewChoice("Liar")
                        .WithNewChoice("Scheming")
                        .WithNewChoice("Secretive")
                        .WithNewChoice("Violent")
                    .End()
                    .WithSelection("Secret Society")
                        .SetIsMutuallyExclusive(false)
                        .WithNewChoice("Not applicable") // 1
                        .WithNewChoice("Not applicable") // 2
                        .WithNewChoice("Not applicable") // 3
                        .WithNewChoice("Not applicable") // 4
                        .WithNewChoice("Not applicable") // 5
                        .WithNewChoice("Not applicable") // 6
                        .WithNewChoice("Not applicable") // 7
                        .WithNewChoice("Not applicable") // 8
                        .WithNewChoice("Not applicable") // 9
                        .WithNewChoice("Not applicable") // 10
                        .WithNewChoice("None, join the society of the character on your left")
                        .WithNewChoice("None, join the society of the character on your right")
                        .WithNewChoice("The Psychoactive Congregation")
                        .WithNewChoice("Organised Crime Syndicate")
                        .WithNewChoice("Hack the complex")
                        .WithNewChoice("Anti-robot Alliance")
                        .WithNewChoice("Abberants Uber Alles")
                        .WithNewChoice("Internal Affairs")
                        .WithNewChoice("Project Rockstar")
                        .WithNewChoice("The Escapists")
                    .End()
                    .WithSelection("Abberant Power")
                        .SetIsMutuallyExclusive(false)
                        .WithNewChoice("Not applicable") // 1
                        .WithNewChoice("Not applicable") // 2
                        .WithNewChoice("Not applicable") // 3
                        .WithNewChoice("Not applicable") // 4
                        .WithNewChoice("Not applicable") // 5
                        .WithNewChoice("Not applicable") // 6
                        .WithNewChoice("Not applicable") // 7
                        .WithNewChoice("Not applicable") // 8
                        .WithNewChoice("Not applicable") // 9
                        .WithNewChoice("Not applicable") // 10
                        .WithNewChoice("Regeneration")
                        .WithNewChoice("Chameleon Skin")
                        .WithNewChoice("Read minds")
                        .WithNewChoice("Implant thoughts")
                        .WithNewChoice("Enhanced senses")
                        .WithNewChoice("Erase memories")
                        .WithNewChoice("Start fires")
                        .WithNewChoice("Teleport")
                        .WithNewChoice("Machine curse")
                        .WithNewChoice("Super strength")
                    .End()
                .End()
            .End();
    }
}

