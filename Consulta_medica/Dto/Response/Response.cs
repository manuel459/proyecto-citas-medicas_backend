using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Dto.Response
{
    public class Response
    {
        public int exito { get; set; }
        public string mensaje { get; set; }
        public object data { get; set; }

        public Response() 
        {
            this.exito = 0;
        }
    }
}
