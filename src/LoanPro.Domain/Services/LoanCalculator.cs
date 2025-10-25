namespace LoanPro.Domain.Services;

using LoanPro.Domain.Errors;
using LoanPro.Domain.Models;

/// <summary>
/// Provides loan calculation logic (French amortization method).
/// Handles both total-only results and full amortization schedules.
/// </summary>
public static class LoanCalculator
{
    /// <summary>
    /// Calculates the fixed monthly payment (and optional amortization schedule)
    /// using the French annuity formula.
    /// </summary>
    /// <param name="p">Loan parameters (principal, rate, months, decimals).</param>
    /// <param name="generateSchedule">
    /// If true, returns per-installment details; otherwise, returns totals only.
    /// </param>
    /// <returns>LoanResult with payment info and optional schedule.</returns>
    public static LoanResult Calculate(LoanParameters p, bool generateSchedule)
    {
        decimal totalInterest;
        decimal totalPaid;

        // Step 1: Validate input parameters according to domain rules.
        Validate(p);

        // Step 2: Convert annual nominal rate to monthly nominal rate.
        // Example: 0.45 (45% annual) → 0.45 / 12 = 0.0375 (3.75% monthly nominal)
        var r = p.AnnualNominalRate / 12m;

        // Step 3: Compute the monthly payment.
        decimal payment;

        if (r == 0m)
        {
            // Case A: Zero interest rate → linear repayment (equal parts of principal)
            payment = p.Principal / p.Months;
        }
        else
        {
            // Case B: Positive interest rate → French annuity formula:
            // payment = P * r / (1 - (1 + r)^(-n))
            var onePlusRPowerMinusN = Pow(1m + r, -p.Months);
            payment = p.Principal * r / (1m - onePlusRPowerMinusN);
        }

        // Round payment to currency precision (e.g., 2 decimals)
        payment = Round(payment, p.CurrencyDecimals);

        // Step 4: Fast path — if only totals are needed, avoid loop.
        if (!generateSchedule)
        {
            totalPaid = Round(payment * p.Months, p.CurrencyDecimals);
            totalInterest = Round(totalPaid - p.Principal, p.CurrencyDecimals);

            return new LoanResult(payment, totalPaid, totalInterest);
        }

        // Step 5: Build amortization schedule
        var remaining = p.Principal;
        var schedule = new List<LoanInstallment>(p.Months);

        totalInterest = 0m;
        totalPaid = 0m;

        // Loop through each month
        for (int k = 1; k <= p.Months; k++)
        {
            // Compute monthly interest portion.
            // If r = 0, no interest is accrued.
            decimal interest = (r == 0m)
                ? 0m
                : Round(remaining * r, p.CurrencyDecimals);

            // Principal portion is the remaining part of the payment.
            decimal principalPart = payment - interest;

            // On the last installment, adjust to eliminate rounding drift.
            // Ensures that the remaining principal becomes exactly zero.
            if (k == p.Months)
            {
                principalPart = Round(remaining, p.CurrencyDecimals);
                payment = Round(principalPart + interest, p.CurrencyDecimals);
            }

            // Update remaining principal after this installment.
            remaining = Round(remaining - principalPart, p.CurrencyDecimals);

            // Accumulate totals for summary.
            totalInterest += interest;
            totalPaid += payment;

            // Add the current installment to the schedule.
            schedule.Add(new LoanInstallment(
                Number: k,
                Payment: payment,
                InterestPortion: interest,
                PrincipalPortion: principalPart,
                RemainingPrincipal: remaining
            ));
        }

        // Step 6: Final rounding for accumulated totals.
        totalInterest = Round(totalInterest, p.CurrencyDecimals);
        totalPaid = Round(totalPaid, p.CurrencyDecimals);

        // Step 7: Return complete loan summary.
        // MonthlyPayment = first installment payment (constant for all).
        return new LoanResult(
            MonthlyPayment: schedule[0].Payment,
            TotalPaid: totalPaid,
            TotalInterest: totalInterest,
            Schedule: schedule
        );
    }

    /// <summary>
    /// Validates loan parameters and throws DomainValidationException if invalid.
    /// </summary>
    private static void Validate(LoanParameters p)
    {
        var errors = new List<string>();

        if (p.Principal <= 0)
            errors.Add("Principal must be greater than 0.");

        if (p.Months <= 0)
            errors.Add("Months must be greater than 0.");

        if (p.AnnualNominalRate < 0)
            errors.Add("Annual nominal rate cannot be negative.");

        if (p.CurrencyDecimals < 0 || p.CurrencyDecimals > 6)
            errors.Add("Currency decimals must be between 0 and 6.");

        if (errors.Count > 0)
            throw new DomainValidationException(errors);
    }

    /// <summary>
    /// Rounds a decimal value to the specified number of decimals
    /// using MidpointRounding.ToEven (banker's rounding).
    /// </summary>
    private static decimal Round(decimal value, int decimals) =>
        Math.Round(value, decimals, MidpointRounding.ToEven);

    /// <summary>
    /// Integer exponent power for decimals (supports negative exponents).
    /// Implemented manually to avoid precision loss from double conversions.
    /// </summary>
    private static decimal Pow(decimal baseValue, int exponent)
    {
        if (exponent == 0) return 1m;

        bool negativeExponent = exponent < 0;
        int n = Math.Abs(exponent);
        decimal result = 1m;
        decimal factor = baseValue;

        // Exponentiation by squaring (efficient integer power)
        while (n > 0)
        {
            if ((n & 1) == 1)
                result *= factor;

            factor *= factor;
            n >>= 1;
        }

        return negativeExponent ? 1m / result : result;
    }
}
