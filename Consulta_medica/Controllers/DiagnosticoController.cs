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
    public class DiagnosticoController : ControllerBase
    {
        private readonly IDiagnosticoRepository _diagnostico;
        public DiagnosticoController(IDiagnosticoRepository diagnostico) 
        {
            _diagnostico = diagnostico;
        }

        [HttpPost("ConsultarDiagnostico")]
        public async Task<IActionResult> getDiagnostico(DiagnosticoRequestDto request, [FromQuery] string correoElectronico) 
        {
            
            Response orespuesta = new Response();
            try
            {
                var lst = await _diagnostico.getDiagnostico(request);
                if (lst != null)
                {
                    orespuesta.exito = 1;
                    orespuesta.mensaje = "Diagnostico traido con exito";
                    orespuesta.data = lst;
                }
                else
                {
                    orespuesta.mensaje = "Paciente no figura con cita pendiente";
                    orespuesta.data = lst;
                }
               
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
  
            return Ok(orespuesta);
        }

        [HttpPost("ReporteDiagnostico")]
        public IActionResult ReporteDiagnostico(DiagnosticoRequestPdfDto request) 
        {
            Response orespuesta = new Response();
            try
            {
                var lst =  _diagnostico.ReporteDiagnostico(request);
                orespuesta.exito = 1;
                orespuesta.mensaje = "Diagnostico generado con exito";
                orespuesta.data = lst;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
            return Ok(orespuesta);
        }

        [HttpPost("SaveHistoryMedic")]
        public async Task<IActionResult> getUpdDiagnostico(DiagnosticoRequestPdfDto request)
        {
            Response orespuesta = new Response();
            try
            {
                var lst = await _diagnostico.getUpdDiagnostico(request);
                orespuesta.exito = 1;
                orespuesta.mensaje = "Paciente atendido";
                orespuesta.data = lst;
            }
            catch (Exception ex)
            {
                orespuesta.mensaje = ex.Message;
            }
            return Ok(orespuesta);
        }


    }
}
