using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Msh.EasyRabbitMQ.ServiceBus;

namespace SendApp
{
    public class TaskRunner : IHostedService
    {
        private readonly ILogger<TaskRunner> _logger;

        public TaskRunner(ILogger<TaskRunner> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}