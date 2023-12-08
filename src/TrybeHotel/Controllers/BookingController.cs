using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TrybeHotel.Dto;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("booking")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _repository;

        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            Room room = _repository.GetRoomById(bookingInsert.RoomId);
            if (room.RoomId == 0) return NotFound(new { Message = "room not found" });
            if (room.Capacity < bookingInsert.GuestQuant)
                return BadRequest(new { Message = "Guest quantity over room capacity" });
            var userTokenClaims = HttpContext.User.Identity as ClaimsIdentity;
            string userEmail = userTokenClaims!.Claims
                .First(claim => claim.Type == ClaimTypes.Email).Value;
            BookingResponse newBooking = _repository.Add(bookingInsert, userEmail);
            return Created("GetBooking", newBooking);
        }

        [HttpGet("{Bookingid}")]
        public IActionResult GetBooking(int Bookingid)
        {
            throw new NotImplementedException();
        }
    }
}
