
using System.Text.Json.Serialization;

namespace Kadense.RPG;

public class DiscordRateLimitResponse
{
    [JsonPropertyName("retry_after")]
    public float RetryAfter { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("global")]
    public bool Global { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }
}