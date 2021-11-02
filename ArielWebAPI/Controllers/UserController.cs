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
            try
            {
                var users = _userBL.GetUsers();
                if(users == null)
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);

                return Ok(users);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult RemoveUser(string id)
        {
            try
            {
                _userBL.Remove(id);
                return Ok();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
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
        }

        [HttpPost]
        public ActionResult CreateUser(string firstName,string lastName)
        {
            try
            {
                _userBL.CreateUser(firstName, lastName);

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok();
        }
    }
}
