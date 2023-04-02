using Consulta_medica.Dto.Request;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Repository
{
    public class DiagnosticoRepository : IDiagnosticoRepository
    {
        private readonly consulta_medicaContext _context;
        public DiagnosticoRepository(consulta_medicaContext context) 
        {
            _context = context;
        }

        public async Task<int> ValidatePermission(string correo) 
        {
            var permiso = await (from m in _context.Medico
                                 join p in _context.Permisos
                                 on m.Idtip equals p.idtip
                                 where m.Correo == correo && p.sSlug == "LIST-MODULE-DIAGNOSIS"
                                 select m).CountAsync();
            return permiso;
        }

        public async Task<object> getDiagnostico(DiagnosticoRequestDto request) 
        {

            var list = await (from c in _context.Citas
                              join m in _context.Medico
                              on c.Codmed equals m.Codmed
                              join e in _context.Especialidad
                              on c.Codes equals e.Codes
                              join p in _context.Paciente
                              on c.Dnip equals p.Dnip
                              where c.Estado == 1 && c.Dnip == request.dni
                              select new { c.Id,m.Nombre, NombEspecialidad = e.Nombre,p.Nomp, c.Feccit }).FirstOrDefaultAsync();

            return list;
        }

        public object ReporteDiagnostico(DiagnosticoRequestPdfDto request)
        {

            return request;

        }

        public async Task<object> getUpdDiagnostico(DiagnosticoRequestPdfDto request) 
        {
            var list = await _context.Citas.Where(x => x.Id == request.idCita).ToListAsync();

            foreach (var item in list)
            {
                item.Estado = 2;
                await _context.SaveChangesAsync();
            }
            
            var Codes = await (from e in _context.Especialidad
                        where e.Nombre == request.especialidad
                        select e.Codes).FirstOrDefaultAsync();

            var idMedico = await (from m in _context.Medico
                            where m.Nombre == request.nombre
                            select m.Codmed).FirstOrDefaultAsync();

            HistorialMedico ohistoria = new HistorialMedico();
            ohistoria.idCita = request.idCita;
            ohistoria.Dnip = request.id;
            ohistoria.Codmed = idMedico;
            ohistoria.Codes = Codes;
            ohistoria.Fecct = request.fecct;
            ohistoria.Diagnostico = request.diagnostico;
            ohistoria.Receta = request.medicamentos;
            _context.HistorialMedico.Add(ohistoria);
            await _context.SaveChangesAsync();

            return ohistoria;
        }

    }
}
