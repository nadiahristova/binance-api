﻿namespace Binance.Domain.Exceptions.Custom;

public class UnAuthorizedDomainException : ApplicationException
{
    public UnAuthorizedDomainException(string msg) : base(msg) { }
}