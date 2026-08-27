using SB.Solicitudes.Domain.Common.Results;
using SB.Solicitudes.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResultEntity<LoginResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken);
    }
}
