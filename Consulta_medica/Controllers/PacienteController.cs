using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Consulta_medica.RealTemp;
using Consulta_medica.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PacienteController : ControllerBase
    {
        private readonly consulta_medicaContext _context;
        private readonly IPacienteRepository _pacientes;
        private IHubContext<HubAlert> _hubContext;
        public PacienteController(consulta_medicaContext context, IPacienteRepository pacientes , IHubContext<HubAlert> hubContext)
        {
            _context = context;
            _pacientes = pacientes;
            _hubContext = hubContext;
        }

        [HttpGet("{usuario}")]
        public async Task<IActionResult> GetPacientes([FromRoute] string usuario, [FromQuery] RequestGenericFilter request)
        {
            Response orespuesta = new Response();
            try
            { 
                var lst = await _pacientes.GetPacientes(request, usuario);
                orespuesta.exito = 1;
                orespuesta.mensaje = "Paciente traido con exito";
                orespuesta.data = lst;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
           
            return Ok(orespuesta);
        }

        [HttpPost]
        public async Task<IActionResult> AddPaciente(PacienteRequestDto request)
        {
            Response orespuesta = new Response();
            try
            {
                // await _hubContext.Clients.All.SendAsync("Inserto paciente", request);
                //var detail = await _context.Paciente.FirstOrDefaultAsync(x => x.Dnip == request.Dnip);
                //if (detail is not null)
                //{
                //    orespuesta.exito = 0;
                //    orespuesta.mensaje = "Este paciente ya existe en la base de datos";
                //    return Ok(orespuesta);
                //}

                var lst = await _pacientes.AddPaciente(request);
                orespuesta.exito = 1;
                orespuesta.mensaje = "Paciente insertado con exito";
                orespuesta.data = lst;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
            return Ok(orespuesta);
            
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePaciente(PacienteRequestDto request)
        {
            var lst = await _pacientes.UpdatePaciente(request);
            Response orespuesta = new Response();
            orespuesta.exito = 1;
            orespuesta.mensaje = "Paciente editado con exito";
            orespuesta.data = lst;
            return Ok(orespuesta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            Response orespuesta = new Response();
            var lst = await _pacientes.DeletePaciente(id);
            orespuesta.exito = 1;
            orespuesta.mensaje = "Paciente eliminado con exito";
            orespuesta.data = lst;
            return Ok(orespuesta);
        }

    }
}
