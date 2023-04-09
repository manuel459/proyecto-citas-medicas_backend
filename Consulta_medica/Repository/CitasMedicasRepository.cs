using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using DocumentFormat.OpenXml.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Paragraph = iTextSharp.text.Paragraph;

namespace Consulta_medica.Repository
{
    public class CitasMedicasRepository : ICitasMedicasRepository
    {
        private readonly consulta_medicaContext _context;
        public CitasMedicasRepository(consulta_medicaContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidatePermission(string correoElectronico)
        {

            PermisosRepository permisosRepository = new PermisosRepository(_context);

            var permisosGenericos = await permisosRepository.validateGenericPermission(correoElectronico);

            var exist = permisosGenericos.Contains("LIST-MODULE-NEWCITA");

            return (exist) ? true : false;
        }
        public async Task<IEnumerable<CitasQueryDto>> GetCitas(RequestGenericFilter request)
        {

            var ParamNombreMedico = new SqlParameter("@sNombreMedico", DBNull.Value);

            var ParamNombrePaciente = new SqlParameter("@sNombrePaciente", DBNull.Value);

            var ParamNombreEspecialidad = new SqlParameter("@sNombre_Especialidad", DBNull.Value);

            if (request.numFilter != null && request.textFilter != null)
            {
                switch (request.numFilter)
                {
                    case 0:
                        ParamNombreMedico.Value = request.textFilter;
                        break;
                    case 1:
                        ParamNombrePaciente.Value = request.textFilter;
                        break;
                    case 2:
                        ParamNombreEspecialidad.Value = request.textFilter;
                        break;
                }
            }
            var ParamFechaIni = new SqlParameter("@dFechaInicio", request.dFechaInicio != null ? request.dFechaInicio : DBNull.Value);
            var ParamFechaFin = new SqlParameter("@dFechaFin", request.dFechaFin != null ? request.dFechaFin : DBNull.Value);

            var slgp = filterGeneric(request);

            var queryReponse = await _context.Set<CitasQueryDto>()
                .FromSqlRaw("EXECUTE sp_listado_citas @sNombreMedico, @sNombrePaciente, @sNombre_Especialidad, @sFilterOne, @sFilterTwo, @dFechaInicio, @dFechaFin",
                parameters: new[] { ParamNombreMedico, ParamNombrePaciente, ParamNombreEspecialidad, slgp.pFilterOne, slgp.pFilterTwo,ParamFechaIni, ParamFechaFin }).ToListAsync();

            return queryReponse;
        }

        public SqlGenericParameters filterGeneric(RequestGenericFilter request) 
        {
            SqlGenericParameters generic = new();
            generic.pFilterOne = new SqlParameter("@sFilterOne", request.sFilterOne != null?request.sFilterOne:DBNull.Value);
            generic.pFilterTwo = new SqlParameter("@sFilterTwo", request.sFilterTwo != null?request.sFilterTwo:DBNull.Value);
            return generic;
        }

        public async Task<Response> AddCitas(CitasRequestDto request)
        {

            Response response = new();
            //Extraction Nombre de la especialidad 
            request.NombreEspecialidad = _context.Especialidad.Where(x => x.Codes == request.Codes).Select(x => x.Nombre).FirstOrDefault();
            request.NombreMedico = _context.Medico.Where(x => x.Codmed == request.Codmed).Select(x => x.Nombre).FirstOrDefault();
            request.Costo = _context.Especialidad.Where(x => x.Codes == request.Codes).Select(x => x.Costo).FirstOrDefault();
            //end

            var FinalHora = request.Hora.Split(" ");

            Citas ocitas = new Citas();
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    ocitas.Dnip = request.Dnip;
                    ocitas.Codmed = request.Codmed;
                    ocitas.Feccit = request.Feccit;
                    ocitas.Estado = 1;
                    ocitas.nEstado_Pago = 1;
                    ocitas.Codes = request.Codes;
                    ocitas.Hora = TimeSpan.Parse(FinalHora[0].ToString());
                    _context.Citas.Add(ocitas);
                    var insert = await _context.SaveChangesAsync();

                    if (insert.Equals(1))
                    {
                        if (request.bActiveNotificaciones)
                        {
                            //Generar pdf de cita medica

                            GenerarPDF generarPDF = new();
                            string newDocumentFileName = generarPDF.GenerateInvestorDocument(request);

                            //Email de envio de correo de cita medica

                            generarPDF.EnvioNotification(request, newDocumentFileName);
                        }
                        response.data = ocitas;
                        response.mensaje = "Cita creada exitosamente";
                        response.exito = 1;
                    }
                    else 
                    {
                        response.mensaje.Equals("Ha ocurrido un error momento de insertar la cita medica");
                    }
                    
 
                    transaction.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                response.data = ocitas;
                response.mensaje = "Ha ocurrido un error al momento de generar la cita medica";
                response.exito = 0;
            }

            return response;
        }

        public async Task<CitasRequestDto> UpdateCitas(CitasRequestDto request)
        {
            //Extraction Nombre de la especialidad 
            request.NombreEspecialidad = _context.Especialidad.Where(x => x.Codes == request.Codes).Select(x => x.Nombre).FirstOrDefault();
            request.Costo = _context.Especialidad.Where(x => x.Codes == request.Codes).Select(x => x.Costo).FirstOrDefault();
            //end

            var id = await _context.Citas.Where(x => x.Id == request.Id).ToListAsync();
            var HoraFinal = request.Hora.Split(" ");
            foreach (var item in id)
            {
                item.Id = request.Id;
                item.Dnip = request.Dnip;
                item.Codmed = request.Codmed;
                item.Feccit = request.Feccit;
                item.Estado = 1;
                item.Codes = request.Codes;
                item.Hora = TimeSpan.Parse(HoraFinal[0].ToString());
                await _context.SaveChangesAsync();
            }

            if (request.bActiveNotificaciones)
            {
                //Generar pdf de cita medica

                GenerarPDF generarPDF = new();
                string newDocumentFileName = generarPDF.GenerateInvestorDocument(request);

                //Email de envio de correo de cita medica

                generarPDF.EnvioNotification(request, newDocumentFileName);
            }

            return request;
        }

        public async Task<Citas> DeleteCitas(int id)
        {
            var lst = await _context.Citas.FirstOrDefaultAsync(x => x.Id == id);
            _context.Citas.Remove(lst);
            await _context.SaveChangesAsync();
            return lst;
        }

    }
}
