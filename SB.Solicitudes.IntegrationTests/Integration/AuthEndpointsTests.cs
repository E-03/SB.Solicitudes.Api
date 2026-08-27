using System.Net;
using System.Net.Http.Json;
using SB.Solicitudes.Application.DTOs.Auth;
using Xunit;

namespace SB.Solicitudes.IntegrationTests.Integration
{
    public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthEndpointsTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_CredencialesValidas_DevuelveTokenYDatosDeUsuario()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequest(CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.Password));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.NotNull(body);
            Assert.False(string.IsNullOrWhiteSpace(body!.Token));
            Assert.Equal("Administrador", body.Rol);
        }

        [Fact]
        public async Task Login_CredencialesInvalidas_Devuelve401()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequest(CustomWebApplicationFactory.AdminEmail, "ClaveIncorrecta"));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_CorreoVacio_Devuelve400()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequest("", "algo"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
