using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Dto
{
    public sealed record LoginResponse(
        string Token,
        DateTime ExpiresAt,
        int UsuarioId,
        string Nombre,
        string Correo,
        string Rol);
}
