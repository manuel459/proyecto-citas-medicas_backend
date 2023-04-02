using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Repository
{
    public class PagosRepository : IPagosRepository
    {
        private readonly consulta_medicaContext _context;
        public PagosRepository(consulta_medicaContext context) 
        {
            _context = context;
        }

        public async Task<DetailPagoResponse> getInfoPago(int sId_Cita) 
        {

            var cita = await (from c in _context.Citas
                              join p in _context.Paciente
                              on c.Dnip equals p.Dnip
                              join e in _context.Especialidad
                              on c.Codes equals e.Codes
                              join m in _context.Medico
                              on c.Codmed equals m.Codmed
                              where c.Id == sId_Cita && c.nEstado_Pago == 1
                              select new DetailPagoResponse 
                              {
                                  nDnip = c.Dnip,
                                  sNombre_Paciente = p.Nomp,
                                  sEspecialidad = e.Nombre,
                                  sNombre_Medico = m.Nombre,
                                  dFecha_Cita = c.Feccit,
                                  dImporte_Total = e.Costo
                              }).FirstOrDefaultAsync();

            return cita;
        }

        public async Task<bool> InsertPagoCita(Pagos request) 
        {
            Pagos insert = new() 
            {
                nId_Pago = request.nId_Pago,
                sCod_Cita = request.sCod_Cita,
                nNumero_Tarjeta = request.nNumero_Tarjeta,
                nMes = request.nMes,
                nAnio = request.nAnio,
                nDni = request.nDni,
                dCreate_Datetime = DateTime.Now
            };
            _context.Pagos.Add(insert);
            var response = await _context.SaveChangesAsync();

            return response > 0; 
        }


        public async Task<bool> UpdateEstadoPagoCita(int nId_Cita)
        {
            var cita = await _context.Citas.FirstOrDefaultAsync(x => x.Id == nId_Cita);

            if (cita is not null)
            {
                cita.nEstado_Pago = 2;
            }

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }
    }
}
