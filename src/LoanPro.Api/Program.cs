using LoanPro.Application.Abstractions;
using LoanPro.Application.Dtos;
using LoanPro.Application.Services;
using LoanPro.Domain.Errors;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddSingleton<ILoanUseCase, LoanUseCase>();

var app = builder.Build();

app.MapGet("/", () => "LoanPro API up");

app.MapPost("/loans/calculate", (LoanRequestDto request, ILoanUseCase useCase) =>
{
    try
    {
        var result = useCase.Calculate(request);
        return Results.Ok(result);
    }
    catch (DomainValidationException vex)
    {
        return Results.ValidationProblem(
            vex.Errors.ToDictionary(e => e, _ => new[] { "Invalid" }),
            title: "Validation failed",
            statusCode: StatusCodes.Status400BadRequest
        );
    }
});

app.Run();
