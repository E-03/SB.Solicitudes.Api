SB Solicitudes — Backend
API REST para registrar, consultar y gestionar solicitudes internas de servicios tecnológicos, construida en .NET 8 / ASP.NET Core Web API siguiendo Clean Architecture + Onion Architecture.

Este repositorio cubre únicamente el backend. El frontend (React + TypeScript) se construirá posteriormente sobre estos contratos.

1. Qué hace el sistema
Permite que los usuarios (Solicitante, Analista, Administrador) registren solicitudes de servicio, les den seguimiento con historial de estados y comentarios, las asignen a un analista, y consulten un dashboard con métricas agregadas — todo protegido con autenticación JWT y autorización basada en roles validada en backend.

2. Arquitectura
Domain          → Entidades, enums y reglas de negocio. No depende de nada.
Application     → Casos de uso (Services), DTOs, interfaces (repos, JWT,
                   notificaciones, usuario actual), Result Pattern,
                   paginación y validadores (FluentValidation).
                   Depende únicamente de Domain.
Infrastructure  → EF Core (DbContext, configuraciones, migraciones),
                   repositorios concretos, JWT, hashing de contraseñas,
                   notificaciones, seed. Implementa las interfaces de
                   Application.
Api             → Controllers, Program.cs, middleware de excepciones,
                   configuración de Swagger/JWT. No contiene lógica de
                   negocio.
Regla de dependencias respetada:

Api → Application → Domain
Infrastructure → Application → Domain
Domain no referencia EF Core, ASP.NET Core, JWT ni SQL Server. Application no referencia Infrastructure ni ningún tipo concreto de persistencia — solo interfaces.

Estructura de proyectos
SB.Solicitudes.Api.sln
├── SB.Solicitudes.Api             (Web API)
├── SB.Solicitudes.Application     (casos de uso / puerto de entrada de la lógica)
├── SB.Solicitudes.Domain          (entidades y reglas de dominio)
├── SB.Solicitudes.Infrastructure  (EF Core, seguridad, notificaciones)
└── SB.Solicitudes.IntegrationTests
3. Patrones y decisiones técnicas
Repository + Generic Repository: IGenericRepository<T> centraliza GetById/GetAll/GetPaged/Add/Update/Remove/Exists. Los repositorios específicos (ISolicitudRepository, etc.) solo agregan métodos que realmente necesitan lógica de consulta propia (evita duplicar CRUD).
Unit of Work (IUnitOfWork): agrupa todos los repositorios y expone un único SaveChangesAsync, evitando SaveChanges() dispersos. Es clave para operaciones compuestas como "cambiar estado + registrar historial + notificar".
Result Pattern (Result / ResultEntity<T> / ResultError / ErrorType): los Services devuelven resultados explícitos (Validation, NotFound, Unauthorized, Forbidden, Conflict, Failure) en lugar de lanzar excepciones para errores de negocio esperados. Un ResultExtensions.ToActionResult() en la capa Api traduce el resultado a la respuesta HTTP correspondiente, así los Controllers no repiten ese mapeo.
Paginación eficiente: ISolicitudRepository.GetPagedAsync construye el IQueryable con todos los filtros antes de Count/Skip/Take, de modo que SQL Server resuelve el filtrado y la paginación — nunca se trae la tabla completa a memoria.
Notificaciones desacopladas (INotificationService / INotificacionService): SolicitudService solo conoce la abstracción. La implementación (NotificacionService) delega en una lista de INotificacionHandler (consola + base de datos), lo que permite agregar EmailNotificacionHandler o RabbitMqNotificacionHandler sin tocar la lógica de solicitudes.
Entidades ricas: constructores que garantizan el estado inicial válido, setters privados y métodos de comportamiento (Solicitud.CambiarEstado, Solicitud.AsignarResponsable) en lugar de DTOs anémicos con setters públicos.
Enums en vez de strings sueltos: Estado, Prioridad, Visibilidad, Canal se modelan como enums en el dominio y se persisten como nvarchar vía HasConversion<string>() — se obtiene seguridad de tipos en C# y legibilidad en la base de datos.
ICurrentUserService: abstrae HttpContext/claims para que Application nunca dependa de ASP.NET Core directamente.
4. Autenticación y autorización
Login (POST /api/auth/login) valida credenciales, verifica que el usuario esté activo, compara el hash de contraseña (Microsoft.AspNetCore.Identity.PasswordHasher) y genera un JWT (IJwtService / JwtService) con claims NameIdentifier, Name, Email y Role.

Los endpoints protegidos usan [Authorize] y, cuando corresponde, [Authorize(Roles = "...")] (por ejemplo, el dashboard solo es accesible para Administrador y Analista).

La visibilidad de una solicitud individual y el filtrado de listados se resuelven en el repositorio (consulta SQL) según el rol:

Rol	Solicitudes visibles	Asignar	Comentario interno	Dashboard
Administrador	Todas	Sí	Sí	Sí
Analista	Disponibles (sin responsable) o asignadas a él	Sí	Sí	Sí
Solicitante	Únicamente las propias	No	No	No
Esto se valida siempre en el servidor — nunca se confía en el frontend.

