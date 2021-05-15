using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public interface IRpcServiceManager
    {
        void RpcServer(string queue = null);
    }

    public class RpcServiceManager : IRpcServiceManager
    {
        private readonly IConnectionManager _connectionManager;
        private readonly RabbitMqOptions _options;
        private IModel _channel;

        public RpcServiceManager(
            IConnectionManager connectionManager,
            IOptions<RabbitMqOptions> options)
        {
            _connectionManager = connectionManager;
            _options = options.Value;
        }

        public void RpcServer(string queue = null)
        {
            _connectionManager.TryConnect();
            _channel = _connectionManager.Channel;

            _channel.QueueDeclare(
                queue: queue ?? _options.PublishOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: null);

            _channel.BasicQos(
                prefetchSize: _options.PrefetchSize,
                prefetchCount: _options.PrefetchCount,
                global: false);

            EventingBasicConsumer consumer = new(_channel);

            _channel.BasicConsume(
                queue: queue ?? _options.PublishOptions.Queue,
                autoAck: _options.AutoAcknowledge,
                consumer: consumer);

            consumer.Received += Received;
        }

        private void Received(object sender, BasicDeliverEventArgs e)
        {
            string response = string.Empty;
            var body = e.Body.ToArray();
            var props = e.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                if (!int.TryParse(message, out var number))
                    throw new ArgumentException(nameof(message));
#if DEBUG
                Console.WriteLine($" [.] fib({number})");
#endif
                response = Fib(number).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                response = "";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);
                _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
        }

        private static int Fib(int n) =>
            (n == 0 || n == 1) ? n : Fib(n - 1) + Fib(n - 2);
    }
}
