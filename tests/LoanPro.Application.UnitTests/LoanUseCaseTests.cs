using LoanPro.Application.Abstractions;
using LoanPro.Application.Dtos;
using LoanPro.Application.Services;

namespace LoanPro.Application.UnitTests;

public class LoanUseCaseTests
{
    private readonly ILoanUseCase _useCase = new LoanUseCase();

    [Fact]
    public void Calculate_MapsDomainResultIntoDto_CreatesScheduleWhenRequested()
    {
        // Arrange
        var request = new LoanRequestDto
        {
            Principal = 50_000m,
            AnnualNominalRate = 0.24m, // 24% nominal annual
            Months = 6,
            GenerateSchedule = true,
            CurrencyDecimals = 2
        };

        // Act
        var dto = _useCase.Calculate(request);

        // Assert
        Assert.True(dto.MonthlyPayment > 0);
        Assert.True(dto.TotalPaid >= request.Principal);
        Assert.Equal(dto.TotalPaid - request.Principal, dto.TotalInterest);

        Assert.NotNull(dto.Schedule);
        Assert.Equal(6, dto.Schedule!.Count);

        var first = dto.Schedule.First();
        Assert.Equal(1, first.Number);
        Assert.Equal(first.Payment, dto.MonthlyPayment); // constant payment (French)
    }

    [Fact]
    public void Calculate_WithoutSchedule_DoesNotPopulateSchedule()
    {
        // Arrange
        var request = new LoanRequestDto
        {
            Principal = 10_000m,
            AnnualNominalRate = 0.10m,
            Months = 10,
            GenerateSchedule = false,
            CurrencyDecimals = 2
        };

        // Act
        var dto = _useCase.Calculate(request);

        // Assert
        Assert.NotEqual(0m, dto.MonthlyPayment);
        Assert.Null(dto.Schedule);
    }
}
