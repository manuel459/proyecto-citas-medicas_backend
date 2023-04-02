using Consulta_medica.Sevurity.Enum;
using Consulta_medica.Sevurity.Requirement;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Sevurity.Handler
{
    public class HavePermissionHandler : AuthorizationHandler<HavePermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HavePermissionRequirement requirement)
        {
            if (requirement.PermissionOperator == PermissionOperator.And)
            {
                foreach (var permission in requirement.Permissions)
                {
                    if (!context.User.HasClaim(HavePermissionRequirement.ClaimType, permission))
                    {
                        //si el usuario carece de cualquiera de los permisos requeridos
                        //lo marcamos como fallido
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                //Tiene todos los permisos
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            foreach (var permission in requirement.Permissions)
            {
                if (context.User.HasClaim(HavePermissionRequirement.ClaimType, permission))
                {
                    //En el caso OR, si encontramos un permiso coincidente
                    //lo marcamos como Succeed
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            //No tiene ninguno de los permisos requeridos
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
