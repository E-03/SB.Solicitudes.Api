using SB.Solicitudes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
        DateTime GetExpiration();
    }
}
