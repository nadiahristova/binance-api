using Binance.Domain.Exceptions.Custom;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.RegularExpressions;

namespace Binance.Domain.Exceptions;

public static class Rfc7807
{
    /// <summary>
    /// Gets Rfc7807Object equivalent of an exception
    /// </summary>
    /// <param name="appException">Application exception</param>
    /// <param name="httpRequestPath">Request path</param>
    /// <returns>Rfc7807Object</returns>
    public static Rfc7807Object Factory(Exception appException, string httpRequestPath)
    {
        var errors = new List<string>() { appException.Message ?? string.Empty };

        return ReturnRfc7807Object(appException.GetType(), httpRequestPath, errors);
    }

    /// <summary>
    /// Gets Rfc7807Object equivalent of an exception type
    /// </summary>
    /// <typeparam name="T">Application exception type</typeparam>
    /// <param name="httpRequestPath">Request path</param>
    /// <param name="errors">Error msgs</param>
    /// <returns>Rfc7807Object</returns>
    public static Rfc7807Object Factory<T>(string httpRequestPath, List<string> errors)
        where T : Exception
    {
        var exceptionType = typeof(T);

        return ReturnRfc7807Object(exceptionType, httpRequestPath, errors);
    }

    private static Rfc7807Object ReturnRfc7807Object(Type exceptionType, string httpRequestPath, List<string> errors)
    {
        int statusCode = exceptionType switch
        {
            Type et when et == typeof(BadRequestDomainException) => (int)HttpStatusCode.BadRequest,
            Type et when et == typeof(ForbiddenDomainException) => (int)HttpStatusCode.Forbidden,
            Type et when et == typeof(NotFoundDomainException) => (int)HttpStatusCode.NotFound,
            Type et when et == typeof(ConflictDomainException) => (int)HttpStatusCode.Conflict,
            Type et when et == typeof(UnAuthorizedDomainException) => (int)HttpStatusCode.Unauthorized,
            Type et when et == typeof(UnprocessableEntityDomainException) => (int)HttpStatusCode.UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

        return new Rfc7807Object
        {
            Title = exceptionType.IsSubclassOf(typeof(ApplicationException)) 
                        ? Regex.Replace(exceptionType.Name, "([a-z])([A-Z])", "$1 $2")
                        : "Internal Domain Exception",
            Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{(int)statusCode}",
            Errors = errors,
            Status = statusCode,
            Instance = httpRequestPath
        };
    }
}
