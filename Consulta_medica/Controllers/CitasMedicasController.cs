using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Consulta_medica.Sevurity.Atributtes;
using Consulta_medica.Sevurity.Enum;
using Consulta_medica.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitasMedicasController : ControllerBase
    {
        private readonly ICitasMedicasRepository _citas;
        private readonly consulta_medicaContext _context;
        public CitasMedicasController(ICitasMedicasRepository citas, consulta_medicaContext context) 
        {
            _citas = citas;
            _context = context;
        }

        [HttpGet("{usuario}")]
        public async Task<IActionResult> GetCitas([FromRoute] string usuario, [FromQuery] RequestGenericFilter request)
       {
            
            Response orespuesta = new Response();
            try
            {

                var permission = await _citas.ValidatePermission(usuario);
                if (permission)
                {
                    var lst = await _citas.GetCitas(request, usuario);
                    orespuesta.exito = 1;
                    orespuesta.mensaje = "citas traida con exito";
                    orespuesta.data = lst;
                }
                else
                {
                    return new ObjectResult("Forbidden") { StatusCode = 403, Value = orespuesta.mensaje };
                }

            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
           
            return Ok(orespuesta);
        }

        [HttpPost]
        public async Task<IActionResult> AddCitas(CitasRequestDto request) 
        {
            var response = await _citas.AddCitas(request);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCitas(CitasRequestDto request)
        {
            Response orespuesta = new Response();
            try
            {
                var lst = await _citas.UpdateCitas(request);
                orespuesta.exito = 1;
                orespuesta.mensaje = "cita editada con exito";
                orespuesta.data = lst;
               
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
            return Ok(orespuesta);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitas(int id)
        {
           
            Response orespuesta = new Response();
            try
            {
                var lst = await _citas.DeleteCitas(id);
                orespuesta.exito = 1;
                orespuesta.mensaje = "cita eliminada con exito";
                orespuesta.data = lst;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
            
            return Ok(orespuesta);
        }

        [HttpPost("DniPaciente")]
        public async Task<IActionResult> ConsultarDni(CitasRequestDniDto Personal)
        {
            Response orespuesta = new Response();
            try
            {
                var lst = await _context.Paciente.FirstOrDefaultAsync(x => x.Dnip == Personal.Dnip);
                orespuesta.exito = 1;
                orespuesta.mensaje = "dni traido con exito";
                orespuesta.data = lst;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
           
            return Ok(orespuesta);
        }

        [HttpPost("Especialidad")]
        public async Task<IActionResult> ConsultarEspecialidad(RequestPersonalDto Personal)
        {
            Response orespuesta = new Response();
            try
            {
                var lst = await (from m in _context.Medico
                                 join e in _context.Especialidad on m.Codes equals e.Codes
                                 where m.Codes == Personal.Codes
                                 select new { m.Codes, m.Nombre, e.Costo }).ToListAsync();
                orespuesta.exito = 1;
                orespuesta.mensaje = "Especialidad traido con exito";
                orespuesta.data = lst;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
            
            return Ok(orespuesta);
        }

        [HttpGet("ConsultarDiasLaborable/{Nombre}")]
        public async Task<IActionResult> ConsultarDiasLaborables(string Nombre) 
        {
            Response orespuesta = new Response();
            try
            {
                var horario = await (from h in _context.Horario
                                     join m in _context.Medico
                                     on h.Idhor equals m.Idhor
                                     where m.Nombre == Nombre
                                     select h).FirstOrDefaultAsync();

                if (horario is null)
                {
                    orespuesta.exito = 0;
                    orespuesta.mensaje = "No se encontro un horario en la base de datos para este usuario";
                    orespuesta.data = horario;
                }
                else 
                {
                    orespuesta.exito = 1;
                    orespuesta.mensaje = "Horarios traidos con exito";
                    orespuesta.data = horario;
                }

            }
            catch (Exception ex)
            {
                orespuesta.exito = 0;
                orespuesta.mensaje = ex.Message;
            }
            return Ok(orespuesta);
        }

        [HttpPost("Horario")]
        public async Task<IActionResult> ConsultarHorario(DatosRequestCitasDto request) 
        {
            var Hinicio = await (from m in _context.Medico
                           join h in _context.Horario
                           on m.Idhor equals h.Idhor
                           where m.Codmed == request.codmed
                                 select h.Hinicio).FirstOrDefaultAsync();

            var Hfin = await (from m in _context.Medico
                        join h in _context.Horario
                        on m.Idhor equals h.Idhor
                        where m.Codmed == request.codmed
                              select h.Hfin).FirstOrDefaultAsync();

            List<Content> Contenthoras = new List<Content>();

            for (int i = (int)TimeSpan.Parse(Hinicio.ToString()).TotalHours; i < TimeSpan.Parse(Hfin.ToString()).TotalHours; i++)
            {   
                Content ocontent = new Content();

                ocontent.contentCitas = TimeSpan.Parse(i.ToString() + ":00" + ":00");
                var citasRegistradas = await (from m in _context.Medico
                                              join c in _context.Citas
                                              on m.Codmed equals c.Codmed
                                              where m.Codmed == request.codmed
                                              && c.Feccit == request.Feccit
                                              select c.Hora).ToListAsync();

                ocontent.status = !citasRegistradas.Contains(ocontent.contentCitas);
                Contenthoras.Add(ocontent);
            }
            Response orespuesta = new Response();
            orespuesta.exito = 1;
            orespuesta.mensaje = "Horarios traidos con exito";
            orespuesta.data = Contenthoras;

            return Ok(orespuesta);
        }

        [HttpPost("NombreMedico")]
        public async Task<IActionResult> ObtenerCita(CitasRequestCodmedDto codmed) 
        {
            Response orespuesta = new Response();
            try
            {
                var nombreMedico = await (from m in _context.Medico
                                          where m.Codmed == codmed.codmed
                                          select m.Nombre).FirstOrDefaultAsync();
                if (nombreMedico != null)
                {
                    orespuesta.exito = 1;
                    orespuesta.mensaje = "Nombre exitoso";
                }
                else 
                {
                    orespuesta.exito = 0;
                    orespuesta.mensaje = "No se encontro registro";
                }
                
                orespuesta.data = nombreMedico;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message; 
            }

            return Ok(orespuesta);
        }

        [HttpGet("HistoricMedik/{dnip}")]
        public async Task<IActionResult> obtenerHistoriaMedica([FromRoute] int dnip)
        {
            Response orespuesta = new Response();
            try
            {
                var obtenerCita = await (from h in _context.HistorialMedico
                                         join e in _context.Especialidad
                                         on h.Codes equals e.Codes
                                         join m in _context.Medico
                                         on h.Codmed equals m.Codmed
                                         join p in _context.Paciente
                                         on h.Dnip equals p.Dnip
                                         where h.Dnip == dnip
                                         orderby h.Fecct descending
                                         select new 
                                                 {
                                                    h.idCita,
                                                    sNombre_Especialidad = e.Nombre,
                                                    sNombre_Medico = m.Nombre,
                                                    h.Dnip,
                                                    p.Nomp,
                                                    h.Fecct,
                                                    h.Diagnostico,
                                                    h.Receta
                                                 }).ToListAsync();// await _context.HistorialMedico.Where(x => x.Dnip == dnip).ToListAsync();

                orespuesta.exito = 1;
                orespuesta.mensaje = "Consulta exitosa";
                orespuesta.data = obtenerCita;

            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }

            return Ok(orespuesta);
        }
    }
}
