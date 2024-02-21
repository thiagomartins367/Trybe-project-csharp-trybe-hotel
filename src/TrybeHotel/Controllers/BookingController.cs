using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using TrybeHotel.Dto;
using TrybeHotel.Errors;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("booking")]
    [Produces("application/json")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _repository;
        private readonly IRoomRepository _roomRepository;

        public BookingController(IBookingRepository repository, IRoomRepository roomRepository)
        {
            _repository = repository;
            _roomRepository = roomRepository;
        }

        /// <summary>
        ///     Adiciona uma nova reserva
        /// </summary>
        /// <remarks>
        /// Request body example:
        ///
        ///     {
        ///        "checkIn": "2024-02-20",
        ///        "checkOut": "2024-03-20",
        ///        "guestQuant": 2,
        ///        "roomId": 1
        ///     }
        ///
        /// </remarks>
        /// <response code="201">`Created` Retorna os dados da nova reserva</response>
        /// <response code="400">`Bad Request` Retorna mensagem <b>"Guest quantity over room capacity"</b></response>
        /// <response code="401">`Unauthorized` Quando o token do usuário não é informado ou é inválido. (Sem corpo de resposta)</response>
        /// <response code="404">`Not Found` Retorna mensagem <b>"Room not found"</b> <br /> <p>`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</p></response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            try
            {
                if (!_roomRepository.RoomExists(bookingInsert.RoomId))
                    return NotFound("Room not found");
                RoomDto room = _roomRepository.GetRoomById(bookingInsert.RoomId);
                if (room.Capacity < bookingInsert.GuestQuant)
                    return BadRequest(new ApiErrorResponse { Message = "Guest quantity over room capacity" });
                string userEmail = GetUserEmailFromToken();
                BookingResponse newBooking = _repository.Add(bookingInsert, userEmail);
                return Created("GetBooking", newBooking);
            }
            catch (KeyNotFoundException notFoundException)
            {
                return NotFound(new ApiErrorResponse { Message = notFoundException.Message });
            }
            catch (UnauthorizedAccessException unauthorizedException)
            {
                return Unauthorized(new ApiErrorResponse { Message = unauthorizedException.Message });
            }
        }

        /// <summary>
        ///     Retorna uma reserva pelo id
        /// </summary>
        /// <response code="200">`OK` Retorna uma reserva</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação do parâmetro <i>BookingId</i></response>
        /// <response code="401">`Unauthorized` Quando o token do usuário não é informado ou é inválido. (Sem corpo de resposta)</response>
        /// <response code="404">`Not Found` Retorna mensagem <b>"Booking not found"</b> <br /> <p>`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</p></response>
        /// <response code="405">`Method Not Allowed` Acesso incorreto ao <i>endpoint</i>. Ex: <b>/booking/</b> (Sem corpo de resposta)</response>
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        [HttpGet("{BookingId}")]
        public IActionResult GetBooking(int BookingId)
        {
            try
            {
                var userEmail = GetUserEmailFromToken();
                var booking = _repository.GetBooking(BookingId, userEmail);
                return Ok(booking);
            }
            catch (KeyNotFoundException notFoundException)
            {
                return NotFound(new ApiErrorResponse { Message = notFoundException.Message });
            }
            catch (UnauthorizedAccessException UnauthorizedException)
            {
                return Unauthorized(new ApiErrorResponse { Message = UnauthorizedException.Message });
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
