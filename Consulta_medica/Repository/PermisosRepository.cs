using Consulta_medica.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Repository
{
    public class PermisosRepository 
    {
        private readonly consulta_medicaContext _context;
        public PermisosRepository(consulta_medicaContext context)
        {
            _context = context;
        }

        public async Task<List<string>> validateGenericPermission(string correoElectronico) 
        {
            var permisosMedicos = await (from m in _context.Medico
                                  join p in _context.Permisos
                                  on m.Idtip equals p.idtip
                                  where m.Correo == correoElectronico
                                  select p.sSlug).ToListAsync();

            var permisosAdm = await (from a in _context.Administrador
                                  join p in _context.Permisos
                                  on a.Iptip equals p.idtip
                                  where a.Correo == correoElectronico
                                  select p.sSlug).ToListAsync();

            if (permisosMedicos.Count().Equals(0))
            {
              return permisosAdm;
            } 

            return permisosMedicos;
        }
    }
}
