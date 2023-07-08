using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Repository
{
    public class PacienteRepository : IPacienteRepository
    {
        public readonly consulta_medicaContext _context;
        public PacienteRepository(consulta_medicaContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<Paciente>> GetPacientes(RequestGenericFilter request, string usuario)
        {

            var ParamDniPaciente = new SqlParameter("@sDni", DBNull.Value);

            var ParamNombrePaciente = new SqlParameter("@sNombrePaciente", DBNull.Value);

            var ParamNumeroPaciente = new SqlParameter("@sNumero", DBNull.Value);

            var ParamUsuario = new SqlParameter("@sUsuario", usuario != null ? usuario : DBNull.Value);

            if (request.numFilter != null && request.textFilter != null)
            {
                switch (request.numFilter)
                {
                    case 0:
                        ParamDniPaciente.Value = request.textFilter;
                        break;
                    case 1:
                        ParamNombrePaciente.Value = request.textFilter;
                        break;
                    case 2:
                        ParamNumeroPaciente.Value = request.textFilter;
                        break;
                }
            }
            var ParamFechaIni = new SqlParameter("@dFechaInicio", request.dFechaInicio != null ? request.dFechaInicio : DBNull.Value);
            var ParamFechaFin = new SqlParameter("@dFechaFin", request.dFechaFin != null ? request.dFechaFin : DBNull.Value);

            var slgp = filterGeneric(request);

            var queryReponse = await _context.Set<Paciente>()
                .FromSqlRaw("EXECUTE sp_listado_pacientes @sDni, @sNombrePaciente, @sNumero, @sFilterOne, @sFilterTwo, @dFechaInicio, @dFechaFin, @sUsuario",
                parameters: new[] { ParamDniPaciente, ParamNombrePaciente, ParamNumeroPaciente, slgp.pFilterOne, slgp.pFilterTwo, ParamFechaIni, ParamFechaFin, ParamUsuario }).ToListAsync();

            return queryReponse;
        }

        public SqlGenericParameters filterGeneric(RequestGenericFilter request)
        {
            SqlGenericParameters generic = new();
            generic.pFilterOne = new SqlParameter("@sFilterOne", request.sFilterOne != null ? request.sFilterOne : DBNull.Value);
            generic.pFilterTwo = new SqlParameter("@sFilterTwo", request.sFilterTwo != null ? request.sFilterTwo : DBNull.Value);
            return generic;
        }

        public async Task<Paciente> AddPaciente(PacienteRequestDto request)
        {
            try
            {
                Paciente opaciente = new Paciente();
                opaciente.Dnip = request.Dnip;
                opaciente.Idtip = "U003";
                opaciente.Nomp = request.Nomp;
                opaciente.Apellidos = request.Apellidos;
                opaciente.Numero = request.Numero;
                opaciente.Edad = request.edad;
                opaciente.correoElectronico = request.correoElectronico;
                _context.Paciente.Add(opaciente);
                await _context.SaveChangesAsync();
                return opaciente;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        public async Task<PacienteRequestDto> UpdatePaciente(PacienteRequestDto request)
        {
            var id = await _context.Paciente.Where(x => x.Dnip == request.Dnip).ToListAsync();

            foreach (var item in id)
            {
                item.Dnip = request.Dnip;
                item.Nomp = request.Nomp;
                item.Numero = request.Numero;
                item.Edad = request.edad;
                item.correoElectronico = request.correoElectronico;
                await _context.SaveChangesAsync();
            }

            return request;
        }

        public async Task<Paciente> DeletePaciente(int id)
        {
            var lst = await _context.Paciente.FirstOrDefaultAsync(x => x.Dnip == id);
            _context.Paciente.Remove(lst);
            await _context.SaveChangesAsync();
            return lst;
        }
    }
}
