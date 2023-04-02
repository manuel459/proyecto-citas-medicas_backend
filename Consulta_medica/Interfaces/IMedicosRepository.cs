using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Interfaces
{
    public interface IMedicosRepository
    {
        Task<IEnumerable<Medico>> GetMedicos();
        Task<Medico> AddMedico(MedicoRequestDto request);
        Task<MedicoRequestDto> UpdateMedico(MedicoRequestDto request);
        Task<Medico> DeleteMedico(string id);
        Task<Response> Filters(RequestFilterDto request);
        Task<int> ValidatePermission(string correoElectronico);
    }
}
