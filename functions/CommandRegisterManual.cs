using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kadense.RPG;

namespace Kadense.RPG
{
    public class CommandRegisterManual
    {
        private readonly ILogger<CommandRegisterManual> _logger;

        public CommandRegisterManual(ILogger<CommandRegisterManual> logger)
        {
            _logger = logger;
        }

        [Function("CommandRegisterManual")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            var processor = new DiscordInteractionProcessor();
            var result = await processor.RegisterCommandsAsync(_logger);

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(result);
        }
    }
}
