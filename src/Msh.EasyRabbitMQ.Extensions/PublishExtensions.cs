using System.Collections.Generic;

namespace Msh.EasyRabbitMQ.Extensions
{
    public static class PublishExtensions
    {
        public static void Publish(
            this MessageResult messageResult,
            Dictionary<string, object> arguments = null) =>
            messageResult.QueueResult.PublishManager.PublishUsingQueue(
                messageResult.Message,
                messageResult.QueueResult.PublishOptions.Queue,
                arguments);

        public static void Publish(
            this PublishResult publishResult,
            IDictionary<string, object> arguments = null)
        {
            publishResult.ExchangeTypeResult.PublishManager.PublishUsingExchange(
                publishResult.Message,
                publishResult.ExchangeTypeResult.PublishOptions.Exchange,
                publishResult.ExchangeTypeResult.PublishOptions.RoutingKey,
                publishResult.ExchangeTypeResult.PublishOptions.ExchangeType,
                arguments);
        }
    }
}
