# Plataforma de Solicitudes Internas --- Backend .NET 8

> Documento técnico de implementación del backend basado en la Prueba
> Técnica Full Stack de agosto de 2026.
>
> **Alcance de este documento:** únicamente backend. El frontend React +
> TypeScript se construirá posteriormente sobre estos contratos.

## 1. Objetivo

Implementar una API REST empresarial para registrar, consultar y
gestionar solicitudes internas de servicios tecnológicos, con:

-   .NET 8 / ASP.NET Core Web API.
-   Entity Framework Core + SQL Server.
-   Clean Architecture + Onion Architecture.
-   SOLID y Clean Code.
-   Repository + Generic Repository.
-   Unit of Work.
-   Result Pattern.
-   DTOs y validaciones.
-   Paginación y filtros ejecutados en backend.
-   JWT + roles.
-   Swagger con botón **Authorize** para enviar el Bearer Token.
-   Historial de estados y comentarios.
-   Notificaciones desacopladas.
-   Dashboard.
-   Migraciones EF Core.
-   Logging y manejo consistente de errores.
-   Pruebas unitarias/integración como extensión recomendada.

La prueba exige separación de responsabilidades, bajo acoplamiento,
extensibilidad, paginación, filtros, trazabilidad, autorización en
backend y documentación de decisiones técnicas.
fileciteturn0file0L109-L142

## 2. Requerimientos cubiertos

La implementación cubre los endpoints definidos por la prueba:

  -------------------------------------------------------------------------------------
  Método                  Endpoint                              Responsabilidad
  ----------------------- ------------------------------------- -----------------------
  POST                    `/api/auth/login`                     Autenticación + JWT

  GET                     `/api/solicitudes`                    Consulta paginada +
                                                                filtros

  POST                    `/api/solicitudes`                    Crear solicitud

  GET                     `/api/solicitudes/{id}`               Detalle + historial +
                                                                comentarios

  PATCH                   `/api/solicitudes/{id}/estado`        Cambiar estado

  PATCH                   `/api/solicitudes/{id}/asignacion`    Asignar/reasignar
                                                                responsable

  POST                    `/api/solicitudes/{id}/comentarios`   Crear comentario

  GET                     `/api/dashboard/resumen`              Métricas

  GET                     `/api/catalogos/areas`                Áreas activas

  GET                     `/api/catalogos/tipos-solicitud`      Tipos activos
  -------------------------------------------------------------------------------------

Estos endpoints corresponden a los servicios/API sugeridos en el
documento oficial. fileciteturn0file0L202-L221

------------------------------------------------------------------------

# 3. Arquitectura

## 3.1 Estructura de solución

``` text
src/
└── backend/
    ├── SB.Solicitudes.Api/
    │   ├── Controllers/
    │   │   ├── AuthController.cs
    │   │   ├── SolicitudesController.cs
    │   │   ├── DashboardController.cs
    │   │   └── CatalogosController.cs
    │   ├── Extensions/
    │   │   ├── SwaggerExtensions.cs
    │   │   └── ExceptionHandlingExtensions.cs
    │   ├── Middleware/
    │   └── Program.cs
    │
    ├── SB.Solicitudes.Application/
    │   ├── Common/
    │   │   ├── Interfaces/
    │   │   │   ├── IUnitOfWork.cs
    │   │   │   ├── IGenericRepository.cs
    │   │   │   ├── IUsuarioRepository.cs
    │   │   │   ├── ISolicitudRepository.cs
    │   │   │   ├── IAreaRepository.cs
    │   │   │   ├── ITipoSolicitudRepository.cs
    │   │   │   ├── IComentarioRepository.cs
    │   │   │   ├── IHistorialEstadoRepository.cs
    │   │   │   ├── INotificacionRepository.cs
    │   │   │   ├── IJwtTokenGenerator.cs
    │   │   │   ├── INotificationService.cs
    │   │   │   └── ICurrentUserService.cs
    │   │   ├── Models/
    │   │   │   ├── Result.cs
    │   │   │   ├── PagedResult.cs
    │   │   │   └── PagedRequest.cs
    │   │   └── Exceptions/
    │   ├── DTOs/
    │   │   ├── Auth/
    │   │   ├── Solicitudes/
    │   │   ├── Comentarios/
    │   │   └── Dashboard/
    │   ├── Services/
    │   │   ├── AuthService.cs
    │   │   ├── SolicitudService.cs
    │   │   ├── DashboardService.cs
    │   │   └── CatalogoService.cs
    │   ├── Validators/
    │   └── DependencyInjection.cs
    │
    ├── SB.Solicitudes.Domain/
    │   ├── Common/
    │   │   └── BaseEntity.cs
    │   ├── Entities/
    │   ├── Enums/
    │   └── Rules/
    │
    ├── SB.Solicitudes.Infrastructure/
    │   ├── Persistence/
    │   │   ├── ApplicationDbContext.cs
    │   │   ├── Configurations/
    │   │   ├── Repositories/
    │   │   ├── UnitOfWork.cs
    │   │   └── Migrations/
    │   ├── Security/
    │   │   ├── JwtTokenGenerator.cs
    │   │   └── PasswordHasher.cs
    │   ├── Notifications/
    │   │   └── DatabaseNotificationService.cs
    │   ├── CurrentUser/
    │   │   └── CurrentUserService.cs
    │   └── DependencyInjection.cs
    │
    └── SB.Solicitudes.Tests/
        ├── Application/
        ├── Infrastructure/
        └── Api/
```

## 3.2 Regla de dependencias

``` text
Api
 ↓
Application
 ↓
Domain

Infrastructure
 ↓
Application
 ↓
Domain
```

**Domain no conoce Infrastructure, EF Core, JWT, HTTP ni ASP.NET Core.**

**Application no conoce SQL Server ni EF Core.**

**Infrastructure implementa las interfaces definidas por Application.**

**Api solamente coordina HTTP y delega la lógica al Application.**

Esto cumple el objetivo de separar responsabilidades y mantener bajo
acoplamiento. La prueba evalúa explícitamente arquitectura,
responsabilidades claras y extensibilidad.
fileciteturn0file0L117-L128

------------------------------------------------------------------------

# 4. Paquetes NuGet

## Domain

No requiere paquetes externos.

## Application

``` bash
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

## Infrastructure

``` bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.Extensions.Identity.Core
dotnet add package System.IdentityModel.Tokens.Jwt
```

## Api

``` bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
```

------------------------------------------------------------------------

# 5. Domain

## 5.1 BaseEntity

``` csharp
namespace SB.Solicitudes.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
}
```

## 5.2 Enums

``` csharp
namespace SB.Solicitudes.Domain.Enums;

public enum RolUsuario
{
    Administrador,
    Analista,
    Solicitante
}

public enum EstadoSolicitud
{
    Registrada,
    EnAnalisis,
    EnProgreso,
    EnEsperaDelSolicitante,
    Resuelta,
    Cerrada
}

public enum PrioridadSolicitud
{
    Baja,
    Media,
    Alta,
    Critica
}

public enum VisibilidadComentario
{
    Interno,
    Publico
}
```

Se mantienen los roles mínimos exigidos por la prueba: Administrador,
Analista y Solicitante. fileciteturn0file0L99-L103

------------------------------------------------------------------------

# 6. Entidades

## 6.1 Usuario

``` csharp
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities;

public class Usuario : BaseEntity
{
    private Usuario() { }

    public Usuario(
        string nombre,
        string correo,
        string contraseñaHash,
        string rol)
    {
        Nombre = nombre;
        Correo = correo;
        ContraseñaHash = contraseñaHash;
        Rol = rol;
        Activo = true;
    }

    public string Nombre { get; private set; } = null!;
    public string Correo { get; private set; } = null!;
    public string ContraseñaHash { get; private set; } = null!;
    public string Rol { get; private set; } = null!;
    public bool Activo { get; private set; }

    public ICollection<Solicitud> SolicitudesCreadas { get; private set; }
        = new List<Solicitud>();

    public ICollection<Solicitud> SolicitudesAsignadas { get; private set; }
        = new List<Solicitud>();

