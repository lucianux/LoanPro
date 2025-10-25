using System.Reflection;
using LoanPro.Application.Abstractions;
using LoanPro.Application.Dtos;
using LoanPro.Application.Services;
using LoanPro.Domain.Errors;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
builder.Services.AddSingleton<ILoanUseCase, LoanUseCase>();

// --- Swagger services ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "LoanPro API",
        Version = "v1",
        Description = "Loan payment and amortization schedule calculator (French method)."
    });

    // Include XML documentation from the API assembly
    var apiXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXml);
    if (File.Exists(apiXmlPath))
        c.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);

    // Include XML documentation from the Application assembly (for DTOs)
    var appAsmName = typeof(LoanUseCase).Assembly.GetName().Name!;
    var appXml = $"{appAsmName}.xml";
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXml);
    if (File.Exists(appXmlPath))
        c.IncludeXmlComments(appXmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

// --- Swagger only in Development environment ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "LoanPro API v1");
        o.DocumentTitle = "LoanPro - OpenAPI";
    });
}

// Root endpoint
app.MapGet("/", () => "LoanPro API up");

// Loan calculation endpoint
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
.WithName("CalculateLoan")
.WithSummary("Calculate loan payment and schedule")
.WithDescription("Returns monthly payment, totals, and optional amortization schedule.");

app.Run();
