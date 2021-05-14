using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public class RpcServiceManager
    {
        private readonly IConnectionManager _connectionManager;
        private readonly RabbitMqOptions _options;

        public RpcServiceManager(
            IConnectionManager connectionManager,
            IOptions<RabbitMqOptions> options)
        {
            _connectionManager = connectionManager;
            _options = options.Value;
        }

        public void RpcServer(
            string queue = null)
        {
            _connectionManager.TryConnect();
            IModel channel = _connectionManager.Channel;
            channel.QueueDeclare(
                queue: queue ?? _options.PublishOptions.Queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(0, 1, false);

            EventingBasicConsumer consumer = new(channel);

            channel.BasicConsume(
                queue: queue ?? _options.PublishOptions.Queue,
                autoAck: _options.AutoAcknowledge,
                consumer: consumer);
        }
    }
}
