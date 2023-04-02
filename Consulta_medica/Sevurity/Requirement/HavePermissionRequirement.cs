using Consulta_medica.Sevurity.Enum;
using Consulta_medica.Static;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Sevurity.Requirement
{
    public class HavePermissionRequirement  : IAuthorizationRequirement
    {
        public static string ClaimType => AppClaimTypes.Permissions;

        public PermissionOperator PermissionOperator { get; }

        public string[] Permissions { get; }

        public HavePermissionRequirement(PermissionOperator permissionOperator, string[] permissions)
        {
            if (permissions.Length == 0)
            {
                throw new ArgumentException("Se requiere al menos un permiso.", nameof(permissions));
            }

            PermissionOperator = permissionOperator;
            Permissions = permissions;
        }
    }
}