5. Reglas de negocio de Solicitudes
El código se genera automáticamente como SOL-{año}-{consecutivo:D4} a partir del último código registrado. (Nota de concurrencia: bajo carga concurrente extrema esta estrategia podría colisionar; para producción se recomienda una secuencia SQL o un método transaccional dedicado — documentado también en el propio Markdown de especificación).
Cada cambio de estado exige un comentario y queda registrado en HistorialEstado (estado anterior, estado nuevo, usuario, fecha, comentario).
No se puede cerrar una solicitud sin que exista un comentario de resolución (Visibilidad = Interno con el texto "Resolución").
Una solicitud cerrada solo puede reabrirse por Administrador o Analista.
Asignar/reasignar exige que el responsable sea un usuario con rol Analista y activo; enviar responsableId: null desasigna.
Cada creación, asignación, cambio de estado y cierre genera una notificación desacoplada de la lógica principal.
6. Endpoints
POST   /api/auth/login

GET    /api/solicitudes                       (paginado + filtros)
POST   /api/solicitudes
GET    /api/solicitudes/{id}
PATCH  /api/solicitudes/{id}/estado
PATCH  /api/solicitudes/{id}/asignacion
POST   /api/solicitudes/{id}/comentarios

GET    /api/dashboard/resumen                  (Administrador, Analista)

GET    /api/catalogos/areas
GET    /api/catalogos/tipos-solicitud
Filtros de GET /api/solicitudes: Estado, Prioridad, AreaId, UsuarioSolicitanteId, ResponsableId, FechaDesde, FechaHasta, PageNumber, PageSize.

7. Configuración
appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SB_Solicitudes;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "SB.Solicitudes.Api",
    "Audience": "SB.Solicitudes.Client",
    "ExpirationMinutes": 60
  }
}
appsettings.Development.json trae una clave JWT únicamente para desarrollo local. Nunca uses esa clave en producción — usa User Secrets, variables de entorno o un vault.

8. Migraciones
dotnet ef migrations add NombreDeLaMigracion \
  --project SB.Solicitudes.Infrastructure \
  --startup-project SB.Solicitudes.Api \
  --output-dir Persistence/Migrations

dotnet ef database update \
  --project SB.Solicitudes.Infrastructure \
  --startup-project SB.Solicitudes.Api
La migración inicial (InitialCreate) ya está incluida en el repositorio.

9. Seed de datos
Al ejecutar la API en entorno Development, DbInitializer.SeedAsync aplica las migraciones pendientes y, si las tablas están vacías, crea:

Áreas: Tecnología, Canales, Operaciones, Seguridad.

Tipos de solicitud: Incidente, Requerimiento, Acceso, Soporte.

Usuarios de prueba (contraseñas hasheadas, nunca en texto plano):

Correo	Contraseña	Rol
admin@demo.local	Admin123!	Administrador
analista@demo.local	Analista123!	Analista
solicitante@demo.local	Solicitante123!	Solicitante
10. Cómo ejecutar el proyecto
dotnet restore
dotnet build
dotnet run --project SB.Solicitudes.Api
Con ASPNETCORE_ENVIRONMENT=Development (perfil por defecto), la API aplica migraciones y siembra datos automáticamente al iniciar.

11. Swagger
Ejecuta la API y abre /swagger.
Ejecuta POST /api/auth/login con uno de los usuarios de prueba.
Copia el token de la respuesta.
Haz clic en Authorize e introduce el JWT (Swagger antepone Bearer automáticamente).
Ya puedes ejecutar cualquier endpoint protegido.
12. Tests
El proyecto SB.Solicitudes.IntegrationTests contiene:

Unit/ — AuthService y SolicitudService con IUnitOfWork y ICurrentUserService simulados (Moq): credenciales inválidas, cambio de estado sin comentario, cierre sin comentario de resolución, reapertura (permitida a Analista, denegada a Solicitante), asignación (rol inválido, responsable inválido), comentarios internos.
Integration/ — WebApplicationFactory<Program> con EF Core InMemory: login, autorización (401/403), creación de solicitud + consulta de detalle, paginación, cierre sin resolución.
dotnet test SB.Solicitudes.IntegrationTests
13. Logging
Se registran eventos relevantes (creación, cambio de estado, asignación, errores no controlados vía el middleware global) sin exponer contraseñas, JWT completos ni cadenas de conexión.

14. Manejo de errores
ExceptionHandlingMiddleware captura cualquier excepción no controlada, la registra y devuelve una respuesta JSON consistente (500). Los errores de negocio esperados (validación, no encontrado, no autorizado, prohibido, conflicto) se representan siempre con el Result Pattern y se traducen a sus códigos HTTP correspondientes (400/404/401/403/409).

15. Trade-offs y extensiones no implementadas
Generación de código de solicitud: por simplicidad se usa "último código + 1"; en un entorno de alta concurrencia real se recomienda una secuencia SQL dedicada (documentado también como nota en el código).
Docker / CI: no incluidos en esta entrega — son extras señalados como opcionales en la especificación. La arquitecta actual (DbContext parametrizado por IConfiguration, sin secretos hardcodeados) no requiere cambios para contenedores.
Specification pattern / eventos de dominio: no implementados; se consideraron innecesarios para el alcance actual y se optó por mantener los repositorios específicos simples, evitando abstracciones adicionales sin un caso de uso concreto que las justifique.