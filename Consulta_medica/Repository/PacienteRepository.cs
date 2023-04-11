using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Repository
{
    public class PacienteRepository : IPacienteRepository
    {
        public readonly consulta_medicaContext _context;
        public PacienteRepository(consulta_medicaContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<Paciente>> GetPacientes(string correoElectronico)
        {
            PermisosRepository permisos = new PermisosRepository(_context);

            var permisosGenerales = await permisos.validateGenericPermission(correoElectronico);

            var exist = permisosGenerales.Contains("LIST-MODULE-PACIENTES-INDIVIDUAL");

            if (exist)
            {
                var lst = await (from p in _context.Paciente
                                 join c in _context.Citas
                                 on p.Dnip equals c.Dnip
                                 where c.Codmed == _context.Medico.Where(x => x.Correo == correoElectronico).Select(x => x.Codmed).FirstOrDefault()
                                 select p).OrderByDescending(x => x.Dnip).Distinct().ToListAsync();
                return lst;
            }
            else 
            {
                var lst = await _context.Paciente.OrderByDescending(x => x.Dnip).ToListAsync();
                return lst;
            }
            
           
        }

        public async Task<Paciente> AddPaciente(PacienteRequestDto request)
        {
            Paciente opaciente = new Paciente();
            opaciente.Dnip = request.Dnip;
            opaciente.Idtip = "U003";
            opaciente.Nomp = request.Nomp;
            opaciente.Numero = request.Numero;
            opaciente.Edad = request.edad;
            opaciente.correoElectronico = request.correoElectronico;
            _context.Paciente.Add(opaciente);
            await _context.SaveChangesAsync();
            return opaciente;
        }

        public async Task<PacienteRequestDto> UpdatePaciente(PacienteRequestDto request)
        {
            var id = await _context.Paciente.Where(x => x.Dnip == request.Dnip).ToListAsync();

            foreach (var item in id)
            {
                item.Nomp = request.Nomp;
                item.Numero = request.Numero;
                item.Edad = request.edad;
                item.correoElectronico = request.correoElectronico;
                await _context.SaveChangesAsync();
            }

            return request;
        }

        public async Task<Paciente> DeletePaciente(int id)
        {
            var lst = await _context.Paciente.FirstOrDefaultAsync(x => x.Dnip == id);
            _context.Paciente.Remove(lst);
            await _context.SaveChangesAsync();
            return lst;
        }

        public async Task<Response> Filters(RequestFilterDto request)
        {
            Response orespuesta = new Response();
            switch (request.status)
            {
                //Dni del Paciente
                case "0":
                    var Filter = await _context.Paciente.Where(x => x.Dnip == Int32.Parse(request.texto)).ToListAsync();
                    if (Filter.Count() > 0)
                    {
                        orespuesta.exito = 1;
                        orespuesta.mensaje = "Filtro exitoso";
                        orespuesta.data = Filter;
                    }
                    else
                    {
                        orespuesta.exito = 0;
                        orespuesta.mensaje = "No existe";
                        orespuesta.data = Filter;
                    }

                    break;
                //Nombre del paciente
                case "1":
                    var FilterTwo = await _context.Paciente.Where(x => x.Nomp.Contains(request.texto)).ToListAsync();
                    if (FilterTwo.Count() > 0)
                    {
                        orespuesta.exito = 1;
                        orespuesta.mensaje = "Filtro exitoso";
                        orespuesta.data = FilterTwo;
                    }
                    else
                    {
                        orespuesta.exito = 0;
                        orespuesta.mensaje = "No existe";
                        orespuesta.data = FilterTwo;
                    }
                    break;
                //Numero del Paciente
                case "2":
                    var FilterThree = await _context.Paciente.Where(x => x.Numero == Int32.Parse(request.texto)).ToListAsync();
                    if (FilterThree.Count() > 0)
                    {
                        orespuesta.exito = 1;
                        orespuesta.mensaje = "Filtro exitoso";
                        orespuesta.data = FilterThree;
                    }
                    else
                    {
                        orespuesta.exito = 0;
                        orespuesta.mensaje = "No existe";
                        orespuesta.data = FilterThree;
                    }
                    break;
            }
            return orespuesta;
        }
    }
}
