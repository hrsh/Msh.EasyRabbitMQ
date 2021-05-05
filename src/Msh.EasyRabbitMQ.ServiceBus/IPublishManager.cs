using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public interface IPublishManager
    {
        /// <summary>
        /// Publish a single message to the queue.
        /// </summary>
        /// <param name="message">Message to publish</param>
        /// <param name="queue">Target queue, if null, publisher uses queue name from option</param>
        /// <param name="arguments">Additional arguments</param>
        void PublishUsingQueue(
            string message,
            string queue = null,
            Dictionary<string, object> arguments = null);

        /// <summary>
        /// Publish a single message to the queue. This overload uses a type as message. Also, 
        /// this method uses <see cref="Newtonsoft.Json" /> for serialization, hence, 
        /// <see cref="Newtonsoft.Json.ReferenceLoopHandling"/> will ignore.
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
        /// Publish a single message to the queue. This overload uses a type as message. Also, 
        /// this method uses <see cref="Newtonsoft.Json" /> for serialization, hence, 
        /// <see cref="Newtonsoft.Json.ReferenceLoopHandling"/> will ignore.
        /// </summary>
        /// <typeparam name="T">The type of object to be sent</typeparam>
        /// <param name="source">An instance of object to be sent</param>
        /// <param name="jsonSerializerSettings">Custom <see cref="Newtonsoft.Json.ReferenceLoopHandling"/></param>
        /// <param name="queue">Target queue, if null, publisher uses queue name from option</param>
        /// <param name="arguments">Additional arguments</param>
        void PublishUsingQueue<T>(
            T source,
            JsonSerializerSettings jsonSerializerSettings,
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