    public ICollection<Comentario> Comentarios { get; private set; }
        = new List<Comentario>();
}
```

## 6.2 Área

``` csharp
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities;

public class Area : BaseEntity
{
    private Area() { }

    public Area(string nombre)
    {
        Nombre = nombre;
        Activa = true;
    }

    public string Nombre { get; private set; } = null!;
    public bool Activa { get; private set; }

    public ICollection<Solicitud> Solicitudes { get; private set; }
        = new List<Solicitud>();
}
```

## 6.3 TipoSolicitud

``` csharp
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities;

public class TipoSolicitud : BaseEntity
{
    private TipoSolicitud() { }

    public TipoSolicitud(string nombre, string? descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = true;
    }

    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; }

    public ICollection<Solicitud> Solicitudes { get; private set; }
        = new List<Solicitud>();
}
```

## 6.4 Solicitud

``` csharp
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Enums;

namespace SB.Solicitudes.Domain.Entities;

public class Solicitud : BaseEntity
{
    private Solicitud() { }

    public Solicitud(
        string codigo,
        string titulo,
        string descripcion,
        PrioridadSolicitud prioridad,
        DateTime fechaCompromiso,
        int usuarioSolicitanteId,
        int areaId,
        int tipoSolicitudId,
        string? evidenciaUrl)
    {
        Codigo = codigo;
        Titulo = titulo;
        Descripcion = descripcion;
        Prioridad = prioridad.ToString();
        Estado = EstadoSolicitud.Registrada.ToString();
        FechaCreacion = DateTime.UtcNow;
        FechaCompromiso = fechaCompromiso;
        UsuarioSolicitanteId = usuarioSolicitanteId;
        AreaId = areaId;
        TipoSolicitudId = tipoSolicitudId;
        EvidenciaUrl = evidenciaUrl;
    }

    public string Codigo { get; private set; } = null!;
    public string Titulo { get; private set; } = null!;
    public string Descripcion { get; private set; } = null!;
    public string Prioridad { get; private set; } = null!;
    public string Estado { get; private set; } = null!;
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaCompromiso { get; private set; }
    public string? EvidenciaUrl { get; private set; }

    public int UsuarioSolicitanteId { get; private set; }
    public Usuario UsuarioSolicitante { get; private set; } = null!;

    public int? ResponsableId { get; private set; }
    public Usuario? Responsable { get; private set; }

    public int AreaId { get; private set; }
    public Area Area { get; private set; } = null!;

    public int TipoSolicitudId { get; private set; }
    public TipoSolicitud TipoSolicitud { get; private set; } = null!;

    public void CambiarEstado(EstadoSolicitud nuevoEstado)
    {
        Estado = nuevoEstado.ToString();
    }

    public void AsignarResponsable(int responsableId)
    {
        ResponsableId = responsableId;
    }
}
```

## 6.5 HistorialEstado

``` csharp
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities;

public class HistorialEstado : BaseEntity
{
    private HistorialEstado() { }

    public HistorialEstado(
        int solicitudId,
        int usuarioId,
        string estadoAnterior,
        string estadoNuevo,
        string comentario)
    {
        SolicitudId = solicitudId;
        UsuarioId = usuarioId;
        EstadoAnterior = estadoAnterior;
        EstadoNuevo = estadoNuevo;
        Comentario = comentario;
        Fecha = DateTime.UtcNow;
    }

    public string EstadoAnterior { get; private set; } = null!;
    public string EstadoNuevo { get; private set; } = null!;
    public DateTime Fecha { get; private set; }
    public string Comentario { get; private set; } = null!;

    public int SolicitudId { get; private set; }
    public Solicitud Solicitud { get; private set; } = null!;

    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;
}
```

## 6.6 Comentario

``` csharp
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities;

public class Comentario : BaseEntity
{
    private Comentario() { }

    public Comentario(
        int solicitudId,
        int usuarioId,
        string texto,
        string visibilidad)
    {
        SolicitudId = solicitudId;
        UsuarioId = usuarioId;
        Texto = texto;
        Visibilidad = visibilidad;
        Fecha = DateTime.UtcNow;
    }

    public string Texto { get; private set; } = null!;
    public string Visibilidad { get; private set; } = null!;
    public DateTime Fecha { get; private set; }

    public int SolicitudId { get; private set; }
    public Solicitud Solicitud { get; private set; } = null!;

    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;
}
```

## 6.7 Notificacion

``` csharp
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities;

public class Notificacion : BaseEntity
{
    private Notificacion() { }

    public Notificacion(
        int solicitudId,
        int usuarioDestinoId,
        string canal,
        string asunto,
        string mensaje)
    {
        SolicitudId = solicitudId;
        UsuarioDestinoId = usuarioDestinoId;
        Canal = canal;
        Asunto = asunto;
        Mensaje = mensaje;
        Estado = "Pendiente";
        Fecha = DateTime.UtcNow;
    }

    public string Canal { get; private set; } = null!;
    public string Asunto { get; private set; } = null!;
    public string Mensaje { get; private set; } = null!;
    public string Estado { get; private set; } = null!;
    public DateTime Fecha { get; private set; }

    public int SolicitudId { get; private set; }
    public Solicitud Solicitud { get; private set; } = null!;

    public int UsuarioDestinoId { get; private set; }
    public Usuario UsuarioDestino { get; private set; } = null!;
}
```

------------------------------------------------------------------------

# 7. Application --- Result Pattern

El patrón Result evita utilizar excepciones como mecanismo normal de
control de flujo.

``` csharp
namespace SB.Solicitudes.Application.Common.Models;

public enum ErrorType
{
    Validation,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    Failure
}

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type);

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error)
        => new(false, error);
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
        => new(true, value, null);

    public static Result<T> Failure(Error error)
        => new(false, default, error);
}
```

------------------------------------------------------------------------

# 8. Paginación

## 8.1 PagedRequest

``` csharp
namespace SB.Solicitudes.Application.Common.Models;

public sealed class PagedRequest
{
    private const int MaxPageSize = 100;

    private int _page = 1;
    private int _pageSize = 20;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}
```

## 8.2 PagedResult

``` csharp
namespace SB.Solicitudes.Application.Common.Models;

public sealed class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; init; }
        = Array.Empty<T>();

    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }

    public int TotalPages =>
        PageSize == 0
            ? 0
            : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
```

------------------------------------------------------------------------

# 9. Contratos de repositorio

## 9.1 IGenericRepository

``` csharp
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Application.Common.Interfaces;

public interface IGenericRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default);
}
```

## 9.2 Repositorios específicos

``` csharp
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Common.Interfaces;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default);
}

public interface ISolicitudRepository : IGenericRepository<Solicitud>
{
    Task<Solicitud?> GetDetalleAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<string?> ObtenerUltimoCodigoAsync(
        CancellationToken cancellationToken = default);

    Task<int> CountByEstadoAsync(
        string estado,
        CancellationToken cancellationToken = default);

    Task<int> CountByPrioridadAsync(
        string prioridad,
        CancellationToken cancellationToken = default);

