using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consulta_medica.Repository
{
    public class ConfiguracionesRepository : IConfiguracionesRepository
    {
        private readonly consulta_medicaContext _context;
        public ConfiguracionesRepository(consulta_medicaContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<ConfiguracionesResponse>> getConfiguraciones(string sEntidad, string sId) 
        {
            var ParamEntidad = new SqlParameter("@sEntidad", sEntidad == null ? DBNull.Value:sEntidad);

            var ParamId = new SqlParameter("@sId", sId == null ? DBNull.Value : sId);

            var response = await _context.Set<ConfiguracionesResponse>().FromSqlRaw("EXECUTE sp_get_configuraciones @sEntidad, @sId", parameters: new[] { ParamEntidad, ParamId }).ToListAsync();

            return response;
        }
    }
}
