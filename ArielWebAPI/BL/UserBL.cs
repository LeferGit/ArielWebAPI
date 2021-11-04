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

        public User GetUserByLastName(string lastName)
        {
            var list = _userRepo.GetUsersAsync();
            if (list == null)
                throw new Exception();

            return list.Result.Where(x => x.LastName == lastName).FirstOrDefault();
   
        }

        public async Task<User> GetUser(string id)
        {
            return await _userRepo.GetUserAsync(id);
        }
        public async Task Remove(string id)
        {
            await _userRepo.RemoveUserAsync(id);
        }

        public void CreateUser(string firstName,string lastName)
        {
            _rabbitMQUserPublisher.Publish(new User() { FirstName = firstName, LastName = lastName });

        }

        public async Task<List<User>> GetUsers()
        {
            return await _userRepo.GetUsersAsync();
        }
    }
}
