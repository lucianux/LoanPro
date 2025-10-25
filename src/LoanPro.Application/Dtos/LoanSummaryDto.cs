namespace LoanPro.Application.Dtos;

public sealed record LoanSummaryDto(
    decimal MonthlyPayment,
    decimal TotalPaid,
    decimal TotalInterest,
    IReadOnlyList<LoanInstallmentDto>? Schedule = null
);
