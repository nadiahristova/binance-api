using Binance.Domain.Dtos;
using FluentValidation;

namespace Binance.Infrastructure.Validation;

public class GetSimpleMovingAverageValidator : AbstractValidator<GetSimpleMovingAverageDto>
{
    private readonly DateTime _minDate = DateTime.UtcNow.AddYears(-2);

    public GetSimpleMovingAverageValidator()
    {
        RuleFor(x => x.n)
            .GreaterThan(0)
            .LessThanOrEqualTo(_ => 1000);

        RuleFor(x => x.p)
            .IsInEnum();

        RuleFor(x => x.s)
            .GreaterThanOrEqualTo(_ => _minDate)
            .LessThanOrEqualTo(_ => DateTime.UtcNow)
            .When(x => x.s.HasValue);
    }
}
