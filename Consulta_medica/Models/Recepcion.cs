using System;
using System.ComponentModel.DataAnnotations;

namespace Consulta_medica.Models
{
    public class Recepcion
    {
        [Key]
        public int codre { get; set; }
        public string Iptip { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Sexo { get; set; }
        public DateTime? Nac { get; set; }
        public string Correo { get; set; }
        public string Pswd { get; set; }
        public int? Dni { get; set; }
    }
}
