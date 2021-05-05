using Microsoft.AspNetCore.Mvc;
using Msh.EasyRabbitMQ.Extensions;
using Msh.EasyRabbitMQ.ServiceBus;

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
            //_publishManager.PublishUsingQueue(message, Consts.QueueName);

            //_publishManager.PublishUsingExchange(
            //    message,
            //    exchange: "my_exchange",
            //    routingKey: "my_key",
            //    exchangeType: "topic",
            //    arguments: null);

            _publishManager
                .WithQueue("queue_name")
                .WithMessage(message)
                .Publish();


            _publishManager
                .WithExchange("fluent_exchange_name")
                .WithRoute("fluent_routing_key")
                .WithMessage("fluent_message")
                .Topic()
                .Publish();

            _publishManager
                .WithExchange("")
                .WithRoute("")
                .WithMessage("")
                .Direct()
                .Publish();

            return Ok();
        }
    }
}
