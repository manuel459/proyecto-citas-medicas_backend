using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PersonalMedikController : ControllerBase
    {
        public readonly consulta_medicaContext _context;
        public PersonalMedikController(consulta_medicaContext context) 
        {
            _context = context;
        }

        [HttpPost("RegistroPersonal")]
        public async Task<IActionResult> Registro_Personal(RequestPersonalDto Personal)
        {
            ResponsePacienteDto respuesta = new ResponsePacienteDto();
            if (Personal.Idtip == "U002")
            {
                var codmed = (from c in _context.Medico
                              orderby c.Codmed descending
                              select c.Codmed).FirstOrDefault();

                string[] ID = codmed.Split("M0");
                int Id = Convert.ToInt32(ID[1]) + 1;
                var IdFinal = Convert.ToString("M0" + Id);

                Medico omedico = new Medico();
                omedico.Codmed = IdFinal;
                omedico.Nombre = Personal.Nombre;
                omedico.Sexo = Personal.Sexo;
                omedico.Nac = Personal.Nac;
                omedico.Correo = Personal.Correo;
                omedico.Pswd = Personal.Pswd;
                omedico.Dni = Personal.Dni;
                //omedico.Idhor = Personal.Idhor;
                //omedico.Asis = Personal.Asis;
                omedico.Codes = Personal.Codes;
                omedico.Idtip = Personal.Idtip;
                await _context.Medico.AddAsync(omedico);
                await _context.SaveChangesAsync();
                respuesta.mensaje = ("Medico registrado con exito");

            }
            else if (Personal.Idtip == "U001")
            {
                var codad = (from c in _context.Administrador
                             orderby c.Codad descending
                             select c.Codad).FirstOrDefault();

                string[] codad1 = codad.Split("A00");
                int CODAD1 = Convert.ToInt32(codad1[1]) + 1;
                var IdFinalAd = Convert.ToString("A00" + CODAD1);

                Administrador oadministrador = new Administrador();
                oadministrador.Codad = IdFinalAd;
                oadministrador.Nombre = Personal.Nombre;
                oadministrador.Sexo = Personal.Sexo;
                oadministrador.Nac = Personal.Nac;
                oadministrador.Correo = Personal.Correo;
                oadministrador.Pswd = Personal.Pswd;
                oadministrador.Dni = Personal.Dni;
                oadministrador.Iptip = Personal.Idtip;
                await _context.Administrador.AddAsync(oadministrador);
                await _context.SaveChangesAsync();
                respuesta.mensaje = ("Administrador registrado con exito");
                return Ok(respuesta);
            }
            else
            {

                Paciente opaciente = new Paciente();
                opaciente.Dnip = Personal.Dni;
                opaciente.Nomp = Personal.Nombre;
                opaciente.Idtip = Personal.Idtip;
                opaciente.Numero = Convert.ToInt32(Personal.Numero);
                await _context.Paciente.AddAsync(opaciente);
                await _context.SaveChangesAsync();
                respuesta.mensaje = ("Paciente registrado con exito");
            }
            return Ok(respuesta);
        }

        [HttpPost("DniPaciente")]
        public async Task<IActionResult> ConsultarDni(RequestPersonalDto Personal) 
        {
            ResponsePacienteDto orespuesta = new ResponsePacienteDto();
            var lst = await _context.Paciente.FirstOrDefaultAsync(x => x.Dnip == Personal.Dni);
            orespuesta.data = lst;
            orespuesta.mensaje = "dni traido con exito";
            return Ok(orespuesta);
        }

        [HttpPost("Especialidad")]
        public async Task<IActionResult> ConsultarEspecialidad(RequestPersonalDto Personal) 
        {
            ResponsePacienteDto orespuesta = new ResponsePacienteDto();
            var lst = await (from m in _context.Medico
                             join h in _context.Horario on m.Idhor equals h.Idhor
                             join e in _context.Especialidad on m.Codes equals e.Codes
                             where m.Codes == Personal.Codes
                       select new { m.Codes,h.Hinicio,m.Nombre,e.Costo}).ToListAsync();
            orespuesta.data = lst;
            orespuesta.mensaje = "Especialidad traido con exito";
            return Ok(orespuesta);
        }
    }
}