    Task<int> CountVencidasAsync(
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Solicitud>> GetUltimasAsync(
        int cantidad,
        CancellationToken cancellationToken = default);
}

public interface IAreaRepository : IGenericRepository<Area>
{
    Task<IReadOnlyCollection<Area>> GetActivasAsync(
        CancellationToken cancellationToken = default);
}

public interface ITipoSolicitudRepository : IGenericRepository<TipoSolicitud>
{
    Task<IReadOnlyCollection<TipoSolicitud>> GetActivosAsync(
        CancellationToken cancellationToken = default);
}

public interface IComentarioRepository : IGenericRepository<Comentario>
{
    Task<IReadOnlyCollection<Comentario>> GetBySolicitudIdAsync(
        int solicitudId,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteComentarioResolucionAsync(
        int solicitudId,
        CancellationToken cancellationToken = default);
}

public interface IHistorialEstadoRepository : IGenericRepository<HistorialEstado>
{
    Task<IReadOnlyCollection<HistorialEstado>> GetBySolicitudIdAsync(
        int solicitudId,
        CancellationToken cancellationToken = default);
}

public interface INotificacionRepository : IGenericRepository<Notificacion>
{
}
```

------------------------------------------------------------------------

# 10. Unit of Work

## IUnitOfWork

``` csharp
namespace SB.Solicitudes.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IUsuarioRepository Usuarios { get; }
    ISolicitudRepository Solicitudes { get; }
    IAreaRepository Areas { get; }
    ITipoSolicitudRepository TiposSolicitud { get; }
    IComentarioRepository Comentarios { get; }
    IHistorialEstadoRepository HistorialEstados { get; }
    INotificacionRepository Notificaciones { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
```

## UnitOfWork

``` csharp
using SB.Solicitudes.Application.Common.Interfaces;

namespace SB.Solicitudes.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        IUsuarioRepository usuarios,
        ISolicitudRepository solicitudes,
        IAreaRepository areas,
        ITipoSolicitudRepository tiposSolicitud,
        IComentarioRepository comentarios,
        IHistorialEstadoRepository historialEstados,
        INotificacionRepository notificaciones)
    {
        _context = context;
        Usuarios = usuarios;
        Solicitudes = solicitudes;
        Areas = areas;
        TiposSolicitud = tiposSolicitud;
        Comentarios = comentarios;
        HistorialEstados = historialEstados;
        Notificaciones = notificaciones;
    }

    public IUsuarioRepository Usuarios { get; }
    public ISolicitudRepository Solicitudes { get; }
    public IAreaRepository Areas { get; }
    public ITipoSolicitudRepository TiposSolicitud { get; }
    public IComentarioRepository Comentarios { get; }
    public IHistorialEstadoRepository HistorialEstados { get; }
    public INotificacionRepository Notificaciones { get; }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
```

------------------------------------------------------------------------

# 11. GenericRepository

``` csharp
using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public virtual Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
        => DbSet.AddAsync(entity, cancellationToken).AsTask();

    public virtual void Update(TEntity entity)
        => DbSet.Update(entity);

    public virtual void Remove(TEntity entity)
        => DbSet.Remove(entity);

    public virtual Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(x => x.Id == id, cancellationToken);
}
```

------------------------------------------------------------------------

# 12. SolicitudRepository

La paginación de solicitudes no debe cargar todas las filas en memoria.
Los filtros deben aplicarse sobre `IQueryable` antes de `Count`, `Skip`
y `Take`.

``` csharp
using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories;

public sealed class SolicitudRepository
    : GenericRepository<Solicitud>, ISolicitudRepository
{
    public SolicitudRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Solicitud?> GetDetalleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.UsuarioSolicitante)
            .Include(x => x.Responsable)
            .Include(x => x.Area)
            .Include(x => x.TipoSolicitud)
            .Include(x => x.Comentarios)
            .Include(x => x.HistorialEstados)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<string?> ObtenerUltimoCodigoAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => x.Codigo)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> CountByEstadoAsync(
        string estado,
        CancellationToken cancellationToken = default)
        => DbSet.CountAsync(x => x.Estado == estado, cancellationToken);

    public Task<int> CountByPrioridadAsync(
        string prioridad,
        CancellationToken cancellationToken = default)
        => DbSet.CountAsync(x => x.Prioridad == prioridad, cancellationToken);

    public Task<int> CountVencidasAsync(
        DateTime now,
        CancellationToken cancellationToken = default)
        => DbSet.CountAsync(
            x => x.FechaCompromiso < now &&
                 x.Estado != "Cerrada" &&
                 x.Estado != "Resuelta",
            cancellationToken);

    public async Task<IReadOnlyCollection<Solicitud>> GetUltimasAsync(
        int cantidad,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.Responsable)
            .OrderByDescending(x => x.FechaCreacion)
            .Take(cantidad)
            .ToListAsync(cancellationToken);
    }
}
```

> Nota: si las colecciones `Comentarios` e `HistorialEstados` no existen
> como propiedades de navegación en `Solicitud`, no se deben agregar
> artificialmente en Infrastructure. En ese caso el detalle debe
> consultar ambos repositorios por `SolicitudId`. La implementación debe
> respetar el modelo real del proyecto.

------------------------------------------------------------------------

# 13. Repositorios restantes

``` csharp
using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories;

public sealed class UsuarioRepository
    : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public Task<Usuario?> GetByCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default)
        => DbSet
            .FirstOrDefaultAsync(
                x => x.Correo == correo,
                cancellationToken);
}

public sealed class AreaRepository
    : GenericRepository<Area>, IAreaRepository
{
    public AreaRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<Area>> GetActivasAsync(
        CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .Where(x => x.Activa)
            .OrderBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
}

public sealed class TipoSolicitudRepository
    : GenericRepository<TipoSolicitud>, ITipoSolicitudRepository
{
    public TipoSolicitudRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<TipoSolicitud>> GetActivosAsync(
        CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
}

public sealed class ComentarioRepository
    : GenericRepository<Comentario>, IComentarioRepository
{
    public ComentarioRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<Comentario>> GetBySolicitudIdAsync(
        int solicitudId,
        CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.SolicitudId == solicitudId)
            .OrderBy(x => x.Fecha)
            .ToListAsync(cancellationToken);

    public Task<bool> ExisteComentarioResolucionAsync(
        int solicitudId,
        CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(
            x => x.SolicitudId == solicitudId &&
                 x.Visibilidad == "Interno" &&
                 x.Texto.Contains("Resolución"),
            cancellationToken);
}

public sealed class HistorialEstadoRepository
    : GenericRepository<HistorialEstado>, IHistorialEstadoRepository
{
    public HistorialEstadoRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<HistorialEstado>> GetBySolicitudIdAsync(
        int solicitudId,
        CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.SolicitudId == solicitudId)
            .OrderBy(x => x.Fecha)
            .ToListAsync(cancellationToken);
}

public sealed class NotificacionRepository
    : GenericRepository<Notificacion>, INotificacionRepository
{
    public NotificacionRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
```

------------------------------------------------------------------------

# 14. ApplicationDbContext

``` csharp
using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<TipoSolicitud> TiposSolicitud => Set<TipoSolicitud>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<HistorialEstado> HistorialEstados => Set<HistorialEstado>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
```

Las configuraciones de entidades deben permanecer en
`Infrastructure/Persistence/Configurations`, tal como se planteó
previamente.

------------------------------------------------------------------------

# 15. Filtros de solicitudes

## SolicitudFilter

``` csharp
namespace SB.Solicitudes.Application.DTOs.Solicitudes;

public sealed class SolicitudFilter
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }

    public string? Estado { get; init; }
    public string? Prioridad { get; init; }

    public int? AreaId { get; init; }
    public int? UsuarioSolicitanteId { get; init; }
    public int? ResponsableId { get; init; }

    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
}
```

Para la implementación final, este filtro debe convertirse en un
`IQueryable<Solicitud>` dentro del repositorio específico o mediante
Specification. La prueba permite explícitamente Specification como extra
valorado. fileciteturn0file0L237-L243

------------------------------------------------------------------------

# 16. DTOs

## Login

``` csharp
public sealed record LoginRequest(
    string Correo,
    string Contraseña);

public sealed record LoginResponse(
    string Token,
    DateTime ExpiraEn,
    int UsuarioId,
    string Nombre,
    string Rol);
```

## Crear solicitud

``` csharp
public sealed record CrearSolicitudRequest(
    string Titulo,
    string Descripcion,
    int TipoSolicitudId,
    string Prioridad,
    int AreaId,
    DateTime FechaCompromiso,
    string? EvidenciaUrl);
```

El usuario solicitante se obtiene del JWT, no del body. Esto evita que
un usuario pueda crear una solicitud atribuyéndola a otro usuario.

## Cambio de estado

``` csharp
public sealed record CambiarEstadoRequest(
    string Estado,
    string Comentario);
```

## Asignación

``` csharp
public sealed record AsignarResponsableRequest(
    int ResponsableId);
```

## Comentario

``` csharp
public sealed record CrearComentarioRequest(
    string Texto,
    string Visibilidad);
```

------------------------------------------------------------------------

# 17. Current User

La capa Application necesita saber quién está autenticado, pero no debe
depender de `HttpContext`.

## ICurrentUserService

``` csharp
namespace SB.Solicitudes.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Role { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(string role);
}
```

## Implementación

``` csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SB.Solicitudes.Application.Common.Interfaces;

namespace SB.Solicitudes.Infrastructure.CurrentUser;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var value = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public string? Role =>
        _httpContextAccessor
            .HttpContext?
            .User
            .FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated
        == true;

    public bool IsInRole(string role)
        => _httpContextAccessor.HttpContext?
            .User.IsInRole(role) == true;
}
```

------------------------------------------------------------------------

# 18. JWT

## IJwtTokenGenerator

``` csharp
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Usuario usuario);
}
```

## JwtSettings

``` csharp
namespace SB.Solicitudes.Infrastructure.Security;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpirationMinutes { get; set; } = 60;
}
```

## JwtTokenGenerator

``` csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.Nombre),

            new Claim(
                ClaimTypes.Email,
                usuario.Correo),

            new Claim(
                ClaimTypes.Role,
                usuario.Rol)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _settings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
