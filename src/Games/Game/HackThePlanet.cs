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

