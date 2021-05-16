using Msh.EasyRabbitMQ.ServiceBus.ServiceBusOptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Msh.EasyRabbitMQ.ServiceBus.ServiceBusPublisher
{
    public interface IPublishManager
    {
        /// <summary>
        /// Publish a single payload to the queue.
        /// </summary>
        /// <param name="payload">payload to publish</param>
        /// <param name="queue">Target queue, if null, publisher uses queue name from option</param>
        /// <param name="arguments">Additional arguments</param>
        void PublishUsingQueue(
            string payload,
            string queue = null,
            Dictionary<string, object> arguments = null);

        /// <summary>
        /// Publish a single payload to the queue. This overload uses a type as payload. Also, 
        /// this method uses <see cref="Newtonsoft.Json" /> for serialization, hence, 
        /// <see cref="ReferenceLoopHandling"/> will ignore.
        /// </summary>
        /// <typeparam name="T">The type of object to be sent</typeparam>
        /// <param name="source">An instance of object to be sent</param>
        /// <param name="queue">Target queue, if null, publisher uses queue name from option</param>
        /// <param name="arguments">Additional arguments</param>
        void PublishUsingQueue<T>(
            T source,
            string queue = null,
            Dictionary<string, object> arguments = null);

        /// <summary>
        /// Publish a single payload to the queue. This overload uses a type as payload. Also, 
        /// this method uses <see cref="Newtonsoft.Json" /> for serialization, hence, 
        /// <see cref="ReferenceLoopHandling"/> will ignore.
        /// </summary>
        /// <typeparam name="T">The type of object to be sent</typeparam>
        /// <param name="source">An instance of object to be sent</param>
        /// <param name="jsonSerializerSettings">Custom <see cref="ReferenceLoopHandling"/></param>
        /// <param name="queue">Target queue, if null, publisher uses queue name from option</param>
        /// <param name="arguments">Additional arguments</param>
        void PublishUsingQueue<T>(
            T source,
            JsonSerializerSettings jsonSerializerSettings,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void PublishUsingTaskQueue(
            string payload,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void PublishUsingTaskQueue<T>(
            T source,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void PublishUsingExchange(
            string payload,
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
            string payload,
            Action<PublishOptions> options,
            IDictionary<string, object> arguments = null);

        void PublishUsingExchange<T>(
            T source,
            Action<PublishOptions> options,
            IDictionary<string, object> arguments = null);
    }
}
