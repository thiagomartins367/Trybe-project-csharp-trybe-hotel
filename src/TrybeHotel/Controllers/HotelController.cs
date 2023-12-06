using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("hotel")]
    public class HotelController : Controller
    {
        private readonly IHotelRepository _repository;

        public HotelController(IHotelRepository repository)
        {
            _repository = repository;
        }

        // 4. Desenvolva o endpoint GET /hotel
        [HttpGet]
        public IActionResult GetHotels()
        {
            var hotels = _repository.GetHotels();
            return Ok(hotels);
        }

        // 5. Desenvolva o endpoint POST /hotel
        [HttpPost]
        public IActionResult PostHotel([FromBody] Hotel hotel)
        {
            Console.WriteLine(hotel.HotelId);
            var newHotel = _repository.AddHotel(hotel);
            return Created("", newHotel);
        }
    }
}