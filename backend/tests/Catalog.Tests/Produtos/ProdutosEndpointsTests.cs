using System.Net;
using System.Net.Http.Json;
using Catalog.Application.Categorias;
using Catalog.Application.Produtos;
using FluentAssertions;
using Xunit;

namespace Catalog.Tests.Produtos;

public class ProdutosEndpointsTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;

    public ProdutosEndpointsTests(CatalogApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "AC03 - GET /api/produtos retorna 200 com categoria aninhada")]
    public async Task AC03_GetProdutos_RetornaListaComCategoriaInclude()
    {
        var categoria = await CreateCategoria("Categoria Teste", null);
        await CreateProduto("Produto Teste", 99.90m, categoria.Id);

        var response = await _client.GetAsync("/api/produtos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoDto>>();
        produtos.Should().NotBeNull().And.NotBeEmpty();
        produtos!.First().Categoria.Should().NotBeNull();
        produtos.First().Categoria.Nome.Should().Be("Categoria Teste");
    }

    [Fact(DisplayName = "AC03 - POST /api/produtos cria e retorna 201")]
    public async Task AC03_PostProduto_CriaERetorna201()
    {
        var categoria = await CreateCategoria("Categoria Producao", null);
        var dto = new CreateProdutoDto("Produto Novo", "Descricao", 49.90m, categoria.Id);

        var response = await _client.PostAsJsonAsync("/api/produtos", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var criado = await response.Content.ReadFromJsonAsync<ProdutoDto>();
        criado.Should().NotBeNull();
        criado!.Nome.Should().Be("Produto Novo");
        criado.CategoriaId.Should().Be(categoria.Id);
    }

    [Fact(DisplayName = "AC03 - PUT /api/produtos/{id} atualiza e retorna 204")]
    public async Task AC03_PutProduto_AtualizaERetorna204()
    {
        var categoria = await CreateCategoria("Categoria Update", null);
        var produto = await CreateProduto("Produto Original", 10m, categoria.Id);

        var update = new UpdateProdutoDto("Produto Atualizado", null, 20m, categoria.Id);
        var response = await _client.PutAsJsonAsync($"/api/produtos/{produto.Id}", update);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "AC03 - DELETE /api/produtos/{id} retorna 204")]
    public async Task AC03_DeleteProduto_RetornaNoContent()
    {
        var categoria = await CreateCategoria("Categoria Delete", null);
        var produto = await CreateProduto("Produto Delete", 1m, categoria.Id);

        var response = await _client.DeleteAsync($"/api/produtos/{produto.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "AC05 - POST produto com nome curto retorna 400 com sumario")]
    public async Task AC05_PostProdutoNomeCurto_DeveRetornar400ComSumario()
    {
        var categoria = await CreateCategoria("Categoria Para Validacao", null);
        var dto = new CreateProdutoDto("abc", null, 10m, categoria.Id);

        var response = await _client.PostAsJsonAsync("/api/produtos", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey("Nome");
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
