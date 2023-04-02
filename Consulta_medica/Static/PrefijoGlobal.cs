using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Static
{
    public static class DynamicPolicies
    {
        public const string POLICY_PREFIX = "PERMISSION_";
        public const string SEPARATOR = "_";
    }

    public static class Permissions
    {
        public const string LIST_MODULE_MEDICOS = "LIST-MODULE-MEDICOS";
        public const string LIST_MODULE_NEWCITA = "LIST-MODULE-NEWCITA";
        public const string LIST_MODULE_PACIENTES_INDIVIDUAL = "LIST-MODULE-PACIENTES-INDIVIDUAL";
   
    }

    public static class AppClaimTypes
    {
        public const string Permissions = "permissions";
    }
}
