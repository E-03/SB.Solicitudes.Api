using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Auth;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using Xunit;

namespace SB.Solicitudes.IntegrationTests.Integration
{
    public class SolicitudesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public SolicitudesEndpointsTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private async Task<HttpClient> CreateAuthenticatedClientAsync(string correo)
        {
            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequest(correo, CustomWebApplicationFactory.Password));

            var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", login!.Token);

            return client;
        }

        [Fact]
        public async Task Get_SinToken_Devuelve401()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/solicitudes");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CrearSolicitud_ComoSolicitante_GeneraCodigoYPermiteConsultarDetalle()
        {
            var client = await CreateAuthenticatedClientAsync(
                CustomWebApplicationFactory.SolicitanteEmail);

            var crear = new CrearSolicitudRequest(
                "No enciende el equipo",
                "El equipo de la estación 4 no enciende.",
                "Alta",
                AreaId: 1,
                TipoSolicitudId: 1,
                DateTime.UtcNow.AddDays(2),
                null,
                "Referencia del ticket físico");

            var createResponse = await client.PostAsJsonAsync("/api/solicitudes", crear);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var creada = await createResponse.Content.ReadFromJsonAsync<SolicitudResponse>();

            Assert.NotNull(creada);
            Assert.StartsWith("SOL-", creada!.Codigo);
            Assert.Equal("Registrada", creada.Estado);

            var detailResponse = await client.GetAsync($"/api/solicitudes/{creada.Id}");

            Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);

            var detalle = await detailResponse.Content.ReadFromJsonAsync<SolicitudDetailResponse>();

            Assert.NotNull(detalle);
            Assert.Equal(creada.Codigo, detalle!.Codigo);
            Assert.Empty(detalle.Historial);
        }

        [Fact]
        public async Task Get_Paginado_DevuelveEstructuraPaginada()
        {
            var client = await CreateAuthenticatedClientAsync(
                CustomWebApplicationFactory.AdminEmail);

            var response = await client.GetAsync("/api/solicitudes?PageNumber=1&PageSize=5");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var paged = await response.Content
                .ReadFromJsonAsync<PaginatedResult<SolicitudListItem>>();

            Assert.NotNull(paged);
            Assert.Equal(1, paged!.PageNumber);
            Assert.Equal(5, paged.PageSize);
        }

        [Fact]
        public async Task Dashboard_ComoSolicitante_Devuelve403()
        {
            var client = await CreateAuthenticatedClientAsync(
                CustomWebApplicationFactory.SolicitanteEmail);

            var response = await client.GetAsync("/api/dashboard/resumen");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CambiarEstado_ACerradaSinComentarioDeResolucion_Devuelve400()
        {
            var solicitanteClient = await CreateAuthenticatedClientAsync(
                CustomWebApplicationFactory.SolicitanteEmail);

            var crear = new CrearSolicitudRequest(
                "Otro incidente",
                "Descripción del incidente.",
                "Media",
                AreaId: 1,
                TipoSolicitudId: 1,
                DateTime.UtcNow.AddDays(2),
                null,
                null);

            var createResponse = await solicitanteClient.PostAsJsonAsync("/api/solicitudes", crear);
            var creada = await createResponse.Content.ReadFromJsonAsync<SolicitudResponse>();

            var adminClient = await CreateAuthenticatedClientAsync(
                CustomWebApplicationFactory.AdminEmail);

            var cambiarEstado = new CambiarEstadoRequest("Cerrada", "Se cierra sin resolución formal");

            var response = await adminClient.PatchAsJsonAsync(
                $"/api/solicitudes/{creada!.Id}/estado",
                cambiarEstado);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
