using FluentValidation;

namespace Catalog.Application.Produtos;

public class CreateProdutoValidator : AbstractValidator<CreateProdutoDto>
{
    public CreateProdutoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(5).WithMessage("O nome deve ter no mínimo 5 caracteres.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");

        RuleFor(x => x.Preco)
            .GreaterThanOrEqualTo(0).WithMessage("O preço deve ser maior ou igual a zero.");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("A categoria é obrigatória.");
    }
}

public class UpdateProdutoValidator : AbstractValidator<UpdateProdutoDto>
{
    public UpdateProdutoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(5).WithMessage("O nome deve ter no mínimo 5 caracteres.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");

        RuleFor(x => x.Preco)
            .GreaterThanOrEqualTo(0).WithMessage("O preço deve ser maior ou igual a zero.");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("A categoria é obrigatória.");
    }
}
