using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Dto.Request
{
    public class LogeoRequestDto
    {
        [Required]
        public string Contraseña { get; set; }
        [Required]
        public string CorreoElectronico { get; set; }
    }
}
