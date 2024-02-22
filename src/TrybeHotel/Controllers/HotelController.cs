using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TrybeHotel.Errors;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("hotel")]
    [Produces("application/json")]
    public class HotelController : Controller
    {
        private readonly IHotelRepository _repository;
        private readonly ICityRepository _cityRepository;

        public HotelController(IHotelRepository repository, ICityRepository cityRepository)
        {
            _repository = repository;
            _cityRepository = cityRepository;
        }

        /// <summary>
        ///     Retorna todos os hotéis cadastrados
        /// </summary>
        /// <response code="200">`OK` Retorna todos os hotéis cadastrados</response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult GetHotels()
        {
            var hotels = _repository.GetHotels();
            return Ok(hotels);
        }

        /// <summary>
        ///     Adiciona um novo hotel
        /// </summary>
        /// <remarks>
        /// ⚠️ Apenas usuários <b>administradores</b> podem consumir esse <i>endpoint</i>.
        /// <br />
        /// <br />
        /// Request body example:
        ///
        ///     {
        ///        "name": "Copacabana Palace",
        ///        "address": "Av. Atlântica, 1702",
        ///        "cityId": 4
        ///     }
        ///
        /// </remarks>
        /// <response code="201">`Created` Retorna os dados do novo hotel</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação de campos</response>
        /// <response code="401">`Unauthorized` Quando o token do usuário não é informado ou é inválido. (Sem corpo de resposta)</response>
        /// <response code="403">`Forbidden` Violação da política de usuário <b>Admin</b> através do campo <i>role</i> do token do usuário. (Sem corpo de resposta)</response>
        /// <response code="404">`Not Found` Retorna mensagem <b>"City not found"</b> <br /> <p>`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</p></response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Admin")]
        public IActionResult PostHotel([FromBody] HotelDtoInsert hotelToInsert)
        {
            if (!_cityRepository.CityExists(hotelToInsert.CityId))
                return NotFound(new ApiErrorResponse { Message = "City not found" });
            var newHotel = _repository.AddHotel(new Hotel
            {
                Name = hotelToInsert.Name,
                Address = hotelToInsert.Address,
                CityId = hotelToInsert.CityId,
            });
            return Created(string.Empty, newHotel);
        }
    }
}
