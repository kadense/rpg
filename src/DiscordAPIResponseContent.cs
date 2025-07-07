
using Kadense.Models.Discord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Kadense.RPG
{
    public class DiscordApiResponseContent
    {
        public DiscordInteractionResponse? Response { get; set; }

        public DiscordFollowupMessageRequest? FollowupMessage { get; set; }
    }
}