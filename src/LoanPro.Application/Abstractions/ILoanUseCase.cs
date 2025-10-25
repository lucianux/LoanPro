namespace LoanPro.Application.Abstractions;

using LoanPro.Application.Dtos;

public interface ILoanUseCase
{
    LoanSummaryDto Calculate(LoanRequestDto request);
}
