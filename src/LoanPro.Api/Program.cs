using System.Reflection;
using LoanPro.Application.Abstractions;
using LoanPro.Application.Dtos;
using LoanPro.Application.Services;
using LoanPro.Domain.Errors;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddSingleton<ILoanUseCase, LoanUseCase>();

// 🔹 Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "LoanPro API",
        Version = "v1",
        Description = "Loan payment and amortization schedule calculator (French method)."
    });

    // Include XML docs (if enabled in csproj)
    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// 🔹 Swagger middleware (enable in all envs; si querés solo dev, envolvé en if)
app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "LoanPro API v1");
    o.DocumentTitle = "LoanPro - OpenAPI";
});

// Minimal endpoint
app.MapGet("/", () => "LoanPro API up");

// Calculates the monthly payment and (optionally) the amortization schedule.
// Uses French amortization method (constant payment).
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
})
.WithName("CalculateLoan")                    // it shows in Swagger
.WithSummary("Calculate loan payment and schedule")
.WithDescription("Returns monthly payment, totals and optional amortization schedule.");

app.Run();
