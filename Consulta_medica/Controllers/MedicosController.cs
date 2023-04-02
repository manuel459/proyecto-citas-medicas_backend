using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Consulta_medica.Sevurity.Enum;
using Consulta_medica.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Consulta_medica.Sevurity.Requirement;
using Consulta_medica.Sevurity.Atributtes;

namespace Consulta_medica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicosController : ControllerBase
    {
        private readonly consulta_medicaContext _context;
        private readonly IMedicosRepository _medicos;
        public MedicosController(consulta_medicaContext context, IMedicosRepository medicos)
        {
            _context = context;
            _medicos = medicos;
        }

        [HttpGet("{usuario}")]

        public async Task<IActionResult> GetMedicos([FromRoute] string usuario)
        {
            Response orespuesta = new Response();

            var permission = await _medicos.ValidatePermission(usuario);
            if (permission > 0)
            {
                var lst = await _medicos.GetMedicos();
                orespuesta.exito = 1;
                orespuesta.mensaje = "Medico traido con exito";
                orespuesta.data = lst;
                return Ok(orespuesta);
            }
            else 
            {
                return new ObjectResult("Forbidden") { StatusCode = 403, Value = orespuesta.mensaje };
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> AddMedico(MedicoRequestDto request) 
        {
            var lst = await _medicos.AddMedico(request);
            Response orespuesta = new Response();
            orespuesta.exito = 1;
            orespuesta.mensaje = "Medico insertado con exito";
            orespuesta.data = lst;
            return Ok(orespuesta);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMedico(MedicoRequestDto request)
        {
            var lst = await _medicos.UpdateMedico(request);
            Response orespuesta = new Response();
            orespuesta.exito = 1;
            orespuesta.mensaje = "Medico editado con exito";
            orespuesta.data = lst;
            return Ok(orespuesta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) 
        {
            Response orespuesta = new Response();
            var lst = await _medicos.DeleteMedico(id);
            orespuesta.exito = 1;
            orespuesta.mensaje = "Medico eliminado con exito";
            orespuesta.data = lst;
            return Ok(orespuesta);
        }

        [HttpPost("Filters")]
        public async Task<IActionResult> Filters(RequestFilterDto request)
        {
            var lst = await _medicos.Filters(request);
            return Ok(lst);
        }
    }
}
