using Binance.Domain.Exceptions;
using Binance.Domain.Exceptions.Custom;
using Binance.Infrastructure.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Binance.API.Extensions.Configuration;

public static class MvcBuilderExtensions
{

    public static void AddFluentValidation(this IMvcBuilder builder)
    {
        builder.AddFluentValidation(options =>
        {
            options.RegisterValidatorsFromAssemblyContaining<GetSimpleMovingAverageValidator>();
            options.ImplicitlyValidateChildProperties = true;
        }).ConfigureApiBehaviorOptions(opts => opts.InvalidModelStateResponseFactory = context =>
        {
            var pattern = @"(?<=[A-Za-z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[0-9]?[A-Z])";
            var errors = context.ModelState
                .Select(kvp => new { kvp.Key, kvp.Value.Errors })
                .SelectMany(a => a.Errors.Select(x =>
                    x.ErrorMessage.Replace("''", $"'{Regex.Replace(a.Key, pattern, " ")}'", StringComparison.CurrentCulture)))
                .ToList();

            var httpRequestPath = context.HttpContext.Request.Path;

            return new UnprocessableEntityObjectResult(Rfc7807.Factory<UnprocessableEntityDomainException>(httpRequestPath, errors));
        });
    }
}
