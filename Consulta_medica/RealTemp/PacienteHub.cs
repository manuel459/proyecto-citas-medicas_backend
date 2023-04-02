using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.RealTemp
{
    public class PacienteHub: Hub
    {
        public async Task Send(string Name, string Brand)
        {
            await Clients.All.SendAsync("Receive", Name, Brand);
        }
    }
}
