namespace LoanPro.Application.Dtos;

/// <summary>
/// Input parameters for loan calculation.
/// </summary>
public sealed class LoanRequestDto
{
    /// <summary>Initial principal amount to be financed (must be greater than 0).</summary>
    public decimal Principal { get; init; }

    /// <summary>Annual nominal interest rate (e.g., 0.45 = 45%). Must be non-negative.</summary>
    public decimal AnnualNominalRate { get; init; }

    /// <summary>Total number of monthly installments (must be greater than 0).</summary>
    public int Months { get; init; }

    /// <summary>If true, returns the full amortization schedule; otherwise, only totals.</summary>
    public bool GenerateSchedule { get; init; } = false;

    /// <summary>Currency decimal places used for rounding (0 to 6). Default is 2.</summary>
    public int CurrencyDecimals { get; init; } = 2;
}
