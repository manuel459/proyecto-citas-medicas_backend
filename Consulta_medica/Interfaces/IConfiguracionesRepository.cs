using Consulta_medica.Dto.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consulta_medica.Interfaces
{
    public interface IConfiguracionesRepository
    {
        Task<IEnumerable<ConfiguracionesResponse>> getConfiguraciones(string sEntidad, string sId);

        Task<IEnumerable<GraficaCitasResponse>> getGraficaCitas();
        Task<GraficasCitasDisponiblesStructureResponse> getGraficaCitasDisponibles(string codmed, string dFecha_Consulta);
    }
}
