﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Consulta_medica.Sevurity.Services
{
    public interface IUserPermissionService
    {
        //string GetEmail();
        ValueTask<ClaimsIdentity> GetUserPermissionsIdentity(CancellationToken cancellationToken);
    }
}
