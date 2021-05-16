using Msh.EasyRabbitMQ.ServiceBus.ServiceBusOptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Msh.EasyRabbitMQ.ServiceBus.ServiceBusSubscriber
{
    public interface ISubscribeManager
    {
        void SubscribeUsingQueue(
            string queue = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingQueue(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingQueue<T>(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingTaskQueue(
            string queue = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingTaskQueue(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingTaskQueue<T>(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingExchange(
            string exchange = null,
            string routingKey = null,
            string queue = null,
            string exchangeType = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingExchange(
            Func<string, Task<bool>> callback,
            string exchange = null,
            string routingKey = null,
            string queue = null,
            string exchangeType = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingExchange<T>(
            Func<string, Task<bool>> callback,
            string exchange = null,
            string routingKey = null,
            string queue = null,
            string exchangeType = null,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingExchange(
            Action<SubscribOptions> options,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingExchange(
            Action<SubscribOptions> options,
            Func<string, Task<bool>> callback,
            Dictionary<string, object> arguments = null);

        void SubscribeUsingExchange<T>(
            Action<SubscribOptions> options,
            Func<string, Task<bool>> callback,
            Dictionary<string, object> arguments = null);
    }
}