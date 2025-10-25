namespace LoanPro.Domain.Models;

public sealed record LoanInstallment(
    int Number,
    decimal Payment,
    decimal InterestPortion,
    decimal PrincipalPortion,
    decimal RemainingPrincipal
);
