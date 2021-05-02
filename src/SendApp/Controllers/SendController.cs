using Microsoft.AspNetCore.Mvc;
using Msh.EasyRabbitMQ.ServiceBus;
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
            _publishManager.PublishUsingQueue(
                message: message,
                queue: Consts.QueueName);
            return Ok();
        }
    }
}