```

**Nunca colocar la clave JWT real en Git.** Usar
`appsettings.Development.json` local, User Secrets o variables de
entorno. La prueba exige que los parámetros sensibles y cadenas de
conexión no sean secretos hardcodeados. fileciteturn0file0L147-L158

------------------------------------------------------------------------

# 19. Password hashing

Para este ejercicio se puede utilizar el hasher de ASP.NET Core Identity
sin convertir `Usuario` en `IdentityUser`.

``` csharp
using Microsoft.AspNetCore.Identity;

namespace SB.Solicitudes.Infrastructure.Security;

public sealed class PasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
        => _hasher.HashPassword(new object(), password);

    public bool Verify(string hash, string password)
        => _hasher.VerifyHashedPassword(
            new object(),
            hash,
            password) == PasswordVerificationResult.Success;
}
```

La prueba permite Identity o un modelo propio simplificado para
usuarios. fileciteturn0file0L163-L174

------------------------------------------------------------------------

# 20. AuthService

``` csharp
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Auth;

namespace SB.Solicitudes.Application.Services;

public sealed class AuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        PasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var usuario = await _unitOfWork.Usuarios
            .GetByCorreoAsync(
                request.Correo,
                cancellationToken);

        if (usuario is null || !usuario.Activo)
        {
            return Result<LoginResponse>.Failure(
                new Error(
                    "AUTH_INVALID",
                    "Credenciales inválidas.",
                    ErrorType.Unauthorized));
        }

        var validPassword = _passwordHasher.Verify(
            usuario.ContraseñaHash,
            request.Contraseña);

        if (!validPassword)
        {
            return Result<LoginResponse>.Failure(
                new Error(
                    "AUTH_INVALID",
                    "Credenciales inválidas.",
                    ErrorType.Unauthorized));
        }

        var token = _jwtTokenGenerator.GenerateToken(usuario);

        return Result<LoginResponse>.Success(
            new LoginResponse(
                token,
                DateTime.UtcNow.AddMinutes(60),
                usuario.Id,
                usuario.Nombre,
                usuario.Rol));
    }
}
```

------------------------------------------------------------------------

# 21. Notificaciones desacopladas

La prueba exige que el mecanismo de notificación pueda cambiar sin
reescribir la lógica principal. fileciteturn0file0L88-L98

## INotificationService

``` csharp
namespace SB.Solicitudes.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyAsync(
        int solicitudId,
        int usuarioDestinoId,
        string asunto,
        string mensaje,
        CancellationToken cancellationToken = default);
}
```

## DatabaseNotificationService

``` csharp
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Notifications;

