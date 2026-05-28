using Catalog.Application.Common;
using Catalog.Application.Produtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/produtos")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutosController(IProdutoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProdutoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var produtos = await _service.ListAsync(cancellationToken);
        return Ok(produtos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProdutoDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return result.Status switch
        {
            ResultStatus.Success => CreatedAtAction(nameof(List), new { id = result.Value!.Id }, result.Value),
            ResultStatus.ValidationFailed => ValidationProblem(BuildModelState(result.Errors!)),
            _ => Problem(detail: result.Detail, statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProdutoDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => Problem(title: "Not Found", detail: result.Detail, statusCode: StatusCodes.Status404NotFound),
            ResultStatus.ValidationFailed => ValidationProblem(BuildModelState(result.Errors!)),
            _ => Problem(detail: result.Detail, statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => Problem(title: "Not Found", detail: result.Detail, statusCode: StatusCodes.Status404NotFound),
            _ => Problem(detail: result.Detail, statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private static ModelStateDictionary BuildModelState(IDictionary<string, string[]> errors)
    {
        var modelState = new ModelStateDictionary();
        foreach (var (key, messages) in errors)
        {
            foreach (var message in messages)
            {
                modelState.AddModelError(key, message);
            }
        }
        return modelState;
    }
}
