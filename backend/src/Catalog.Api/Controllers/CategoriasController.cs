using Catalog.Application.Categorias;
using Catalog.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/categorias")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var categorias = await _service.ListAsync(cancellationToken);
        return Ok(categorias);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaDto dto, CancellationToken cancellationToken)
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => Problem(title: "Not Found", detail: result.Detail, statusCode: StatusCodes.Status404NotFound),
            ResultStatus.Conflict => Problem(title: "Conflict", detail: result.Detail, statusCode: StatusCodes.Status409Conflict),
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
