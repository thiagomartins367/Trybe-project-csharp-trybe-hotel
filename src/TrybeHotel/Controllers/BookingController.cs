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
            string userEmail = GetUserEmailFromToken();
            BookingResponse newBooking = _repository.Add(bookingInsert, userEmail);
            return Created("GetBooking", newBooking);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        [HttpGet("{BookingId}")]
        public IActionResult GetBooking(int bookingId)
        {
            try
            {
                var userEmail = GetUserEmailFromToken();
                var booking = _repository.GetBooking(bookingId, userEmail);
                return Ok(booking);
            }
            catch (KeyNotFoundException notFoundException)
            {
                return NotFound(new { notFoundException.Message });
            }
            catch (UnauthorizedAccessException UnauthorizedException)
            {
                return Unauthorized(new { UnauthorizedException.Message });
            }
        }

        private string GetUserEmailFromToken()
        {
            var userTokenClaims = HttpContext.User.Identity as ClaimsIdentity;
            string userEmail = userTokenClaims!.Claims
                .First(claim => claim.Type == ClaimTypes.Email).Value;
            return userEmail;
        }
    }
}
