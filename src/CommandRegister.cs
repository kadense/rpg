using System;
using Kadense.RPG;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace src
{
    public class CommandRegister
    {
        private readonly ILogger _logger;

        public CommandRegister(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CommandRegister>();
        }

        [Function("RegisterDiscordCommands")]
        public async Task RunAsync([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"RegisterDiscordCommands - Timer trigger function executed at: {DateTime.Now}");
            
            var processor = new DiscordInteractionProcessor();
            await processor.RegisterCommandsAsync(_logger);

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"RegisterDiscordCommands - Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
