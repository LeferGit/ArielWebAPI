using ArielWebAPI.DBs;
using ArielWebAPI.Models;
using ArielWebAPI.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Linq;

namespace ArielWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly RabbitMQUserConsumer _rabbitMQConsumer;

        private readonly ILogger _logger;

        public UserController(ILogger<UserController> logger,IUserRepo userRepo, RabbitMQUserConsumer rabbitMQConsumer)
        {
            _logger = logger;
            _userRepo = userRepo;
            _rabbitMQConsumer = rabbitMQConsumer;
            
        }

        [Route("[action]")]
        [HttpPost]
        public ActionResult RemoveUser(string lastName)
        {
            _userRepo.Remove(
                new User() {LastName = lastName });
            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult GetUserByLastName(string lastName)
        {
            var list = _userRepo.GetCollection();
            if (list == null)
                return StatusCode(StatusCodes.Status503ServiceUnavailable);

            var user = list.Where(x => x.LastName == lastName).FirstOrDefault();
            
            if(user == null)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            RabbitMQUserPublisher.Publish(user);
            return Ok(user);
        }

        [Route("[action]")]
        [HttpPost]
        public ActionResult CreateUser(string firstName,string lastName)
        {
            var user = new User() { FirstName = firstName, LastName = lastName };
            _userRepo.Insert(user);

            return Ok();
        }

    }
}
