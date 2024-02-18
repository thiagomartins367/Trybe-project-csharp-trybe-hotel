using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("geo")]
    [Produces("application/json")]
    public class GeoController : Controller
    {
        private readonly IHotelRepository _repository;
        private readonly IGeoService _geoService;

        public GeoController(IHotelRepository repository, IGeoService geoService)
        {
            _repository = repository;
            _geoService = geoService;
        }

        /// <summary>
        ///     Verifica o status da API de geolocalização
        /// </summary>
        /// <remarks>
        ///     ⚠️ Esse <i>endpoint</i> consome dados da API <b><a href="https://nominatim.org" target="_blank">nominatim</a></b>.
        /// </remarks>
        /// <response code="200">`OK` Retorna resposta específica da API de geolocalização</response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetStatus()
        {
            var externalApiContent = await _geoService.GetGeoStatus();
            return Ok(externalApiContent);
        }

        /// <summary>
        ///     Retorna os hotéis mais próximos de um determinado endereço
        /// </summary>
        /// <remarks>
        /// ⚠️ Esse <i>endpoint</i> consome dados de geolocalização da API <b><a href="https://nominatim.org" target="_blank">nominatim</a></b>.
        /// <br/>
        /// <br/>
        /// ⚠️ (REFATORAR DE "FromBody" para "FromQuery" no método "GetHotelsByLocation") Esse <i>endpoint</i> usa <i>query params</i> (parâmetros de consulta), mas aceita <i>request body</i> (corpo de requisição).
        /// <br/>
        /// <br/>
        /// Request body example:
        ///
        ///     {
        ///        "address": "Av. Francisco Lacerda de Aguiar, 382",
        ///        "city": "Cachoeiro de Itapemirim",
        ///        "state": "ES"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">`OK` Retorna os hotéis mais próximos de um determinado endereço</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação de campos</response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(List<GeoDtoHotelResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpGet]
        [Route("address")]
        public async Task<IActionResult> GetHotelsByLocation([FromBody] GeoDto address)
        {
            var hotelsByLocation = await _geoService.GetHotelsByGeo(address, _repository);
            return Ok(hotelsByLocation);
        }
    }
}