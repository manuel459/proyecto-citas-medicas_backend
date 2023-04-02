using Consulta_medica.Dto.Request;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Transactions;

namespace Consulta_medica.Repository
{
    public class GenerarPDF
    {
        public GenerarPDF()
        {

        }

        public string GenerateInvestorDocument(CitasRequestDto contractInfo)
        {
            DateTime fecha = DateTime.Now;

            string filePath = @"Template\";
            string fileNameExisting = @"cita_medica.pdf";
            //string fileNameNew = @"cita_medica_" +DateTime.Now.Year.ToString()+DateTime.Now.Month.ToString()+DateTime.Now.Day.ToString()+DateTime.Now.Hour+DateTime.Now.Minute.ToString()+DateTime.Now.Second.ToString()+contractInfo.NombrePaciente+ ".pdf";

            string fileNameNew = $"cita_medica_{fecha.Year}{fecha.Month}{fecha.Day}{fecha.Hour}{fecha.Minute}{fecha.Second}{contractInfo.NombrePaciente}.pdf";

            string fullNewPath = filePath + fileNameNew;
            string fullExistingPath = filePath + fileNameExisting;

            using (var existingFileStream = new FileStream(fullExistingPath, FileMode.Open))

            using (var newFileStream = new FileStream(fullNewPath, FileMode.Create))
            {
                // Open existing PDF
                var pdfReader = new PdfReader(existingFileStream);

                // PdfStamper, which will create
                var stamper = new PdfStamper(pdfReader, newFileStream);

                AcroFields fields = stamper.AcroFields;
                fields.SetField("NombrePaciente", contractInfo.NombrePaciente);
                fields.SetField("Dnip", contractInfo.Dnip.ToString());
                fields.SetField("Nombre", contractInfo.NombreMedico);
                fields.SetField("NombreEspecialidad", contractInfo.NombreEspecialidad);
                fields.SetField("Feccit", contractInfo.Feccit.ToString("dd/M/yyyy") + " " + contractInfo.Hora);
                fields.SetField("Costo", contractInfo.Costo.ToString());


                // "Flatten" the form so it wont be editable/usable anymore
                stamper.FormFlattening = true;

                stamper.Close();
                pdfReader.Close();

                return fullNewPath;
            }

        }

        public void EnvioNotification(CitasRequestDto request, string newDocumentFileName)
        {
            string EmailOrigen = "manuel.chirre.sepulveda@gmail.com";

            string contraseña = "oivengxhqmwvfzle";

            MailMessage mailMessage = new MailMessage(EmailOrigen, request.CorreoElectronico, "CITA MEDICA", "<b>Buen dia estimado(a) " + request.NombrePaciente + " adjunto su cita medica<b>");

            mailMessage.Attachments.Add(new Attachment(newDocumentFileName));

            mailMessage.IsBodyHtml = true;


            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Port = 587;
            client.Credentials = new System.Net.NetworkCredential(EmailOrigen, contraseña);
            client.Send(mailMessage);

            client.Dispose();

        }
    }
}
