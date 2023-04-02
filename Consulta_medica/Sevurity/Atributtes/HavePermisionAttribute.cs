using Consulta_medica.Sevurity.Enum;
using Consulta_medica.Static;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Sevurity.Atributtes
{
    public class HavePermisionAttribute : AuthorizeAttribute
    {
        public HavePermisionAttribute(PermissionOperator permissionOperator, params string[] permissions)
        {
            Policy = $"{DynamicPolicies.POLICY_PREFIX}{(int)permissionOperator}{DynamicPolicies.SEPARATOR}{string.Join(DynamicPolicies.SEPARATOR, permissions)}";
        }

        public HavePermisionAttribute(string permission)
        {
            Policy = $"{DynamicPolicies.POLICY_PREFIX}{(int)PermissionOperator.And}{DynamicPolicies.SEPARATOR}{permission}";
        }

        public static PermissionOperator GetOperatorFromPolicy(string policyName)
        {
            var @operator = int.Parse(policyName.AsSpan(DynamicPolicies.POLICY_PREFIX.Length, 1));
            return (PermissionOperator)@operator;
        }

        public static string[] GetPermissionsFromPolicy(string policyName)
        {
            return policyName.Substring(DynamicPolicies.POLICY_PREFIX.Length + 2)
                .Split(new[] { DynamicPolicies.SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
