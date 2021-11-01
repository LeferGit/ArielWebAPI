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
            var list = _userRepo.GetUsers();
            if (list == null)
                throw new Exception();

            return list.Where(x => x.LastName == lastName).FirstOrDefault();
   
        }

        public User GetUser(string id)
        {
            return _userRepo.GetUser(id);
        }
        public void Remove(string id)
        {
            _userRepo.Remove(id);
        }

        public void CreateUser(string firstName,string lastName)
        {

            _rabbitMQUserPublisher.Publish(new User() { FirstName = firstName, LastName = lastName });
            _rabbitMQUserConsumer.Consume();


        }

        public List<User> GetAllUsers()
        {
            return _userRepo.GetUsers();
        }
    }
}
