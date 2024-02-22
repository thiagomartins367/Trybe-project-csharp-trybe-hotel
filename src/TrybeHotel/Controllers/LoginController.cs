using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;
using TrybeHotel.Utils;
using TrybeHotel.Errors;
using TrybeHotel.Errors.ApiExceptions;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("login")]
    [Produces("application/json")]
    public class LoginController : Controller
    {
        private readonly IUserRepository _repository;

        public LoginController(IUserRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Efetua login do usuário
        /// </summary>
        /// <remarks>
        /// Request body example:
        ///
        ///     {
        ///        "email": "rebeca@hotmail.com",
        ///        "password": "senhaDaRebeca!123"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">`OK` Retorna token de autenticação do usuário</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação de campos</response>
        /// <response code="401">`Unauthorized` Retorna mensagem de erro <b>"Incorrect e-mail or password"</b></response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login)
        {
            try
            {
                UserDto user = _repository.Login(login);
                string token = new TokenGenerator().Generate(user);
                return Ok(new { token });
            }
            catch (NotFoundException notFoundException)
            {
                return Unauthorized(new ApiErrorResponse { Message = notFoundException.Message });
            }
        }
    }
}
