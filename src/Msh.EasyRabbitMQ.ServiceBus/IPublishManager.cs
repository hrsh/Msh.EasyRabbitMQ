using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public interface IPublishManager
    {
        void PublishUsingQueue(
            string message,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void PublishUsingQueue<T>(
            T source,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void PublishUsingTaskQueue(
            string message,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void PublishUsingTaskQueue<T>(
            T source,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void PublishUsingExchange(
            string message,
            string exchange = null,
            string routingKey = null,
            string exchangeType = null,
            IDictionary<string, object> arguments = null);

        void PublishUsingExchange<T>(
            T source,
            string exchange = null,
            string routingKey = null,
            string exchangeType = null,
            IDictionary<string, object> arguments = null);

        void PublishUsingExchange(
            string message,
            Action<PublishOptions> options,
            IDictionary<string, object> arguments = null);

        void PublishUsingExchange<T>(
            T source,
            Action<PublishOptions> options,
            IDictionary<string, object> arguments = null);
    }
}
