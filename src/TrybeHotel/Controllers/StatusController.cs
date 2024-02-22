using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Utils;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("/")]
    [Produces("application/json")]
    public class StatusController : Controller
    {
        /// <summary>
        ///     Verifica o status da API
        /// </summary>
        /// <response code="200">`OK` Retorna mensagem <b>"online"</b></response>
        /// <response code="404">`Not Found` Acesso a um <i>endpoint</i> que não existe. (Sem corpo de resposta)</response>
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new ResponseMessage { Message = "online" });
        }
    }
}
