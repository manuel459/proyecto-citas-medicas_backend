using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Interfaces
{
    public interface ICitasMedicasRepository
    {
        Task<IEnumerable<CitasQueryDto>> GetCitas(RequestGenericFilter request, string usuario);
        Task<bool> ValidatePermission(string correoElectronico);
        Task<Response> AddCitas(CitasRequestDto request);
        Task<CitasRequestDto> UpdateCitas(CitasRequestDto request);
        Task<Citas> DeleteCitas(int id);
    }
}
