using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Interfaces
{
    public interface IPacienteRepository
    {
        Task<IEnumerable<Paciente>> GetPacientes(RequestGenericFilter request, string correoElectronico);
        Task<Paciente> AddPaciente(PacienteRequestDto request);
        Task<PacienteRequestDto> UpdatePaciente(PacienteRequestDto request);
        Task<Paciente> DeletePaciente(int id);
    }
}
