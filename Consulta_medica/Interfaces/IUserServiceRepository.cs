using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta_medica.Interfaces
{
    public interface IUserServiceRepository
    {
        UserResponseDto Auth(LogeoRequestDto model);
    }
}
