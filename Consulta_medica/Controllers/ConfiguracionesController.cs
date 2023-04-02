using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionesController : ControllerBase
    {
        private IConfiguracionesRepository _configuraciones;
        public ConfiguracionesController(IConfiguracionesRepository configuraciones) 
        {
            _configuraciones = configuraciones;
        }
        [HttpGet]
        public async Task<IActionResult> getConfiguraciones([FromQuery] string sEntidad, [FromQuery] string sId) 
        {
            Response orespuesta = new();
            try
            {
                var configuraciones = await _configuraciones.getConfiguraciones(sEntidad, sId);
                if (configuraciones.Count().Equals(0))
                {
                    orespuesta.mensaje = "No se encontraron registros";
                    orespuesta.exito = 0;
                    return Ok(orespuesta);
                }
                orespuesta.data = configuraciones;
                orespuesta.mensaje = "consulta exitosa";
                orespuesta.exito = 1;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
            return Ok(orespuesta);
        }
    }
}
