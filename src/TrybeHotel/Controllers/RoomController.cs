using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Errors;
using TrybeHotel.Errors.ApiExceptions;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("room")]
    [Produces("application/json")]
    public class RoomController : Controller
    {
        private readonly IRoomRepository _repository;
        private readonly IHotelRepository _hotelRepository;

        public RoomController(IRoomRepository repository, IHotelRepository hotelRepository)
        {
            _repository = repository;
            _hotelRepository = hotelRepository;
        }

        /// <summary>
        ///     Retorna todos os quartos de um hotel pelo id do hotel
        /// </summary>
        /// <response code="200">`OK` Retorna todos os quartos de um hotel pelo id do hotel</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação do parâmetro <i>HotelId</i></response>
        /// <response code="404">`Not Found` Retorna mensagem <b>"Hotel not found"</b> <br /> <p>`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</p></response>
        /// <response code="405">`Method Not Allowed` Acesso incorreto ao <i>endpoint</i>. Ex: <b>/room/</b> (Sem corpo de resposta)</response>
        [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{HotelId}")]
        public IActionResult GetRoom(int HotelId)
        {
            try
            {
                var rooms = _repository.GetRooms(HotelId);
                return Ok(rooms);
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ApiErrorResponse { Message = notFoundException.Message });
            }
        }

        /// <summary>
        ///     Adiciona um novo quarto
        /// </summary>
        /// <remarks>
        /// ⚠️ Apenas usuários <b>administradores</b> podem consumir esse <i>endpoint</i>.
        /// <br />
        /// <br />
        /// Request body example:
        ///
        ///     {
        ///        "name": "Suite Deluxe",
        ///        "capacity": 4,
        ///        "image": "https://image-example.com/suite/suite-deluxe.jpg",
        ///        "hotelId": 3
        ///     }
        ///
        /// </remarks>
        /// <response code="201">`Created` Retorna os dados do novo quarto</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação de campos</response>
        /// <response code="401">`Unauthorized` Quando o token do usuário não é informado ou é inválido. (Sem corpo de resposta)</response>
        /// <response code="403">`Forbidden` Violação da política de usuário <b>Admin</b> através do campo <i>role</i> do token do usuário. (Sem corpo de resposta)</response>
        /// <response code="404">`Not Found` Retorna mensagem <b>"Hotel not found"</b> <br /> <p>`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</p></response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Admin")]
        public IActionResult PostRoom([FromBody] RoomDtoInsert roomToInsert)
        {
            if (!_hotelRepository.HotelExists(roomToInsert.HotelId))
                return NotFound(new ApiErrorResponse { Message = "Hotel not found" });
            var newRoom = _repository.AddRoom(new Room
            {
                Name = roomToInsert.Name,
                Capacity = roomToInsert.Capacity,
                Image = roomToInsert.Image,
                HotelId = roomToInsert.HotelId,
            });
            return Created(string.Empty, newRoom);
        }

        /// <summary>
        ///     Deleta um quarto pelo id
        /// </summary>
        /// <remarks>
        /// ⚠️ Apenas usuários <b>administradores</b> podem consumir esse <i>endpoint</i>.
        /// </remarks>
        /// <response code="204">`No Content` Quando a deleção ocorre com sucesso. (Sem corpo de resposta)</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação do parâmetro <i>RoomId</i></response>
        /// <response code="401">`Unauthorized` Quando o token do usuário não é informado ou é inválido. (Sem corpo de resposta)</response>
        /// <response code="403">`Forbidden` Violação da política de usuário <b>Admin</b> através do campo <i>role</i> do token do usuário. (Sem corpo de resposta)</response>
        /// <response code="404">`Not Found` Retorna mensagem <b>"Room not found"</b> <br /> <p>`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</p></response>
        /// <response code="405">`Method Not Allowed` Acesso incorreto ao <i>endpoint</i>. Ex: <b>/room/</b> (Sem corpo de resposta)</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [HttpDelete("{RoomId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int RoomId)
        {
            try
            {
                _repository.DeleteRoom(RoomId);
                return NoContent();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ApiErrorResponse { Message = notFoundException.Message });
            }
        }
    }
}
