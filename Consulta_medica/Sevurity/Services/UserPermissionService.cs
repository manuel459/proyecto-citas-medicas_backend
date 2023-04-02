using Consulta_medica.Models;
using Consulta_medica.Sevurity.Middleware;
using Consulta_medica.Static;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Consulta_medica.Sevurity.Services
{
    public class UserPermissionService
    {
        private readonly consulta_medicaContext _dbContext;
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly HttpContext _httpContext;

        public UserPermissionService(consulta_medicaContext dbContext)
        {
            _dbContext = dbContext;
            //_httpContext = httpContext;
        }

        //public string GetEmail()
        //{
        //    if (!_httpContext.User.Identity!.IsAuthenticated)
        //    {
        //        return null;
        //    }

        //    var idClaim = _httpContext.User.Identity.Name;
        //    return idClaim;
        //}

        public async ValueTask<ClaimsIdentity> GetUserPermissionsIdentity(CancellationToken cancellationToken)
        {
            //var userEmail = GetEmail();
            //var user = await _unitOfWork.User.getAsyncEmail("manuel.chirre@materiagris.pe");
            //var roleId = await _dbContext.Roles_Usuarios.FirstOrDefaultAsync(x => x.nId_Usuario == user.nId_Usuario);

            //var tienePermiso = await _dbContext.Permisos_Opciones
            //    .Join(_dbContext.Opciones, po => po.nId_Opcion, o => o.nId_Opcion, (po, o) => new { Permiso_Opcion = po, Opcion = o })
            //    .Where(x => x.Permiso_Opcion.nId_Rol == roleId.nId_Rol && x.Opcion.nEstado == 1 && (new string[] { "ASSISTENCE-LIST-ALL", "ASSISTENCE-LIST-TEAM", "ASSISTENCE-LIST-INDIVIDUAL" })
            //    .Contains(x.Opcion.sSlug))
            //    .Select(x => x.Opcion.sSlug)
            //    .ToListAsync();


            var userPermissions = await (from po in _dbContext.Permisos
                                         where po.idtip == "U001"
                                         select new Claim(AppClaimTypes.Permissions, po.sSlug)).ToListAsync(cancellationToken);

            return CreatePermissionsIdentity(userPermissions);
        }

        private static ClaimsIdentity CreatePermissionsIdentity(IReadOnlyCollection<Claim> claimPermissions)
        {
            if (!claimPermissions.Any())
                return null;

            var permissionsIdentity = new ClaimsIdentity(nameof(PermissionsMiddleware), "name", "role");
            permissionsIdentity.AddClaims(claimPermissions);

            return permissionsIdentity;
        }

    }
}
