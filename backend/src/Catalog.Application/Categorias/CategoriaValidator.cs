using FluentValidation;

namespace Catalog.Application.Categorias;

public class CreateCategoriaValidator : AbstractValidator<CreateCategoriaDto>
{
    public CreateCategoriaValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(5).WithMessage("O nome deve ter no mínimo 5 caracteres.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");
    }
}

public class UpdateCategoriaValidator : AbstractValidator<UpdateCategoriaDto>
{
    public UpdateCategoriaValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(5).WithMessage("O nome deve ter no mínimo 5 caracteres.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");
    }
}
