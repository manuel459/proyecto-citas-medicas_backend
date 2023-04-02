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

        public object GenerarPdf(List<CitasQueryDtoReport> request)
        {

            //var lista =  (from r in request
            //                   select new CitasRequestDto
            //                   {
            //                       Id = r.Id,
            //                       Dnip = r.Dnip,
            //                       NombrePaciente = _context.Paciente.FirstOrDefault(x => x.Dnip == r.Dnip).Nomp.ToString(),
            //                       Codmed = r.Codmed,
            //                       Nombre = _context.Medico.FirstOrDefault(x => x.Codmed == r.Codmed).Nombre.ToString(),
            //                       Feccit = r.Feccit,
            //                       Estado = r.Estado,
            //                       Hora = r.Hora,
            //                       Codes = r.Codes,
            //                       NombreEspecialidad = _context.Especialidad.FirstOrDefault(x => x.Codes == r.Codes).Nombre.ToString()
            //                   }).ToList();

         
            FileStream fs = new FileStream(@"C:\Users\Toshiba\Downloads\PDFGenerado.pdf", FileMode.Create);
            Document doc = new Document(PageSize.LETTER,5,5,7,7);
            PdfWriter pw = PdfWriter.GetInstance(doc, fs);
            doc.Open();

    
            //Titulo y autor
            doc.AddAuthor("WiredBox");
            doc.AddTitle("Generar pdf");

            //Definir fuente
            iTextSharp.text.Font standarFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);


            //Insertar logo
            iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance("https://www.clinicainternacional.com.pe/static/img/header-logo-colores.png");
            imagen.BorderWidth = 0;
            imagen.Alignment = Element.ALIGN_LEFT;
            float percentage = 0.0f;
            percentage = 150 / imagen.Width;
            imagen.ScalePercent(percentage * 100);
            doc.Add(imagen);
            //Encabezado
            var titulo = new Paragraph("Reporte de cita");
            titulo.Alignment = Element.ALIGN_CENTER;
            doc.Add(titulo);
            doc.Add(Chunk.NEWLINE);

         
            //Encabezado de columnas
            PdfPTable tbl = new PdfPTable(10);
            tbl.WidthPercentage = 100;
            tbl.GetType();

            //Configurando el titulo de las columnas

            PdfPCell clId = new PdfPCell(new Phrase("Nro cita", standarFont));
            clId.BorderWidth = 0;
            clId.BorderWidthBottom = 0.70f;

            PdfPCell clDni = new PdfPCell(new Phrase("Dni del paciente", standarFont));
            clDni.BorderWidth = 0;
            clDni.BorderWidthBottom = 0.70f;

            PdfPCell clNombrePaciente = new PdfPCell(new Phrase("Nombre del paciente", standarFont));
            clNombrePaciente.BorderWidth = 0;
            clNombrePaciente.BorderWidthBottom = 0.70f;

            PdfPCell clNombreMedico = new PdfPCell(new Phrase("Nombre del Medico", standarFont));
            clNombreMedico.BorderWidth = 0;
            clNombreMedico.BorderWidthBottom = 0.70f;

            PdfPCell clFec = new PdfPCell(new Phrase("Fecha", standarFont));
            clFec.BorderWidth = 0;
            clFec.BorderWidthBottom = 0.70f;

            PdfPCell clEstado = new PdfPCell(new Phrase("Estado", standarFont));
            clEstado.BorderWidth = 0;
            clEstado.BorderWidthBottom = 0.70f;

            PdfPCell clEstadoPago = new PdfPCell(new Phrase("Estado de Pago", standarFont));
            clEstadoPago.BorderWidth = 0;
            clEstadoPago.BorderWidthBottom = 0.70f;

            PdfPCell clHora = new PdfPCell(new Phrase("Hora", standarFont));
            clHora.BorderWidth = 0;
            clHora.BorderWidthBottom = 0.70f;

            PdfPCell clEspecialidad = new PdfPCell(new Phrase("Especialidad", standarFont));
            clEspecialidad.BorderWidth = 0;
            clEspecialidad.BorderWidthBottom = 0.70f;

            PdfPCell clCosto = new PdfPCell(new Phrase("Costo", standarFont));
            clCosto.BorderWidth = 0;
            clCosto.BorderWidthBottom = 0.70f;

            tbl.AddCell(clId);
            tbl.AddCell(clDni);
            tbl.AddCell(clNombrePaciente);
            tbl.AddCell(clNombreMedico);
            tbl.AddCell(clFec);
            tbl.AddCell(clEstado);
            tbl.AddCell(clHora);
            tbl.AddCell(clEspecialidad);
            tbl.AddCell(clCosto);
            tbl.AddCell(clEstadoPago);

            foreach (var item in request)
            {
                clId = new PdfPCell(new Phrase(item.id.ToString(), standarFont));
                clId.BorderWidth = 0;

                clDni = new PdfPCell(new Phrase(item.dnip.ToString(), standarFont));
                clDni.BorderWidth = 0;

                clNombrePaciente = new PdfPCell(new Phrase(item.sNombre_Paciente, standarFont));
                clNombrePaciente.BorderWidth = 0;

                clNombreMedico = new PdfPCell(new Phrase(item.sNombre_Medico, standarFont));
                clNombreMedico.BorderWidth = 0;

                clFec = new PdfPCell(new Phrase(item.feccit.ToString("yyyy-MM-dd"), standarFont));
                clFec.BorderWidth = 0;

                clEstado = new PdfPCell(new Phrase(item.sEstado, standarFont));
                clEstado.BorderWidth = 0;

                clHora = new PdfPCell(new Phrase(item.hora.ToString(), standarFont));
                clHora.BorderWidth = 0;

                clEspecialidad = new PdfPCell(new Phrase(item.sNombre_Especialidad, standarFont));
                clEspecialidad.BorderWidth = 0;

                clEstadoPago = new PdfPCell(new Phrase(item.sEstado_Pago, standarFont));
                clEstadoPago.BorderWidth = 0;

                clCosto = new PdfPCell(new Phrase(item.costo.ToString(), standarFont));
                clCosto.BorderWidth = 0;

                tbl.AddCell(clId);
                tbl.AddCell(clDni);
                tbl.AddCell(clNombrePaciente);
                tbl.AddCell(clNombreMedico);
                tbl.AddCell(clFec);
                tbl.AddCell(clHora);
                tbl.AddCell(clEstado);
                tbl.AddCell(clEspecialidad);
                tbl.AddCell(clEstadoPago);
                tbl.AddCell(clCosto);
            }
            doc.Add(tbl);
            doc.Close();
            pw.Close();

            return request;
        }
    }
}
