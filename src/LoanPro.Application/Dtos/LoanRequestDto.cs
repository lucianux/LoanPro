namespace LoanPro.Application.Dtos;

public sealed record LoanRequestDto(
    decimal Principal,
    decimal AnnualNominalRate,
    int Months,
    bool GenerateSchedule = false,
    int CurrencyDecimals = 2
);
