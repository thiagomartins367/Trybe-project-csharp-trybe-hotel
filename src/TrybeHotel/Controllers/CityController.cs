using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Dto;
using TrybeHotel.Errors;
using TrybeHotel.Errors.ApiExceptions;
using TrybeHotel.Models;
using TrybeHotel.Repository;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("city")]
    [Produces("application/json")]
    public class CityController : Controller
    {
        private readonly ICityRepository _repository;

        public CityController(ICityRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Retorna todas as cidades cadastradas
        /// </summary>
        /// <response code="200">`OK` Retorna todas as cidades cadastradas</response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        [ProducesResponseType(typeof(IEnumerable<CityDto>), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult GetCities()
        {
            var cities = _repository.GetCities();
            return Ok(cities);
        }

        /// <summary>
        ///     Adiciona uma nova cidade
        /// </summary>
        /// <remarks>
        /// Request body example:
        ///
        ///     {
        ///        "name": "Rio de Janeiro",
        ///        "state": "RJ"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">`Created` Retorna os dados da nova cidade</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação de campos</response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(CityDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost]
        public IActionResult PostCity([FromBody] CityDtoInsert city)
        {
            var newCity = _repository.AddCity(new City { Name = city.Name, State = city.State });
            return Created(string.Empty, newCity);
        }

        /// <summary>
        ///     Atualiza dados de uma cidade
        /// </summary>
        /// <remarks>
        /// Request body example:
        ///
        ///     {
        ///        "cityId": 1,
        ///        "name": "Rio de Janeiro",
        ///        "state": "RJ"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">`OK` Retorna os dados atualizados da cidade</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação de campos</response>
        /// <response code="404">`Not Found` Retorna mensagem <b>"City not found"</b> <br /> <p>`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</p></response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPut]
        public IActionResult PutCity([FromBody] CityDto city)
        {
            try
            {
                var updatedCity = _repository.UpdateCity(
                new City
                {
                    CityId = city.CityId,
                    Name = city.Name,
                    State = city.State
                });
                return Ok(updatedCity);
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ApiErrorResponse { Message = notFoundException.Message });
            }
        }
    }
}
