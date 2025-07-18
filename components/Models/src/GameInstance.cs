using System.Text;

public class GameInstance
{
    public string? GameName { get; set; }
    public string? WorldSetup { get; set; }
    public string? WorldLlmPrompt { get; set; }
    public List<GameParticipant> Participants { get; set; } = new List<GameParticipant>();
    public string GetParticipantText(string[]? attributes = null)
    {
        var builder = new StringBuilder();
        if (attributes == null)
        {
            attributes = new string[] { }; // Default attribute
        }


        var maxParticipantLength = 20;

        var participants = Participants
            .Where(p => p.Name != null)
            .ToList();

        if (participants.Count() > 0)
            maxParticipantLength = Participants.Max(p => p.Name!.Length);

        if (maxParticipantLength < 20)
        {
            maxParticipantLength = 20; // Ensure at least 4 characters for the name
        }

        builder.AppendLine();
        builder.Append("```");
        builder.Append($"# ".PadLeft(7, ' '));
        builder.Append($"Name".PadRight(maxParticipantLength + 1, ' '));
        builder.Append($"Type".PadRight(20, ' '));
        foreach (var attribute in attributes)
        {
            builder.Append($"{attribute}".PadRight(15, ' '));
        }
        builder.AppendLine();
        for (int i = 0; i < participants.Count; i++)
        {
            var participant = participants[i];
            string prefix = (i + 1).ToString().PadLeft(5, ' ');
            builder.Append($"{prefix}) ");
            builder.Append($"{participant.Name}".PadRight(maxParticipantLength + 1, ' '));
            builder.Append($"{participant.Type}".PadRight(20, ' '));

            foreach (var attribute in attributes)
            {
                string initiative = participant.Attributes.ContainsKey(attribute) ? participant.Attributes["Initiative"].ToString() : "";
                builder.Append($"{initiative}".PadRight(15, ' '));
            }
            builder.AppendLine();
        }
        builder.Append("```");
        return builder.ToString();
    }
}
