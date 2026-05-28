using System.Net;
using FluentAssertions;
using Xunit;

namespace Catalog.Tests.Api;

public class CorsTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;

    public CorsTests(CatalogApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "AC06 - CORS aceita origem autorizada (localhost:3000)")]
    public async Task AC06_Cors_AceitaOrigemAutorizada()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/categorias");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await _client.SendAsync(request);

        response.Headers.Should().Contain(h => h.Key == "Access-Control-Allow-Origin");
        response.Headers.GetValues("Access-Control-Allow-Origin").Should().Contain("http://localhost:3000");
    }

    [Fact(DisplayName = "AC06 - CORS rejeita origem nao autorizada")]
    public async Task AC06_CorsRejeitaOrigemNaoAutorizada()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/categorias");
        request.Headers.Add("Origin", "http://evil.example.com");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await _client.SendAsync(request);

        response.Headers.Should().NotContain(h => h.Key == "Access-Control-Allow-Origin");
    }
}
