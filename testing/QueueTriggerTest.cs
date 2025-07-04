using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace testing
{
    public class QueueTriggerTest
    {
        private readonly ILogger<QueueTriggerTest> _logger;

        public QueueTriggerTest(ILogger<QueueTriggerTest> logger)
        {
            _logger = logger;
        }

        [Function(nameof(QueueTriggerTest))]
        public void Run([QueueTrigger("myqueue-items", Connection = "")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