public sealed class DatabaseNotificationService
    : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public DatabaseNotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task NotifyAsync(
        int solicitudId,
        int usuarioDestinoId,
        string asunto,
        string mensaje,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notificacion(
            solicitudId,
            usuarioDestinoId,
            "Database",
            asunto,
            mensaje);

        await _unitOfWork.Notificaciones.AddAsync(
            notification,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

Posteriormente puede reemplazarse por:

``` text
INotificationService
       │
       ├── DatabaseNotificationService
       ├── EmailNotificationService
       └── RabbitMqNotificationService
```

La lógica de solicitudes no cambia.

------------------------------------------------------------------------

# 22. SolicitudService

Toda la lógica de negocio permanece en Application. El Controller no
decide estados, permisos, códigos, relaciones ni reglas.

``` csharp
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Enums;

namespace SB.Solicitudes.Application.Services;

public sealed class SolicitudService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public SolicitudService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<Result<PagedResult<SolicitudListItemDto>>> GetAsync(
        SolicitudFilter filter,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<PagedResult<SolicitudListItemDto>>.Failure(
                new Error(
                    "AUTH_REQUIRED",
                    "Debe autenticarse.",
                    ErrorType.Unauthorized));
        }

        // La consulta concreta debe construirse como IQueryable
        // en ISolicitudRepository para aplicar filtros en SQL.
        // Este método representa el contrato de aplicación.

        return Result<PagedResult<SolicitudListItemDto>>.Failure(
            new Error(
                "NOT_IMPLEMENTED",
                "Implementar consulta paginada en el repositorio específico.",
                ErrorType.Failure));
    }

    public async Task<Result<SolicitudDetailDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var solicitud = await _unitOfWork.Solicitudes
            .GetDetalleAsync(id, cancellationToken);

        if (solicitud is null)
        {
            return Result<SolicitudDetailDto>.Failure(
                new Error(
                    "SOLICITUD_NOT_FOUND",
                    "Solicitud no encontrada.",
                    ErrorType.NotFound));
        }

        if (!PuedeConsultar(solicitud))
        {
            return Result<SolicitudDetailDto>.Failure(
                new Error(
                    "SOLICITUD_FORBIDDEN",
                    "No tiene permisos para consultar esta solicitud.",
                    ErrorType.Forbidden));
        }

        var comentarios = await _unitOfWork.Comentarios
            .GetBySolicitudIdAsync(id, cancellationToken);

        var historial = await _unitOfWork.HistorialEstados
            .GetBySolicitudIdAsync(id, cancellationToken);

        var response = SolicitudMapper.ToDetail(
            solicitud,
            comentarios,
            historial);

        return Result<SolicitudDetailDto>.Success(response);
    }

    public async Task<Result<SolicitudDetailDto>> CrearAsync(
        CrearSolicitudRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null)
        {
            return Result<SolicitudDetailDto>.Failure(
                new Error(
                    "AUTH_REQUIRED",
                    "Debe autenticarse.",
                    ErrorType.Unauthorized));
        }

        if (!await _unitOfWork.Areas
                .ExistsAsync(request.AreaId, cancellationToken))
        {
            return Result<SolicitudDetailDto>.Failure(
                new Error(
                    "AREA_NOT_FOUND",
                    "El área no existe.",
                    ErrorType.Validation));
        }

        if (!await _unitOfWork.TiposSolicitud
                .ExistsAsync(request.TipoSolicitudId, cancellationToken))
        {
            return Result<SolicitudDetailDto>.Failure(
                new Error(
                    "TIPO_NOT_FOUND",
                    "El tipo de solicitud no existe.",
                    ErrorType.Validation));
        }

        var codigo = await GenerarCodigoAsync(
            cancellationToken);

        if (!Enum.TryParse<PrioridadSolicitud>(
                request.Prioridad,
                true,
                out var prioridad))
        {
            return Result<SolicitudDetailDto>.Failure(
                new Error(
                    "INVALID_PRIORITY",
                    "Prioridad inválida.",
                    ErrorType.Validation));
        }

        var solicitud = new Solicitud(
            codigo,
            request.Titulo,
            request.Descripcion,
            prioridad,
            request.FechaCompromiso,
            _currentUser.UserId.Value,
            request.AreaId,
            request.TipoSolicitudId,
            request.EvidenciaUrl);

        await _unitOfWork.Solicitudes
            .AddAsync(solicitud, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyAsync(
            solicitud.Id,
            solicitud.UsuarioSolicitanteId,
            "Solicitud creada",
            $"Se creó la solicitud {solicitud.Codigo}.",
            cancellationToken);

        return await GetByIdAsync(
            solicitud.Id,
            cancellationToken);
    }

    public async Task<Result> CambiarEstadoAsync(
        int id,
        CambiarEstadoRequest request,
        CancellationToken cancellationToken)
    {
        var solicitud = await _unitOfWork.Solicitudes
            .GetByIdAsync(id, cancellationToken);

        if (solicitud is null)
        {
            return Result.Failure(
                new Error(
                    "SOLICITUD_NOT_FOUND",
                    "Solicitud no encontrada.",
                    ErrorType.NotFound));
        }

        if (_currentUser.UserId is null)
        {
            return Result.Failure(
                new Error(
                    "AUTH_REQUIRED",
                    "Debe autenticarse.",
                    ErrorType.Unauthorized));
        }

        if (!Enum.TryParse<EstadoSolicitud>(
                request.Estado,
                true,
                out var nuevoEstado))
        {
            return Result.Failure(
                new Error(
                    "INVALID_STATE",
                    "Estado inválido.",
                    ErrorType.Validation));
        }

        if (string.IsNullOrWhiteSpace(request.Comentario))
        {
            return Result.Failure(
                new Error(
                    "COMMENT_REQUIRED",
                    "El comentario es obligatorio.",
                    ErrorType.Validation));
        }

        if (nuevoEstado == EstadoSolicitud.Cerrada)
        {
            var tieneResolucion = await _unitOfWork.Comentarios
                .ExisteComentarioResolucionAsync(
                    id,
                    cancellationToken);

            if (!tieneResolucion)
            {
                return Result.Failure(
                    new Error(
                        "RESOLUTION_REQUIRED",
                        "No se puede cerrar la solicitud sin comentario de resolución.",
                        ErrorType.Validation));
            }
        }

        if (solicitud.Estado == EstadoSolicitud.Cerrada.ToString() &&
            nuevoEstado != EstadoSolicitud.Cerrada &&
            !_currentUser.IsInRole("Administrador") &&
            !_currentUser.IsInRole("Analista"))
        {
            return Result.Failure(
                new Error(
                    "REOPEN_FORBIDDEN",
                    "Solo Administrador o Analista pueden reabrir una solicitud cerrada.",
                    ErrorType.Forbidden));
        }

        var anterior = solicitud.Estado;

        solicitud.CambiarEstado(nuevoEstado);

        var historial = new HistorialEstado(
            solicitud.Id,
            _currentUser.UserId.Value,
            anterior,
            nuevoEstado.ToString(),
            request.Comentario);

        await _unitOfWork.HistorialEstados
            .AddAsync(historial, cancellationToken);

        _unitOfWork.Solicitudes.Update(solicitud);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyAsync(
            solicitud.Id,
            solicitud.UsuarioSolicitanteId,
            "Cambio de estado",
            $"La solicitud {solicitud.Codigo} cambió de {anterior} a {nuevoEstado}.",
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AsignarAsync(
        int id,
        AsignarResponsableRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Administrador") &&
            !_currentUser.IsInRole("Analista"))
        {
            return Result.Failure(
                new Error(
                    "ASSIGN_FORBIDDEN",
                    "No tiene permisos para asignar solicitudes.",
                    ErrorType.Forbidden));
        }

        var solicitud = await _unitOfWork.Solicitudes
            .GetByIdAsync(id, cancellationToken);

        if (solicitud is null)
        {
            return Result.Failure(
                new Error(
                    "SOLICITUD_NOT_FOUND",
                    "Solicitud no encontrada.",
                    ErrorType.NotFound));
        }

        var responsable = await _unitOfWork.Usuarios
            .GetByIdAsync(request.ResponsableId, cancellationToken);

        if (responsable is null ||
            !responsable.Activo ||
            responsable.Rol != "Analista")
        {
            return Result.Failure(
                new Error(
                    "RESPONSABLE_INVALID",
                    "El responsable debe ser un analista activo.",
                    ErrorType.Validation));
        }

        solicitud.AsignarResponsable(responsable.Id);

        _unitOfWork.Solicitudes.Update(solicitud);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyAsync(
            solicitud.Id,
            responsable.Id,
            "Solicitud asignada",
            $"Se te ha asignado la solicitud {solicitud.Codigo}.",
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AgregarComentarioAsync(
        int id,
        CrearComentarioRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            return Result.Failure(
                new Error(
                    "AUTH_REQUIRED",
                    "Debe autenticarse.",
                    ErrorType.Unauthorized));
        }

        var solicitud = await _unitOfWork.Solicitudes
            .GetByIdAsync(id, cancellationToken);

        if (solicitud is null)
        {
            return Result.Failure(
                new Error(
                    "SOLICITUD_NOT_FOUND",
                    "Solicitud no encontrada.",
                    ErrorType.NotFound));
        }

        if (!Enum.TryParse<VisibilidadComentario>(
                request.Visibilidad,
                true,
                out var visibilidad))
        {
            return Result.Failure(
                new Error(
                    "INVALID_VISIBILITY",
                    "Visibilidad inválida.",
                    ErrorType.Validation));
        }

        if (visibilidad == VisibilidadComentario.Interno &&
            !_currentUser.IsInRole("Administrador") &&
            !_currentUser.IsInRole("Analista"))
        {
            return Result.Failure(
                new Error(
                    "INTERNAL_COMMENT_FORBIDDEN",
                    "Solo Administrador o Analista pueden crear comentarios internos.",
                    ErrorType.Forbidden));
        }

        var comentario = new Comentario(
            id,
            _currentUser.UserId.Value,
            request.Texto,
            visibilidad.ToString());

        await _unitOfWork.Comentarios
            .AddAsync(comentario, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private bool PuedeConsultar(Solicitud solicitud)
    {
        if (_currentUser.IsInRole("Administrador"))
            return true;

        if (_currentUser.IsInRole("Analista"))
            return solicitud.ResponsableId is null ||
                   solicitud.ResponsableId == _currentUser.UserId;

        return solicitud.UsuarioSolicitanteId == _currentUser.UserId;
    }

    private async Task<string> GenerarCodigoAsync(
        CancellationToken cancellationToken)
    {
        var ultimo = await _unitOfWork.Solicitudes
            .ObtenerUltimoCodigoAsync(cancellationToken);

        var numero = 1;

        if (!string.IsNullOrWhiteSpace(ultimo))
        {
            var partes = ultimo.Split('-');

            if (partes.Length == 3 &&
                int.TryParse(partes[2], out var actual))
            {
                numero = actual + 1;
            }
        }

        return $"SOL-{DateTime.UtcNow.Year}-{numero:D4}";
    }
}
```

> En producción, la generación de códigos debe tener una estrategia
> transaccional o secuencia SQL para evitar colisiones bajo
> concurrencia. Para la prueba técnica, el formato `SOL-2026-0001`
> satisface el requisito funcional, pero la decisión de concurrencia
> debe quedar documentada.

------------------------------------------------------------------------

# 23. Controllers

Los controllers contienen exclusivamente:

1.  Recepción HTTP.
2.  Model binding.
3.  Delegación.
4.  Conversión de `Result` a respuesta HTTP.

No contienen reglas de negocio.

## AuthController

``` csharp
using Microsoft.AspNetCore.Mvc;
using SB.Solicitudes.Application.DTOs.Auth;
using SB.Solicitudes.Application.Services;

namespace SB.Solicitudes.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.LoginAsync(
            request,
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToActionResult();
    }
}
```

## SolicitudesController

``` csharp
[ApiController]
[Route("api/solicitudes")]
[Authorize]
public sealed class SolicitudesController : ControllerBase
{
    private readonly SolicitudService _service;

    public SolicitudesController(SolicitudService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] SolicitudFilter filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(
            filter,
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CrearSolicitudRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CrearAsync(
            request,
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id,
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> ChangeState(
        int id,
        CambiarEstadoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CambiarEstadoAsync(
            id,
            request,
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPatch("{id:int}/asignacion")]
    public async Task<IActionResult> Assign(
        int id,
        AsignarResponsableRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.AsignarAsync(
            id,
            request,
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{id:int}/comentarios")]
    public async Task<IActionResult> AddComment(
        int id,
        CrearComentarioRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.AgregarComentarioAsync(
            id,
            request,
            cancellationToken);

        return result.ToActionResult();
    }
}
```

> Para mantener el Controller completamente limpio, la conversión
> `Result -> IActionResult` debe vivir en una extensión de API y no
> repetirse en cada método.

------------------------------------------------------------------------

# 24. Result -\> HTTP

``` csharp
using Microsoft.AspNetCore.Mvc;
using SB.Solicitudes.Application.Common.Models;

namespace SB.Solicitudes.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.Error!.Type switch
        {
            ErrorType.Validation =>
                new BadRequestObjectResult(result.Error),

            ErrorType.NotFound =>
                new NotFoundObjectResult(result.Error),

            ErrorType.Unauthorized =>
                new UnauthorizedObjectResult(result.Error),

            ErrorType.Forbidden =>
                new ObjectResult(result.Error)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                },

            ErrorType.Conflict =>
                new ConflictObjectResult(result.Error),

            _ =>
                new ObjectResult(result.Error)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
        };
    }

    public static IActionResult ToActionResult(
        this Result result)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return result.Error!.Type switch
        {
            ErrorType.Validation =>
                new BadRequestObjectResult(result.Error),

            ErrorType.NotFound =>
                new NotFoundObjectResult(result.Error),

            ErrorType.Unauthorized =>
                new UnauthorizedObjectResult(result.Error),

            ErrorType.Forbidden =>
                new ObjectResult(result.Error)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                },

            ErrorType.Conflict =>
                new ConflictObjectResult(result.Error),

            _ =>
                new ObjectResult(result.Error)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
        };
    }
}
```

------------------------------------------------------------------------

# 25. Dashboard

## DTO

``` csharp
public sealed record DashboardResumenDto(
    int SolicitudesAbiertas,
    int SolicitudesCerradas,
    int SolicitudesVencidas,
    IReadOnlyDictionary<string, int> PorEstado,
    IReadOnlyDictionary<string, int> PorPrioridad,
    IReadOnlyCollection<SolicitudResumenDto> Ultimas);
```

## Controller

``` csharp
[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Administrador,Analista")]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service)
    {
        _service = service;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> GetResumen(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetResumenAsync(
            cancellationToken);

        return result.ToActionResult();
    }
}
```

------------------------------------------------------------------------

# 26. Catálogos

## Controller

``` csharp
[ApiController]
[Route("api/catalogos")]
[Authorize]
public sealed class CatalogosController : ControllerBase
{
    private readonly CatalogoService _service;

    public CatalogosController(CatalogoService service)
    {
        _service = service;
    }

    [HttpGet("areas")]
    public async Task<IActionResult> GetAreas(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAreasActivasAsync(
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("tipos-solicitud")]
    public async Task<IActionResult> GetTipos(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetTiposActivosAsync(
            cancellationToken);

        return result.ToActionResult();
    }
}
```

------------------------------------------------------------------------

# 27. Autorización

Reglas:

  Rol             Solicitudes             Asignación   Internos   Dashboard
  --------------- ----------------------- ------------ ---------- -----------
  Administrador   Todas                   Sí           Sí         Sí
  Analista        Disponibles/asignadas   Sí           Sí         Sí
  Solicitante     Propias + creación      No           No         No

La seguridad debe validarse en backend, no confiar en React. La prueba
exige explícitamente autorización en backend.
fileciteturn0file0L139-L142

------------------------------------------------------------------------

# 28. Dependency Injection

## Application DependencyInjection

``` csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Solicitudes.Application.Services;

namespace SB.Solicitudes.Application;

public static class ApplicationServiceConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(ApplicationServiceConfiguration).Assembly);

        services.AddScoped<AuthService>();
        services.AddScoped<SolicitudService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<CatalogoService>();

        return services;
    }
}
```

## Infrastructure DependencyInjection

``` csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SB.Solicitudes.Application.Common.Interfaces;
using SB.Solicitudes.Infrastructure.CurrentUser;
using SB.Solicitudes.Infrastructure.Notifications;
using SB.Solicitudes.Infrastructure.Persistence;
using SB.Solicitudes.Infrastructure.Persistence.Repositories;
using SB.Solicitudes.Infrastructure.Security;

namespace SB.Solicitudes.Infrastructure;

public static class InfrastructureServiceConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DefaultConnection no está configurada.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(ApplicationDbContext).Assembly.FullName);
                });
        });

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.AddHttpContextAccessor();

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISolicitudRepository, SolicitudRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<ITipoSolicitudRepository, TipoSolicitudRepository>();
        services.AddScoped<IComentarioRepository, ComentarioRepository>();
        services.AddScoped<IHistorialEstadoRepository, HistorialEstadoRepository>();
        services.AddScoped<INotificacionRepository, NotificacionRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<INotificationService, DatabaseNotificationService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<PasswordHasher>();

        return services;
    }
}
```

------------------------------------------------------------------------

# 29. Program.cs

``` csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SB.Solicitudes.Application;
using SB.Solicitudes.Infrastructure;
using SB.Solicitudes.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

var jwtSettings =
    builder.Configuration
        .GetSection(JwtSettings.SectionName)
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "Configuración JWT no encontrada.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "SB Solicitudes API",
            Version = "v1",
            Description =
                "API para gestión interna de solicitudes."
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Ingrese únicamente el JWT. Swagger enviará automáticamente: Bearer {token}."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

## Flujo de Swagger

1.  Ejecutar API.
2.  Abrir `/swagger`.
3.  Ejecutar `POST /api/auth/login`.
4.  Copiar `token`.
5.  Presionar **Authorize**.
6.  Pegar el JWT.
7.  Swagger enviará:

``` http
Authorization: Bearer eyJhbGciOi...
```

8.  Ejecutar cualquier endpoint protegido.

El requisito de Swagger/OpenAPI habilitado para ejecución local está
incluido en la especificación técnica. fileciteturn0file0L147-L155

------------------------------------------------------------------------

# 30. appsettings.json

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SB_Solicitudes;Trusted_Connection=True;TrustServerCertificate=True;"
  },

  "Jwt": {
    "Key": "CHANGE_THIS_USING_USER_SECRETS_OR_ENVIRONMENT",
    "Issuer": "SB.Solicitudes.Api",
    "Audience": "SB.Solicitudes.Client",
    "ExpirationMinutes": 60
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

No subir secretos reales al repositorio.

------------------------------------------------------------------------

# 31. Migraciones

Desde la raíz de la solución:

``` bash
dotnet ef migrations add InitialCreate \
  --project src/backend/SB.Solicitudes.Infrastructure \
  --startup-project src/backend/SB.Solicitudes.Api \
  --output-dir Persistence/Migrations
```

Aplicar:

``` bash
dotnet ef database update \
  --project src/backend/SB.Solicitudes.Infrastructure \
  --startup-project src/backend/SB.Solicitudes.Api
```

La prueba acepta migraciones EF Core o script SQL de creación/carga.
fileciteturn0file0L276-L280

------------------------------------------------------------------------

# 32. Seed

Los datos iniciales deben incluir como mínimo:

``` text
Roles:
- Administrador
- Analista
- Solicitante

Áreas:
- Tecnología
- Canales
- Operaciones
- Seguridad

Tipos:
- Incidente
- Requerimiento
- Acceso
- Soporte
```

Usuarios de prueba:

``` text
admin@demo.local
analista@demo.local
solicitante@demo.local
```

Las contraseñas reales de prueba deben documentarse de forma segura y
únicamente para entorno local.

La entrega solicita credenciales de prueba no sensibles o usuarios
semilla documentados. fileciteturn0file0L267-L275

------------------------------------------------------------------------

# 33. Validaciones

Las validaciones deben ejecutarse en backend aunque React también
valide.

Ejemplo con FluentValidation:

``` csharp
using FluentValidation;
using SB.Solicitudes.Application.DTOs.Solicitudes;

public sealed class CrearSolicitudValidator
    : AbstractValidator<CrearSolicitudRequest>
{
    public CrearSolicitudValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Descripcion)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Prioridad)
            .NotEmpty()
            .Must(x =>
                x.Equals("Baja", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("Media", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("Alta", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("Critica", StringComparison.OrdinalIgnoreCase));

        RuleFor(x => x.FechaCompromiso)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.EvidenciaUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.EvidenciaUrl));
    }
}
```

La prueba exige validaciones de campos obligatorios y longitud máxima.
fileciteturn0file0L77-L87

------------------------------------------------------------------------

# 34. Manejo global de excepciones

Los errores esperados de negocio deben usar `Result`.

Las excepciones inesperadas deben pasar por middleware global.

``` csharp
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error no controlado.");

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    code = "INTERNAL_ERROR",
                    message = "Ocurrió un error interno."
                });
        }
    }
}
```

Registrar:

``` csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

