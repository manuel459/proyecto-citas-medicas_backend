using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IUserServiceRepository _userService;

        public LoginController(IUserServiceRepository userService)
        { _userService = userService; }


        [HttpPost("login")]
        public IActionResult Autentificar([FromBody] LogeoRequestDto model)
        {
            Response respuesta = new Response();
            var userresponse = _userService.Auth(model);
            if (userresponse.Token == null)
            {
                respuesta.exito = 0;
                respuesta.mensaje = "Usuario o contraseña incorrecta";
                return BadRequest(respuesta);
            }

            respuesta.exito = 1;
            respuesta.mensaje = "Ingreso correcto";
            respuesta.data = userresponse;
            return Ok(respuesta);
        }
    }
}
