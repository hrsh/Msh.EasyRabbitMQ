using Microsoft.AspNetCore.Mvc;
using Msh.EasyRabbitMQ.ServiceBus;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendApp.Controllers
{
    [ApiController, Route("[controller]")]
    public class SendController : ControllerBase
    {
        private readonly IPublishManager _publishManager;

        public SendController(IPublishManager publishManager)
        {
            _publishManager = publishManager;
        }

        [HttpGet("{message}")]
        public IActionResult PublishAction(string message)
        {
            FakeClass fakeClass = new()
            {
                Price = 1000,
                Title = message
            };
            // 1
            //_publishManager.PublishUsingQueue(
            //    message: message,
            //    queue: Consts.QueueName);

            // 2
            //_publishManager.PublishUsingQueue(new FakeClass
            //{
            //    Price = 1000,
            //    Title = "title"
            //}, queue: Consts.QueueName);

            //_publishManager.PublishUsingQueue(fakeClass, Consts.QueueName);

            _publishManager.PublishUsingExchange(
                message,
                Consts.ExchangeName,
                Consts.RoutingKey,
                "topic",
                null);

            return Ok();
        }
    }
}
