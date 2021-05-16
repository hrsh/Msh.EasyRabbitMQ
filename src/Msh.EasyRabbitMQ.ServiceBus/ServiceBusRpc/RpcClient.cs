using System;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Options;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusConnection;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusOptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public interface IRpcClient
    {
        string Invoke(
            string payload,
            string routingKey = null);

        void Open(string correlationId = null);

        void Close();
    }

    public class RpcClient : IRpcClient
    {
        private readonly IConnectionManager _connectionManager;
        private readonly RabbitMqOptions _options;
        private IModel _channel;
        private string _replyQueueName;
        private EventingBasicConsumer _consumer;
        private IBasicProperties _props;
        private readonly BlockingCollection<string> _respQueue = new();
        private string _correlationId;

        public RpcClient(
            IConnectionManager connectionManager,
            IOptions<RabbitMqOptions> options)
        {
            _connectionManager = connectionManager;
            _options = options.Value;
        }

        public string Invoke(
            string payload,
            string routingKey = null)
        {
            var body = Encoding.UTF8.GetBytes(payload);

            _channel.BasicPublish(
                exchange: "",
                routingKey: routingKey ?? _options.PublishOptions.RoutingKey,
                basicProperties: _props,
                body: body);

            _channel.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);

            var r = _respQueue.TryTake(out var result) ? result : string.Empty;
            return r;
        }

        public void Open(string correlationId = null)
        {
            _connectionManager.TryConnect();
            _channel = _connectionManager.Channel;

            _replyQueueName = _channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_channel);

            _props = _channel.CreateBasicProperties();
            _correlationId = correlationId ?? Guid.NewGuid().ToString("D");
            _props.CorrelationId = _correlationId;
            _props.ReplyTo = _replyQueueName;

            _consumer.Received += Received;
        }

        private void Received(object sender, BasicDeliverEventArgs @event)
        {
            var body = @event.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            if (@event.BasicProperties.CorrelationId == _correlationId)
            {
                while (!_respQueue.IsAddingCompleted)
                {
                    var addResult = _respQueue.TryAdd(response);
                    if (addResult) _respQueue.CompleteAdding();
                }
            }
        }

        public void Close()
        {
            _channel.Dispose();
        }
    }
}