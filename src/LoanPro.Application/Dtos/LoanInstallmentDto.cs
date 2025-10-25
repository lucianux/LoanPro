namespace LoanPro.Application.Dtos;

/// <summary>
/// Represents a single installment in the amortization schedule.
/// </summary>
public sealed class LoanInstallmentDto
{
    /// <summary>Installment sequence number (1-based).</summary>
    public int Number { get; init; }

    /// <summary>Total payment for the installment (principal + interest).</summary>
    public decimal Payment { get; init; }

    /// <summary>Interest portion of the payment.</summary>
    public decimal InterestPortion { get; init; }

    /// <summary>Principal portion of the payment.</summary>
    public decimal PrincipalPortion { get; init; }

    /// <summary>Remaining principal after this installment.</summary>
    public decimal RemainingPrincipal { get; init; }
}
