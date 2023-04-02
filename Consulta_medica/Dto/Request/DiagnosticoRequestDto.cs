using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Dto.Request
{
    public class DiagnosticoRequestDto
    {
        public int dni { get; set; }
    }

    public class DiagnosticoRequestPdfDto
    {
        public int id { get; set; }
        public int idCita { get; set; }
        public string nombre { get; set; }
        public string especialidad { get; set; }
        public string nomp { get; set; }
        public DateTime fecct { get; set; }
        public string diagnostico { get; set; }
        public string medicamentos { get; set; }

    }
}
