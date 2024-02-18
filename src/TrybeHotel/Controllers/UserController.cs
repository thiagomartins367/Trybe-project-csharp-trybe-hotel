using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TrybeHotel.Errors;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations.Rules;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("user")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Retorna todos os usuários cadastrados
        /// </summary>
        /// <remarks>
        ///     ⚠️ Apenas usuários <b>administradores</b> podem consumir esse <i>endpoint</i>.
        /// </remarks>
        /// <response code="200">`OK` Retorna todos os usuários cadastrados</response>
        /// <response code="401">`Unauthorized` Quando o token do usuário não é informado ou é inválido. (Sem corpo de resposta)</response>
        /// <response code="403">`Forbidden` Violação da política de usuário <b>Admin</b> através do campo <i>role</i> do token do usuário. (Sem corpo de resposta)</response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Admin")]
        public IActionResult GetUsers()
        {
            IEnumerable<UserDto> users = _repository.GetUsers();
            return Ok(users);
        }

        /// <summary>
        ///     Adiciona um novo usuário
        /// </summary>
        /// <remarks>
        /// Request body example:
        ///
        ///     {
        ///        "name": "Rebeca",
        ///        "email": "rebeca@hotmail.com",
        ///        "password": "senhaDaRebeca!123"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">`Created` Retorna os dados do novo usuário adicionado</response>
        /// <response code="400">`Bad Request` Retorna resposta padrão de erro de validação de campos</response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        /// <response code="409">`Conflict` Retorna mensagem de erro <b>"User email already exists"</b></response>
        /// <response code="415">`Unsupported Media Type` Retorna resposta padrão de tipo de mídia não suportado</response>
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost]
        public IActionResult Add([FromBody] UserDtoInsert user)
        {
            if (UserExists(user))
                return Conflict(new { Message = "User email already exists" });
            var newUser = _repository.Add(user);
            return Created("", newUser);
        }

        private bool UserExists(UserDtoInsert user)
        {
            try
            {
                UserDto existingUser = _repository.GetUserByEmail(user.Email);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}
