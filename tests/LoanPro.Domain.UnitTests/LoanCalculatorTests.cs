using LoanPro.Domain.Errors;
using LoanPro.Domain.Models;
using LoanPro.Domain.Services;

namespace LoanPro.Domain.UnitTests;

public class LoanCalculatorTests
{
    // AAA: Arrange – Act – Assert

    [Fact]
    public void Calculate_ZeroRate_ReturnsLinearPaymentsAndZeroInterest()
    {
        // Arrange
        var p = new LoanParameters(
            Principal: 100_000m,
            AnnualNominalRate: 0m,
            Months: 10,
            CurrencyDecimals: 2
        );

        // Act
        var result = LoanCalculator.Calculate(p, generateSchedule: true);

        // Assert
        Assert.Equal(10_000m, result.MonthlyPayment);           // 100k / 10
        Assert.Equal(100_000m, result.TotalPaid);
        Assert.Equal(0m, result.TotalInterest);

        Assert.NotNull(result.Schedule);
        Assert.Equal(10, result.Schedule!.Count);
        Assert.All(result.Schedule, i => Assert.Equal(0m, i.InterestPortion));
        Assert.True(result.Schedule.Last().RemainingPrincipal == 0m);
    }

    [Fact]
    public void Calculate_PositiveRate_ReturnsExpectedConstantPayment()
    {
        // Arrange
        var p = new LoanParameters(
            Principal: 100_000m,
            AnnualNominalRate: 0.45m, // 45% nominal annual
            Months: 12,
            CurrencyDecimals: 2
        );

        // Act
        var result = LoanCalculator.Calculate(p, generateSchedule: false);

        // Assert
        // NOTE:
        // The LoanCalculator uses a **Nominal Monthly Rate**, computed as (AnnualNominalRate / 12).
        // This differs from using an Effective Monthly Rate ( (1 + TNA)^(1/12) - 1 ),
        // which would yield a slightly smaller monthly payment (~10,376.53).
        //
        // With a nominal monthly rate of 0.0375 (3.75%), the expected monthly payment
        // for a 12-month loan of 100,000 at 45% TNA is approximately 10,501.23.
        //
        // This test validates the nominal-rate formula behavior.
        Assert.InRange(result.MonthlyPayment, 10501.20m, 10501.25m);
        Assert.Equal(Math.Round(result.MonthlyPayment * 12, 2, MidpointRounding.ToEven), result.TotalPaid);
        Assert.Equal(result.TotalPaid - 100_000m, result.TotalInterest);
    }

    [Fact]
    public void Calculate_LastInstallment_AdjustsRoundingDrift_ToReachZero()
    {
        // Arrange
        var p = new LoanParameters(
            Principal: 123_456.78m,
            AnnualNominalRate: 0.1234m,
            Months: 37,
            CurrencyDecimals: 2
        );

        // Act
        var result = LoanCalculator.Calculate(p, generateSchedule: true);

        // Assert
        Assert.NotNull(result.Schedule);
        var last = result.Schedule!.Last();
        Assert.Equal(0m, last.RemainingPrincipal); // must end exactly at zero
    }

    [Fact]
    public void Calculate_ZeroRate_ProducesLinearPayments_NoInterest_NoRoundingDrift()
    {
        // Arrange
        var p = new LoanParameters(
            Principal: 123_456.78m,
            AnnualNominalRate: 0m,
            Months: 11,
            CurrencyDecimals: 2
        );

        // Act
        var result = LoanCalculator.Calculate(p, generateSchedule: true);

        // Assert
        Assert.NotNull(result.Schedule);
        var schedule = result.Schedule!;
        Assert.Equal(p.Months, schedule.Count);

        // Cuota básica (para las primeras N-1)
        var expectedPerInstallment = Math.Round(p.Principal / p.Months, p.CurrencyDecimals, MidpointRounding.ToEven);
        Assert.Equal(expectedPerInstallment, result.MonthlyPayment);

        // Interés total: 0
        Assert.Equal(0m, result.TotalInterest);

        // Total pagado con schedule ajustado: debe igualar EXACTAMENTE al principal
        var expectedTotalPaid = Math.Round(p.Principal, p.CurrencyDecimals, MidpointRounding.ToEven);
        Assert.Equal(expectedTotalPaid, result.TotalPaid);

        // Las primeras N-1 cuotas: interés 0 y pago = cuota básica
        for (int i = 0; i < schedule.Count - 1; i++)
        {
            var inst = schedule[i];
            Assert.Equal(0m, inst.InterestPortion);
            Assert.Equal(expectedPerInstallment, inst.Payment);
            Assert.Equal(inst.Payment, inst.PrincipalPortion);
            Assert.True(inst.RemainingPrincipal >= 0m);
        }

        // Última cuota: ajusta centavos residuales y cierra saldo en 0
        var last = schedule[^1];
        Assert.Equal(0m, last.InterestPortion);
        Assert.Equal(0m, last.RemainingPrincipal);
        Assert.Equal(last.Payment, last.PrincipalPortion);
        // Puede ser ligeramente distinta a expectedPerInstallment (ej. +0.04)
        Assert.InRange(last.Payment, expectedPerInstallment - 1m, expectedPerInstallment + 1m);
    }

    [Fact]
    public void Calculate_ZeroRate_WithoutSchedule_ReturnsTotals_NoInterest()
    {
        // Arrange
        // Chosen to divide exactly: 100,000 / 10 = 10,000.00
        var p = new LoanParameters(
            Principal: 100_000m,
            AnnualNominalRate: 0m,  // 0% rate
            Months: 10,
            CurrencyDecimals: 2
        );

        // Act
        var result = LoanCalculator.Calculate(p, generateSchedule: false);

        // Assert
        // Monthly payment should be principal / months with zero interest.
        Assert.Equal(10_000m, result.MonthlyPayment);

        // Totals: with exact division, total paid equals principal; no interest at all.
        Assert.Equal(100_000m, result.TotalPaid);
        Assert.Equal(0m, result.TotalInterest);
    }

    [Theory]
    [InlineData(0, 0.1, 12, "Principal must be greater than 0.")]
    [InlineData(1000, 0.1, 0, "Months must be greater than 0.")]
    [InlineData(1000, -0.1, 12, "Annual nominal rate cannot be negative.")]
    public void Calculate_InvalidParameters_ThrowsDomainValidationException(
        decimal principal, double rate, int months, string expectedMessageFragment)
    {
        // Arrange
        var p = new LoanParameters(
            Principal: principal,
            AnnualNominalRate: (decimal)rate,
            Months: months,
            CurrencyDecimals: 2
        );

        // Act + Assert
        var ex = Assert.Throws<DomainValidationException>(() =>
            LoanCalculator.Calculate(p, generateSchedule: false));

        Assert.Contains(expectedMessageFragment, string.Join(" | ", ex.Errors));
    }
}
