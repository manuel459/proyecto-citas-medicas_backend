using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Consulta_medica.Models
{
    public partial class Administrador
    {
        public string Codad { get; set; }
        public string Iptip { get; set; }
        public string Nombre { get; set; }
        public string Sexo { get; set; }
        public DateTime? Nac { get; set; }
        public string Correo { get; set; }
        public string Pswd { get; set; }
        public int? Dni { get; set; }
    }
}
