﻿namespace Binance.Domain.Exceptions.Custom;

public class UnprocessableEntityDomainException : ApplicationException
{
    public UnprocessableEntityDomainException(string msg) : base(msg) { }
}