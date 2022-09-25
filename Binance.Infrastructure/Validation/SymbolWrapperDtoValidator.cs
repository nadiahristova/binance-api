using FluentValidation;
using Binance.Domain.Dtos;

namespace Binance.Infrastructure.Validation;

public class SymbolWrapperDtoValidator : AbstractValidator<SymbolWrapperDto>
{
    public SymbolWrapperDtoValidator()
    {
        RuleFor(w => w.Symbol)
            .Matches(@"^[a-zA-Z]+$")
            .Length(7);
    }
}
