using System.Text;

public class GameInstance
{
    public string? GameName { get; set; }

    public List<GameParticipant> Participants { get; set; } = new List<GameParticipant>();

    public string GetParticipantText(string[]? attributes = null)
    {
        var builder = new StringBuilder();
        if (attributes == null)
        {
            attributes = new string[] { }; // Default attribute
        }
        builder.Append($"Participant");
        builder.Append($"# ".PadLeft(6, ' '));
        builder.Append($"Name".PadRight(20, ' '));
        builder.Append($"Type".PadRight(20, ' '));
        foreach (var attribute in attributes)
        {
            builder.Append($"{attribute}".PadRight(15, ' '));
        }
        builder.AppendLine();
        for (int i = 0; i < Participants.Count; i++)
        {
            var participant = Participants[i];
            string prefix = (i + 1).ToString().PadLeft(5, ' ');
            builder.Append($"{prefix}) ");
            builder.Append($"**{participant.Name}**".PadRight(20, ' '));
            builder.Append($"{participant.Type}".PadRight(20, ' '));

            foreach (var attribute in attributes)
            {
                string initiative = participant.Attributes.ContainsKey(attribute) ? participant.Attributes["Initiative"].ToString() : "";
                builder.Append($"{initiative}".PadRight(15, ' '));
            }
            builder.AppendLine();
        }
        return builder.ToString();
    }
}
