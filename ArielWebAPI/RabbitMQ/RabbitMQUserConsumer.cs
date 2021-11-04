using ArielWebAPI.BL;
using ArielWebAPI.DBs;
using ArielWebAPI.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArielWebAPI.RabbitMQ
{
    public class RabbitMQUserConsumer
    {
        private EventingBasicConsumer _consumer;
        private IModel _channel;
        private User _user;
        private ILogger _logger;
        public bool IsServiceAvailable  { get; set; }

        private IUserRepository _userRepository;

        public RabbitMQUserConsumer(IUserRepository userRepository, ILogger<RabbitMQUserConsumer> logger)
        {
            _userRepository = userRepository;
            IsServiceAvailable = true;
            _logger = logger;

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

                _consumer = new EventingBasicConsumer(_channel);

                _consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _user = JsonSerializer.Deserialize<User>(message);
                    _userRepository.InsertUserAsync(_user);
                };

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                IsServiceAvailable = false;
            }
        }
        public User Consume()
        {
            if (_channel == null)
            {
                return null;
            }

            _channel.BasicConsume(queue: "usersq",
                                 autoAck: true,
                                 consumer: _consumer);
            return _user;
        }
    }
}
