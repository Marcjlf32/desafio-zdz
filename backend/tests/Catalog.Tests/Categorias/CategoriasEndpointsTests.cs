using System.Net;
using System.Net.Http.Json;
using Catalog.Application.Categorias;
using Catalog.Application.Produtos;
using FluentAssertions;
using Xunit;

namespace Catalog.Tests.Categorias;

public class CategoriasEndpointsTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;

    public CategoriasEndpointsTests(CatalogApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "AC03 - GET /api/categorias retorna 200 com lista")]
    public async Task AC03_GetCategorias_RetornaListaComOk()
    {
        var response = await _client.GetAsync("/api/categorias");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categorias = await response.Content.ReadFromJsonAsync<List<CategoriaDto>>();
        categorias.Should().NotBeNull();
    }

    [Fact(DisplayName = "AC03 - POST /api/categorias cria e retorna 201")]
    public async Task AC03_PostCategoria_CriaERetorna201()
    {
        var dto = new CreateCategoriaDto("Eletronicos", "Produtos eletronicos");

        var response = await _client.PostAsJsonAsync("/api/categorias", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var criada = await response.Content.ReadFromJsonAsync<CategoriaDto>();
        criada.Should().NotBeNull();
        criada!.Nome.Should().Be("Eletronicos");
        criada.Id.Should().BeGreaterThan(0);
    }

    [Fact(DisplayName = "AC03 - PUT /api/categorias/{id} atualiza e retorna 204")]
    public async Task AC03_PutCategoria_AtualizaERetorna204()
    {
        var criada = await CreateCategoria("Categoria Original", null);

        var update = new UpdateCategoriaDto("Categoria Atualizada", "Nova descricao");
        var response = await _client.PutAsJsonAsync($"/api/categorias/{criada.Id}", update);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "AC03 - DELETE /api/categorias/{id} retorna 204 quando sem produtos")]
    public async Task AC03_DeleteCategoria_RetornaNoContent()
    {
        var criada = await CreateCategoria("Categoria para Excluir", null);

        var response = await _client.DeleteAsync($"/api/categorias/{criada.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "AC04 - DELETE categoria com produtos retorna 409 com mensagem")]
    public async Task AC04_DeleteCategoriaComProdutos_DeveRetornar409ComMensagem()
    {
        var categoria = await CreateCategoria("Categoria com Produto", null);
        await CreateProduto("Produto Teste", 10.50m, categoria.Id);

        var response = await _client.DeleteAsync($"/api/categorias/{categoria.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();
        problem.Should().NotBeNull();
        problem!.Detail.Should().Be("Não é possível excluir uma categoria que possua produtos vinculados.");
    }

    [Fact(DisplayName = "AC05 - POST categoria com nome curto retorna 400 com sumario")]
    public async Task AC05_PostCategoriaNomeCurto_DeveRetornar400ComSumario()
    {
        var dto = new CreateCategoriaDto("abc", null);

        var response = await _client.PostAsJsonAsync("/api/categorias", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey("Nome");
        problem.Errors!["Nome"].Should().Contain(m => m.Contains("5 caracteres"));
        problem.Errors["Nome"].Should().Contain(m => m.Contains("mínimo"));
    }

    private async Task<CategoriaDto> CreateCategoria(string nome, string? descricao)
    {
        var response = await _client.PostAsJsonAsync("/api/categorias", new CreateCategoriaDto(nome, descricao));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoriaDto>())!;
    }

    private async Task<ProdutoDto> CreateProduto(string nome, decimal preco, int categoriaId)
    {
        var response = await _client.PostAsJsonAsync("/api/produtos", new CreateProdutoDto(nome, null, preco, categoriaId));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProdutoDto>())!;
    }
}