La prueba solicita manejo consistente de errores y logging de eventos
principales. fileciteturn0file0L147-L158

------------------------------------------------------------------------

# 35. Logging

No registrar:

``` text
Contraseñas
JWT completos
Connection strings
Información sensible innecesaria
```

Sí registrar:

``` text
RequestId
UsuarioId
Endpoint
SolicitudId
Código de solicitud
Cambio de estado
Asignación
Errores técnicos
Duración de operaciones importantes
```

Ejemplo:

``` csharp
_logger.LogInformation(
    "Solicitud {SolicitudId} asignada al usuario {UsuarioId}",
    solicitud.Id,
    responsable.Id);
```

------------------------------------------------------------------------

# 36. Flujo completo de creación

``` text
POST /api/solicitudes
        │
        ▼
SolicitudesController
        │
        ▼
SolicitudService
        │
        ├── CurrentUser
        ├── Validaciones
        ├── AreaRepository
        ├── TipoSolicitudRepository
        ├── Generación código
        ├── Solicitud Entity
        ├── SolicitudRepository
        ├── UnitOfWork
        │
        ▼
Database
        │
        ▼
NotificationService
        │
        ▼
NotificacionRepository
```

El Controller no conoce SQL Server ni reglas de negocio.

------------------------------------------------------------------------

