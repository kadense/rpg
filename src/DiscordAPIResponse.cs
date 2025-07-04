
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Kadense.RPG
{
    public class DiscordApiResponse
    {
        public DiscordApiResponse(ActionResult response, DiscordFollowupMessageRequest? followupMessage = null)
        {
            Response = response;
            FollowupMessageRequest = followupMessage;
        }

        [HttpResult]
        public ActionResult Response { get; set; }

        [QueueOutput("kadense-rpg-followup-messages", Connection = "AzureWebJobsStorage")]
        public DiscordFollowupMessageRequest? FollowupMessageRequest { get; set; }
    }
}