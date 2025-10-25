namespace LoanPro.Domain.Models;

public sealed record LoanParameters(
    decimal Principal,
    decimal AnnualNominalRate,
    int Months,
    int CurrencyDecimals = 2
);