# 37. Flujo de cambio de estado

``` text
PATCH /api/solicitudes/{id}/estado
             │
             ▼
      SolicitudService
             │
             ├── Autenticación
             ├── Autorización
             ├── Solicitud existe
             ├── Estado válido
             ├── Comentario obligatorio
             ├── Validación de cierre
             ├── Regla de reapertura
             │
             ├── Solicitud.CambiarEstado()
             │
             ├── HistorialEstado
             │
             ├── UnitOfWork
             │
             └── NotificationService
```

Cada transición debe guardar:

``` text
SolicitudId
EstadoAnterior
EstadoNuevo
UsuarioId
Fecha
Comentario
```

Esto satisface el requisito de trazabilidad de las transiciones.
fileciteturn0file0L83-L87

------------------------------------------------------------------------

# 38. Paginación eficiente

Nunca hacer:

``` csharp
var all = await db.Solicitudes.ToListAsync();

var page = all
    .Skip(...)
    .Take(...);
```

Debe hacerse:

``` csharp
var query = db.Solicitudes
    .AsNoTracking();

query = query
    .Where(...);

var total = await query.CountAsync();

var items = await query
    .OrderByDescending(x => x.FechaCreacion)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

Esto permite que SQL Server procese la paginación.

La prueba exige listados paginados y filtros aplicados desde backend.
fileciteturn0file0L123-L132

------------------------------------------------------------------------

# 39. Índices

Mantener índices sobre:

``` text
Solicitudes.Codigo
Solicitudes.Estado
Solicitudes.Prioridad
Solicitudes.UsuarioSolicitanteId
Solicitudes.ResponsableId
Solicitudes.AreaId
Solicitudes.TipoSolicitudId
Solicitudes.FechaCreacion
Solicitudes.FechaCompromiso

Usuarios.Correo
Areas.Nombre
TiposSolicitud.Nombre
Comentarios.SolicitudId
HistorialEstados.SolicitudId
Notificaciones.SolicitudId
```

Los índices deben responder a los filtros y consultas reales, no
agregarse indiscriminadamente.

------------------------------------------------------------------------

# 40. Seguridad de acceso a solicitudes

El backend nunca debe confiar en:

``` http
GET /api/solicitudes/123
```

solamente porque el usuario esté autenticado.

Debe comprobar:

``` text
Administrador
    -> cualquier solicitud

Analista
    -> solicitud asignada
    -> solicitud disponible para gestión

Solicitante
    -> únicamente solicitudes propias
```

La prueba exige evitar exposición de información a usuarios sin permisos
y plantea esta cuestión explícitamente entre sus preguntas de
conceptualización. fileciteturn0file0L244-L257

------------------------------------------------------------------------

# 41. Reapertura

Regla:

``` text
Solicitud Cerrada
       │
       ├── Administrador → puede reabrir
       │
       ├── Analista      → puede reabrir
       │
       └── Solicitante   → NO puede reabrir
```

Estados permitidos:

``` text
Registrada
En análisis
En progreso
En espera del solicitante
Resuelta
Cerrada
```

Los estados mínimos provienen directamente de la prueba.
fileciteturn0file0L83-L87

------------------------------------------------------------------------

# 42. Cierre con resolución

No permitir:

``` text
PATCH estado = Cerrada
comentario = vacío
```

Debe existir comentario de resolución.

Una implementación más limpia que buscar el texto `"Resolución"`
consiste en extender `Comentario` con un campo semántico como:

``` csharp
public string Tipo { get; private set; }
```

con valores:

``` text
Normal
Resolucion
```

Esto es preferible a depender del contenido libre del comentario.

------------------------------------------------------------------------

# 43. Mejoras recomendadas sobre el modelo inicial

Para dejar el backend realmente empresarial, recomiendo estos cambios
antes de cerrar la implementación:

## 43.1 Estados y prioridades

En lugar de guardar strings arbitrarios:

``` text
"En progreso"
"en progreso"
"EN PROGRESO"
```

usar enums internamente o Value Objects.

## 43.2 Código de solicitud

No confiar únicamente en:

``` csharp
ultimo + 1
```

porque dos requests simultáneos pueden calcular el mismo número.

Para una implementación robusta:

``` text
SQL Sequence
+
formato SOL-{YEAR}-{NUMBER:D4}
```

o una tabla de secuencias con transacción.

## 43.3 Fecha

Persistir UTC:

``` csharp
DateTime.UtcNow
```

y convertir a horario local únicamente en presentación.

------------------------------------------------------------------------

# 44. Tests

La prueba valora pruebas unitarias o de integración sobre componentes
críticos. fileciteturn0file0L44-L50

Prioridad:

### Unitarios

``` text
AuthService
SolicitudService
Reglas de cambio de estado
Reglas de autorización
Generación de código
Result Pattern
Validadores
```

### Integración

``` text
POST /api/auth/login
POST /api/solicitudes
GET /api/solicitudes
GET /api/solicitudes/{id}
PATCH /api/solicitudes/{id}/estado
PATCH /api/solicitudes/{id}/asignacion
POST /api/solicitudes/{id}/comentarios
GET /api/dashboard/resumen
```

------------------------------------------------------------------------

# 45. Docker

Como extra valorado por la prueba:

``` text
API
SQL Server
```

pueden ejecutarse mediante Docker Compose.

La prueba menciona explícitamente contenedores Docker como extra
valorado. fileciteturn0file0L237-L243

Ejemplo conceptual:

``` yaml
services:

  api:
    build:
      context: .
      dockerfile: src/backend/SB.Solicitudes.Api/Dockerfile
    ports:
      - "8080:8080"
    depends_on:
      - sqlserver

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "ChangeThisPassword123!"
    ports:
      - "1433:1433"
```

Nunca usar contraseñas reales en un archivo versionado.

------------------------------------------------------------------------

# 46. CI básico

Otro extra valorado:

``` yaml
name: Backend CI

