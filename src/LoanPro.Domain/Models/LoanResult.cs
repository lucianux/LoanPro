namespace LoanPro.Domain.Models;

public sealed record LoanResult(
    decimal MonthlyPayment,
    decimal TotalPaid,
    decimal TotalInterest,
    IReadOnlyList<LoanInstallment>? Schedule = null
);
