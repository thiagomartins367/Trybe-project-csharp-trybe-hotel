using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : Controller
    {
        private readonly IUserRepository _repository;

        public LoginController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login)
        {
            try
            {
                UserDto user = _repository.Login(login);
                string token = new TokenGenerator().Generate(user);
                return Ok(new { token });
            }
            catch (KeyNotFoundException)
            {
                return Unauthorized(new { Message = "Incorrect e-mail or password" });
            }
        }
    }
}
