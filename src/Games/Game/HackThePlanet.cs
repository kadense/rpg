using System.Reflection;

namespace Kadense.RPG.Games;

public partial class GamesFactory : GameBase
{
    [Game("Hack the Planet")]
    public GamesFactory HackThePlanet()
    {
        var attr = MethodInfo.GetCurrentMethod()!.GetCustomAttribute<GameAttribute>()!;
        return this
            .WithGame(attr.Name, attr.Description ?? "TBD Add Description")
                .WithWorldSection()
                    .WithLlmPrompt(@"""
                        I would like a brief narrative introduction for a game set in 1995 with the technology of that time. The players are hackers that have been investigating a {antagonist} whom they believe is involved in {doing}, unfortunately the person they've been investigating has been alerted to the players activities, and is now framing them for it and will stop at nothing to discredit, imprison and threaten the hackers into silence. The hackers will have to expose the truth and clear their names. Add random elements for flair. Do not add a title or heading. 
                    """)
                    .WithSelection("The antagonist is a")
                        .WithVariableName("antagonist")
                        .WithChoice("Federal Agent")
                            .WithSelection("What are they up doing?")
                                .WithVariableName("doing")
                                .WithNewChoice("Involved in Embezzlement")
                                .WithNewChoice("Funding a foreign terrorist group")
                                .WithNewChoice("Performing identity-theft en-mass")
                                .WithNewChoice("Performing Mass Surveillance of the people")
                                .WithNewChoice("Working with the mob")
                                .WithNewChoice("Trying to take-down hacker groups")
                            .End()
                        .End()
                        .WithChoice("Police Officer")
                            .WithSelection("What are they up doing?")
                                .WithVariableName("doing")
                                .WithNewChoice("Involved in Embezzlement")
                                .WithNewChoice("Funding a foreign terrorist group")
                                .WithNewChoice("Performing identity-theft en-mass")
                                .WithNewChoice("Performing Mass Surveillance of the people")
                                .WithNewChoice("Working with the mob")
                                .WithNewChoice("Trying to take-down hacker groups")
                            .End()
                        .End()
                        .WithChoice("Mayor")
                            .WithSelection("What are they up doing?")
                                .WithVariableName("doing")
                                .WithNewChoice("Involved in Embezzlement")
                                .WithNewChoice("Funding a foreign terrorist group")
                                .WithNewChoice("Performing identity-theft en-mass")
                                .WithNewChoice("Performing Mass Surveillance of the people")
                                .WithNewChoice("Working with the mob")
                                .WithNewChoice("Trying to take-down hacker groups")
                            .End()
                        .End()
                        .WithChoice("Corporate Executive")
                            .WithSelection("What are they up doing?")
                                .WithVariableName("doing")
                                .WithNewChoice("Involved in Embezzlement")
                                .WithNewChoice("Funding a foreign terrorist group")
                                .WithNewChoice("Performing identity-theft en-mass")
                                .WithNewChoice("Performing Mass Surveillance of the people")
                                .WithNewChoice("Working with the mob")
                                .WithNewChoice("Trying to take-down hacker groups")
                            .End()
                        .End()
                        .WithChoice("Civil Servant")
                            .WithSelection("What are they up doing?")
                                .WithVariableName("doing")
                                .WithNewChoice("Involved in Embezzlement")
                                .WithNewChoice("Funding a foreign terrorist group")
                                .WithNewChoice("Performing identity-theft en-mass")
                                .WithNewChoice("Performing Mass Surveillance of the people")
                                .WithNewChoice("Working with the mob")
                                .WithNewChoice("Trying to take-down hacker groups")
                            .End()
                        .End()
                    .End()
                .End()
                .WithCharacterSection()
                    .WithSelection("Clothing")
                        .SetNumberToChoose(2)
                        .WithNewChoice("Neon Camoflage")
                        .WithNewChoice("Tight Undershirt")
                        .WithNewChoice("Too Many Piercings")
                        .WithNewChoice("Knee-High Shitkickers")
                        .WithNewChoice("Big Military Coat")
                        .WithNewChoice("Two-Tone Leathers")
                        .WithNewChoice("Wraparound Sunglasses")
                        .WithNewChoice("Leapard Print Something")
                        .WithNewChoice("Tiny Sunglasses")
                        .WithNewChoice("Enormous Headphones")
                    .End()
                    .WithSelection("Apparent Goal")
                        .WithNewChoice("To show (random PC) that you're hot shit")
                        .WithNewChoice("To make a load of cash")
                        .WithNewChoice("To get into an ivy-league college")
                        .WithNewChoice("To make out with (other, random PC) while 90's music plays in the background")
                        .WithNewChoice("To have a notorious virus named after you")
                        .WithNewChoice("To get revenge on (some asshole)")
                        .WithNewChoice("To build something that will last")
                        .WithNewChoice("To hack something that's never been hacked before")
                        .WithNewChoice("To be regarded as an icon among hackers")
                        .WithNewChoice("To save (some random PC) from themselves")
                    .End()
                    .WithRandomAttributeSplitValue(5)
                        .WithRandomAttributeMinValue(1)
                        .WithRandomAttribute("Meat")
                        .WithRandomAttribute("Cyberspace")
                .End()
                .WithCustomDeck("Words", () => new List<string>
                {
                    "Splice",
                    "Trusted Network",
                    "Glitch",
                    "Spoof",
                    "Trap Door",
                    "Smoke Screen",
                    "Program Manual",
                    "Termite",
                    "Refresh Rate",
                    "Encryption",
                    "Crash",
                    "Trojan",
                    "System Shot",
                    "Motherboard",
                    "Codebook",
                    "Blackwall",
                    "Virtual Machine",
                    "Kill Switch",
                    "Crosswire",
                    "Jump the Network",
                    "Dual Connection",
                    "Backdoor",
                    "Virus",
                    "Phreak",
                    "Work",
                    "Antivirus",
                    "I.C.E.",
                    "Dial-in",
                    "Thermal",
                    "Data Transfer",
                    "Hook Protocol",
                    "Viper",
                    "Slush Files",
                    "Megabyte",
                    "Nest Dump",
                    "Connection Port",
                    "Crackbox",
                    "Cerberus",
                    "Reboot",
                    "Overload"
                })

            .End();
    }
}

