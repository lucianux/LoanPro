namespace LoanPro.Application.Dtos;

/// <summary>
/// Calculation result including monthly payment, totals, and optional schedule.
/// </summary>
public sealed class LoanSummaryDto
{
    /// <summary>Fixed monthly payment calculated using the French method.</summary>
    public decimal MonthlyPayment { get; init; }

    /// <summary>Total amount paid across all installments.</summary>
    public decimal TotalPaid { get; init; }

    /// <summary>Total interest paid across all installments.</summary>
    public decimal TotalInterest { get; init; }

    /// <summary>Optional amortization schedule (one entry per installment).</summary>
    public IReadOnlyList<LoanInstallmentDto>? Schedule { get; init; }
}
