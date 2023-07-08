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
    public class MedicosRepository : IMedicosRepository
    {
        private readonly consulta_medicaContext _context;
        public MedicosRepository(consulta_medicaContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Medico>> GetMedicos()
        {
            var lst = await _context.Medico.OrderByDescending(x => x.Codmed).ToListAsync();
            return lst;
        }

        public async Task<Medico> AddMedico(MedicoRequestDto request)
        {
            var codmed = (from c in _context.Medico
                          orderby c.Codmed descending
                          select c.Codmed).FirstOrDefault();



            string[] ID = codmed.Split("M0");
            int Id = Convert.ToInt32(ID[1]) + 1;
            var IdFinal = Convert.ToString("M0" + Id);

            Medico omedico = new Medico();
            omedico.Codmed = IdFinal;
            omedico.Codes = request.Codes;
            omedico.Idtip = "U002";
            omedico.Nombre = request.Nombre;
            omedico.sApellidos = request.sApellidos;
            omedico.Sexo = request.Sexo;
            omedico.Nac = request.Nac;
            omedico.Correo = request.Correo;
            omedico.Pswd = request.Pswd;
            omedico.Dni = request.Dni;
            omedico.Idhor = request.Idhor;
            omedico.Asis = "No";
            _context.Medico.Add(omedico);
            await _context.SaveChangesAsync();
            return omedico;
        }

        public async Task<MedicoRequestDto> UpdateMedico(MedicoRequestDto request)
        {
            var id = await _context.Medico.Where(x => x.Codmed == request.Codmed).ToListAsync();

            foreach (var item in id)
            {
                item.Codes = request.Codes;
                item.Nombre = request.Nombre;
                item.sApellidos = request.sApellidos;
                item.Sexo = request.Sexo;
                item.Nac = request.Nac;
                item.Correo = request.Correo;
                item.Pswd = request.Pswd;
                item.Dni = request.Dni;
                item.Idhor = request.Idhor;
                await _context.SaveChangesAsync();
            }

            return request;
        }

        public async Task<Medico> DeleteMedico(string id)
        {
            var lst = await _context.Medico.FirstOrDefaultAsync(x => x.Codmed == id);
            _context.Medico.Remove(lst);
            await _context.SaveChangesAsync();
            return lst;
        }

        public async Task<Response> Filters(RequestFilterDto request)
        {
            Response orespuesta = new Response();
            switch (request.status)
            {
                //Dni del medico
                case "0":
                    var Filter = await _context.Medico.Where(x => x.Dni.ToString().Contains(request.texto)).ToListAsync();
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
                //Nombre del medico
                case "1":
                    var FilterTwo = await _context.Medico.Where(x => x.Nombre.Contains(request.texto)).ToListAsync();
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
                //Codigo del medico
                case "2":
                    var FilterThree = await _context.Medico.Where(x => x.Codmed.Contains(request.texto)).ToListAsync();
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

        public async Task<int> ValidatePermission(string correoElectronico)
        {
            var permiso = await((from m in _context.Administrador
                                join p in _context.Permisos
                                on m.Iptip equals p.idtip
                                where m.Correo == correoElectronico && p.sSlug == "LIST-MODULE-MEDICOS"
                                select p.sSlug).Union(
                                from m in _context.Recepcions
                                join p in _context.Permisos
                                on m.Iptip equals p.idtip
                                where m.Correo == correoElectronico && p.sSlug == "LIST-MODULE-MEDICOS"
                                select p.sSlug
                                )).CountAsync();

            return permiso;
        }
    }
}