on:
  push:
    branches:
      - main
      - develop

  pull_request:

jobs:
  build-test:

    runs-on: ubuntu-latest

    steps:

      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.x"

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release
```

------------------------------------------------------------------------

# 47. README final del repositorio

El README debe incluir:

``` text
1. Descripción
2. Objetivo
3. Arquitectura
4. Estructura del proyecto
5. Requisitos
6. Configuración
7. Connection String
8. JWT
9. Migraciones
10. Seed
11. Ejecución
12. Swagger
13. Usuarios de prueba
14. Endpoints
15. Reglas de autorización
16. Paginación
17. Notificaciones
18. Logging
19. Tests
20. Docker
21. CI
22. Decisiones técnicas
23. Trade-offs
```

La documentación de decisiones técnicas es parte explícita del
ejercicio. fileciteturn0file0L58-L63

------------------------------------------------------------------------

# 48. Decisiones técnicas

## ¿Por qué Clean Architecture?

Porque permite separar:

``` text
Dominio
Aplicación
Infraestructura
API
```

y evita que las reglas de negocio dependan de frameworks o bases de
datos.

## ¿Por qué Repository?

Porque el Application necesita abstraerse del mecanismo de persistencia.

## ¿Por qué GenericRepository?

Para evitar duplicar:

``` text
GetById
GetAll
Add
Update
Delete
Exists
Pagination
```

en cada entidad.

## ¿Por qué repositorios específicos?

Porque las consultas de negocio no deben forzarse dentro del
GenericRepository.

Ejemplo:

``` text
GetByCorreo
GetDetalleSolicitud
GetActivas
GetBySolicitudId
CountVencidas
```

pertenecen a contratos específicos.

## ¿Por qué UnitOfWork?

Permite coordinar varias operaciones relacionadas y hacer un único:

``` csharp
SaveChangesAsync()
```

Esto es especialmente útil al cambiar estado:

``` text
Solicitud
+
Historial
+
Notificación
```

## ¿Por qué Result?

Permite representar explícitamente:

``` text
Success
Validation
NotFound
Unauthorized
Forbidden
Conflict
Failure
```

sin llenar el código de excepciones para errores esperados.

## ¿Por qué INotificationService?

Para cumplir el requisito de extensibilidad:

``` text
Database
→ Email
→ Queue
→ RabbitMQ
```

sin modificar el servicio principal.

------------------------------------------------------------------------

# 49. Principios SOLID aplicados

## S --- Single Responsibility

``` text
Controller
    HTTP

Service
    negocio

Repository
    persistencia

Entity
    comportamiento del dominio

NotificationService
    notificaciones

JwtTokenGenerator
    JWT
```

## O --- Open/Closed

Agregar:

``` text
RabbitMqNotificationService
```

no requiere modificar:

``` text
SolicitudService
```

## L --- Liskov

Los repositorios específicos implementan contratos que respetan el
comportamiento del repositorio base.

## I --- Interface Segregation

No crear una interfaz gigante:

``` text
IEverythingRepository
```

Se utilizan contratos pequeños y específicos.

## D --- Dependency Inversion

Application depende de:

``` text
IUnitOfWork
INotificationService
IJwtTokenGenerator
ICurrentUserService
```

y no de:

``` text
ApplicationDbContext
HttpContext
JwtSecurityToken
SQL Server
```

------------------------------------------------------------------------

# 50. Qué NO hacer

No colocar esto en Controllers:

``` csharp
if (role == "Admin")
if (state == "Closed")
db.SaveChanges()
new JwtSecurityToken()
```

No colocar esto en Domain:

``` csharp
DbContext
HttpContext
ILogger
IConfiguration
```

No colocar SQL directamente en Application.

No devolver entidades EF directamente como contrato público.

No aceptar `UsuarioSolicitanteId` desde el request si se puede obtener
del JWT.

No confiar en autorización únicamente en React.

No traer toda la tabla para paginar en memoria.

No guardar JWT o passwords en logs.

No hardcodear secretos.

------------------------------------------------------------------------

# 51. Checklist de cumplimiento

  Requisito                     Estado
  ----------------------------- ----------------
  .NET 8                        ✅
  ASP.NET Core Web API          ✅
  EF Core                       ✅
  SQL Server                    ✅
  Clean Architecture            ✅
  Onion Architecture            ✅
  SOLID                         ✅
  Repository                    ✅
  Generic Repository            ✅
  Unit of Work                  ✅
  Result Pattern                ✅
  Paginación                    ✅
  Filtros backend               ✅
  JWT                           ✅
  Roles                         ✅
  Swagger Authorize             ✅
  Historial de estados          ✅
  Comentarios                   ✅
  Asignación                    ✅
  Notificaciones desacopladas   ✅
  Dashboard                     ✅
  Migraciones                   ✅
  Seed                          ✅
  Logging                       ✅
  Validaciones backend          ✅
  Manejo global de errores      ✅
  Docker                        ⭐ Extra
  CI                            ⭐ Extra
  Tests integración             ⭐ Extra
  Specification                 ⭐ Recomendado
  Eventos de dominio            ⭐ Recomendado

Los extras de Repository/Unit of Work/Result, mecanismo desacoplado de
notificaciones, Docker, pruebas de integración y CI están expresamente
contemplados por la prueba. fileciteturn0file0L237-L243

------------------------------------------------------------------------

# 52. Endpoints finales

``` text
POST   /api/auth/login

GET    /api/solicitudes
POST   /api/solicitudes
GET    /api/solicitudes/{id}
PATCH  /api/solicitudes/{id}/estado
PATCH  /api/solicitudes/{id}/asignacion
POST   /api/solicitudes/{id}/comentarios

GET    /api/dashboard/resumen

GET    /api/catalogos/areas
GET    /api/catalogos/tipos-solicitud
```

Estos son los únicos endpoints funcionales de negocio definidos para
esta etapa. La prueba oficial lista exactamente estas rutas.
fileciteturn0file0L202-L221

------------------------------------------------------------------------

# 53. Resultado esperado

La solución final debe permitir:

``` text
Login
  ↓
JWT
  ↓
Swagger Authorize
  ↓
Crear solicitud
  ↓
Código SOL-2026-0001
  ↓
Solicitud Registrada
  ↓
Asignar Analista
  ↓
Notificación
  ↓
En análisis
  ↓
En progreso
  ↓
En espera del solicitante
  ↓
Resuelta + comentario de resolución
  ↓
Cerrada
  ↓
Historial completo
  ↓
Dashboard actualizado
```

El backend queda preparado para que el frontend React + TypeScript
consuma estos contratos posteriormente.

------------------------------------------------------------------------

# 54. Nota importante de implementación

Este documento define la arquitectura y el código base que debe formar
el backend. Antes de compilar, hay que ajustar únicamente los nombres a
las entidades reales existentes en el proyecto, especialmente las
navegaciones de `Solicitud` (`Comentarios`, `HistorialEstados`) y
cualquier propiedad que ya haya sido definida previamente.

No se debe duplicar una entidad, configuración, interfaz o extensión que
ya exista. La regla debe ser:

``` text
Una responsabilidad
        ↓
Un lugar
        ↓
Una implementación
```

Eso evita que el proyecto termine con múltiples versiones de la misma
lógica.

------------------------------------------------------------------------

# 55. Criterio final de calidad

La implementación debe poder responder afirmativamente a estas
preguntas:

``` text
¿Puede cambiar SQL Server por otra infraestructura sin tocar Domain?
¿Puede cambiar JWT por otro mecanismo sin tocar las entidades?
¿Puede cambiar DatabaseNotification por RabbitMQ sin tocar SolicitudService?
¿Puede probarse SolicitudService sin levantar HTTP?
¿Puede probarse la lógica de estados sin SQL Server?
¿La paginación ocurre en SQL?
¿La autorización se valida en backend?
¿Los Controllers tienen cero reglas de negocio?
¿Los secretos están fuera del código?
¿Cada transición queda auditada?
¿La solución puede extenderse sin modificar bloques centrales?
```

Si alguna respuesta es "no", debe revisarse antes de considerar
terminado el backend.
