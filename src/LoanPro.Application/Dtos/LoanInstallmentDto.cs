namespace LoanPro.Application.Dtos;

public sealed record LoanInstallmentDto(
    int Number,
    decimal Payment,
    decimal InterestPortion,
    decimal PrincipalPortion,
    decimal RemainingPrincipal
);
