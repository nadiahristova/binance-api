namespace Binance.Domain.Exceptions.Custom;

public class BadRequestDomainException : ApplicationException
{
    public BadRequestDomainException(string msg) : base(msg) { }
}