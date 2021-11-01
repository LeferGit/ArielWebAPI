using ArielWebAPI.BL;
using ArielWebAPI.DBs;
using ArielWebAPI.Models;
using ArielWebAPI.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Linq;

namespace ArielWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserBL _userBL;

        private readonly ILogger _logger;

        public UserController(ILogger<UserController> logger,UserBL userBL)
        {
            _logger = logger;
            _userBL = userBL;
            
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            var users = _userBL.GetAllUsers();

            //RabbitMQUserPublisher.Publish(user);//move to when creating...

            return Ok(users);
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult RemoveUser(string id)
        {
            _userBL.Remove(id);
            return Ok();
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult GetUserById(string id)
        {
            try
            {
                var user = _userBL.GetUser(id);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                return Ok(user);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            //RabbitMQUserPublisher.Publish(user);//move to when creating...

        }

        [HttpPost]
        public ActionResult CreateUser(string firstName,string lastName)
        {
            _userBL.CreateUser(firstName, lastName);

            return Ok();
        }

    }
}
