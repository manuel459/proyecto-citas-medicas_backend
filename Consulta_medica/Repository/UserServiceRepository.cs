using Consulta_medica.Common;
using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Consulta_medica.Repository
{
    public class UserServiceRepository : IUserServiceRepository
    {
        private readonly AppSettings _appSettings;

        public UserServiceRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public UserResponseDto Auth(LogeoRequestDto model)
        {
            UserResponseDto userResponse = new UserResponseDto();

            using (var db = new consulta_medicaContext())
            {
                string contraseña = model.Contraseña;

                var usuario = db.Medico.Where(d => d.Correo == model.CorreoElectronico && d.Pswd == contraseña).FirstOrDefault();
                var administrador = db.Administrador.Where(d => d.Correo == model.CorreoElectronico && d.Pswd == contraseña).FirstOrDefault();
                var recepcion = db.Recepcions.Where(d => d.Correo == model.CorreoElectronico && d.Pswd == contraseña).FirstOrDefault();

                if (usuario != null)
                {
                    userResponse.CorreoElectronico = usuario.Correo;
                    userResponse.Token = GetToken<Medico>(usuario.Codmed.ToString(), usuario.Correo.ToString());
                    var inforUser = db.Medico.FirstOrDefault(x => x.Correo == userResponse.CorreoElectronico);
                    userResponse.Nombre = inforUser.Nombre;
                }
                else if (administrador != null)
                {
                    userResponse.CorreoElectronico = administrador.Correo;
                    userResponse.Token = GetToken<Administrador>(administrador.Codad, administrador.Correo);
                    var inforUser = db.Administrador.FirstOrDefault(x => x.Correo == userResponse.CorreoElectronico);
                    userResponse.Nombre = inforUser.Nombre;
                }
                else if(recepcion != null)
                {
                    userResponse.CorreoElectronico = recepcion.Correo;
                    userResponse.Token = GetToken<Recepcion>(recepcion.codre.ToString(), recepcion.Correo);
                    var inforUser = db.Recepcions.FirstOrDefault(x => x.Correo == userResponse.CorreoElectronico);
                    userResponse.Nombre = inforUser.Nombres + " "+ inforUser.Apellidos;
                }
                  
            }
            return userResponse;
        }


        private string GetToken<T>(string codigo , string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var llave = Encoding.ASCII.GetBytes(_appSettings.Secreto);
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(
                    new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier,codigo.ToString()),
                         new Claim(ClaimTypes.Email, email),
                    }
                    ),
                Expires = DateTime.UtcNow.AddDays(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
