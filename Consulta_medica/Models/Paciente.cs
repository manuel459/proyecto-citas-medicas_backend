using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Consulta_medica.Models
{
    public partial class Paciente 
    {
        public int? Dnip { get; set; }
        public string Idtip { get; set; }
        public string Nomp { get; set; }
        public int? Numero { get; set; }
        public int? Edad { get; set; }
        public string correoElectronico { get; set; }
    }

    public class HubAlert : Hub 
    {

    }
}
