namespace Kadense.RPG;

public enum FollowupProcessorType
{
    PrivateResponse,
    PublicResponse,
    PrivateAiPromptResponse,
    PublicAiPromptResponse,
}

public class DiscordFollowupMessageRequest
{
    public DiscordFollowupMessageRequest() { }
    public string? GuildId { get; set; }
    public string? ChannelId { get; set; }
    public string? Token { get; set; }
    public string? Content { get; set; }
    public FollowupProcessorType? Type { get; set; } = FollowupProcessorType.PrivateResponse;
}