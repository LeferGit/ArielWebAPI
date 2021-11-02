using ArielWebAPI.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArielWebAPI.RabbitMQ
{
    public class RabbitMQUserPublisher
    {
        private IModel _channel;
        private ILogger _logger;
        public bool IsServiceAvailable { get; set; }

        public RabbitMQUserPublisher(ILogger<RabbitMQUserPublisher> logger)
        {
            _logger = logger;
            IsServiceAvailable = true;

            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();

                _channel.QueueDeclare(queue: "usersq",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                arguments: null);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc.ToString());
                IsServiceAvailable = false;
            }
            

        }
        public void Publish(User user)
        {
            string message = JsonSerializer.Serialize(
                user, typeof(User));
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                    routingKey: "usersq",
                                    basicProperties: null,
                                    body: body);
        }
    }
}
