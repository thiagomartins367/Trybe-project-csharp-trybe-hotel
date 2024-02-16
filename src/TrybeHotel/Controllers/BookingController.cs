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
        private readonly IRoomRepository _roomRepository;

        public BookingController(IBookingRepository repository, IRoomRepository roomRepository)
        {
            _repository = repository;
            _roomRepository = roomRepository;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            try
            {
                RoomDto room = _roomRepository.GetRoomById(bookingInsert.RoomId);
                if (room.Capacity < bookingInsert.GuestQuant)
                    return BadRequest(new { Message = "Guest quantity over room capacity" });
                string userEmail = GetUserEmailFromToken();
                BookingResponse newBooking = _repository.Add(bookingInsert, userEmail);
                return Created("GetBooking", newBooking);
            }
            catch (KeyNotFoundException notFoundException)
            {
                return NotFound(new { notFoundException.Message });
            }
            catch (UnauthorizedAccessException unauthorizedException)
            {
                return Unauthorized(new { unauthorizedException.Message });
            }
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
