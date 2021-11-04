using ArielWebAPI.DBs;
using ArielWebAPI.Models;
using ArielWebAPI.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArielWebAPI.BL
{
    public class UserBL
    {
        private readonly IUserRepository _userRepo;
        private readonly RabbitMQUserPublisher _rabbitMQUserPublisher;
        private readonly RabbitMQUserConsumer _rabbitMQUserConsumer;

        public UserBL(IUserRepository userRepo, RabbitMQUserPublisher rabbitMQUserPublisher, RabbitMQUserConsumer rabbitMQUserConsumer)
        {
            _userRepo = userRepo;
            _rabbitMQUserPublisher = rabbitMQUserPublisher;
            _rabbitMQUserConsumer = rabbitMQUserConsumer;
        }

        public async Task<User> GetUserAsync(string id)
        {
            return await _userRepo.GetUserAsync(id);
        }
        public async Task RemoveUserAsync(string id)
        {
            await _userRepo.RemoveUserAsync(id);
        }

        public void CreateUser(string firstName,string lastName)
        {
            _rabbitMQUserPublisher.Publish(new User() { FirstName = firstName, LastName = lastName });

        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userRepo.GetUsersAsync();
        }

        public async Task UpdateUserAsync(string id,User user)
        {
            user.Id = id;
            await _userRepo.UpdateUserAsync(user);
        }
    }
}
