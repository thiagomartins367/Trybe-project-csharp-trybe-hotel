using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Admin")]
        public IActionResult GetUsers()
        {
            IEnumerable<UserDto> users = _repository.GetUsers();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Add([FromBody] UserDtoInsert user)
        {
            if (UserExists(user))
                return Conflict(new { Message = "User email already exists" });
            var newUser = _repository.Add(user);
            return Created("", newUser);
        }

        private bool UserExists(UserDtoInsert user)
        {
            UserDto existingUser = _repository.GetUserByEmail(user.Email);
            if (existingUser.Email == user.Email) return true;
            return false;
        }
    }
}
