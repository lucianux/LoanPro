namespace LoanPro.Application.Services;

using LoanPro.Application.Abstractions;
using LoanPro.Application.Dtos;
using LoanPro.Domain.Models;
using LoanPro.Domain.Services;

public sealed class LoanUseCase : ILoanUseCase
{
    public LoanSummaryDto Calculate(LoanRequestDto request)
    {
        var parameters = new LoanParameters(
            Principal: request.Principal,
            AnnualNominalRate: request.AnnualNominalRate,
            Months: request.Months,
            CurrencyDecimals: request.CurrencyDecimals
        );

        var result = LoanCalculator.Calculate(parameters, request.GenerateSchedule);

        return new LoanSummaryDto(
            MonthlyPayment: result.MonthlyPayment,
            TotalPaid: result.TotalPaid,
            TotalInterest: result.TotalInterest,
            Schedule: result.Schedule?.Select(i =>
                new LoanInstallmentDto(
                    Number: i.Number,
                    Payment: i.Payment,
                    InterestPortion: i.InterestPortion,
                    PrincipalPortion: i.PrincipalPortion,
                    RemainingPrincipal: i.RemainingPrincipal
                )
            ).ToList()
        );
    }
}
