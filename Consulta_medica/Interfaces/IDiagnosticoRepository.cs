using Consulta_medica.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Interfaces
{
    public interface IDiagnosticoRepository
    {
        public Task<object> getDiagnostico(DiagnosticoRequestDto request);
        public object ReporteDiagnostico(DiagnosticoRequestPdfDto request);
        public Task<object> getUpdDiagnostico(DiagnosticoRequestPdfDto request);
        public Task<int> ValidatePermission(string correo);
    }
}
