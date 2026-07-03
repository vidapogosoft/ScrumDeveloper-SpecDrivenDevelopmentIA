using System.Net;
using System.Net.Http.Headers;

namespace Guia.API.Tests.Integration;

public class StartupBehaviorTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public StartupBehaviorTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Swagger_EnDevelopment_EstaDisponible()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Cors_Preflight_DevuelveHeaders()
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/astro/detalle/signo/Aries");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await client.SendAsync(request);

        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task SeedingInicial_PermiteConsultarTemas()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/numerologia/temas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.NotEqual("[]", payload.Trim());
    }
}
