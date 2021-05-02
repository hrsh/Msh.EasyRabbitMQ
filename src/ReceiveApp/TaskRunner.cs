using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Msh.EasyRabbitMQ.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiveApp
{
    public class TaskRunner : IHostedService
    {
        private readonly ISubscribeManager _subscribeManager;
        private readonly ILogger<TaskRunner> _logger;

        public TaskRunner(ISubscribeManager subscribeManager, ILogger<TaskRunner> logger)
        {
            _subscribeManager = subscribeManager;
            _logger = logger;

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Register subscribtion...");

            _subscribeManager.SubscribeUsingQueue(queue: Consts.QueueName);

            _logger.LogInformation("Subscribtion registered.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
