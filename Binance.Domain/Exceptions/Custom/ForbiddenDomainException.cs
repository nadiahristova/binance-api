namespace Binance.Domain.Exceptions.Custom;

public class ForbiddenDomainException : ApplicationException
{
    public ForbiddenDomainException(string msg) : base(msg) { }
}