using ArielWebAPI.Models;
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
        public static void Publish(User user)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "usersq",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                arguments: null);

                string message = JsonSerializer.Serialize(
                     user, typeof(User));
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "usersq",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
