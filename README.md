# LoanPro

**LoanPro** is a lightweight .NET 8 application that calculates loan payments and generates amortization schedules.

---

## Architecture Overview

The project follows a **clean, layered architecture** with clear separation of concerns:

```
LoanPro/
├─ LoanPro.sln
├─ src/
│  ├─ LoanPro.Api/            # Presentation layer (REST API)
│  ├─ LoanPro.Application/    # Application layer (use cases, DTOs, orchestration)
│  └─ LoanPro.Domain/         # Domain layer (business rules, validation, pure logic)
└─ tests/
   ├─ LoanPro.Domain.UnitTests/
   └─ LoanPro.Application.UnitTests/
```

- **LoanPro.Domain**  
  Contains the core business logic and validation rules for loan calculations.
  - Entities: `LoanParameters`, `LoanInstallment`, `LoanResult`
  - Domain Service: `LoanCalculator`
  - Custom Exception: `DomainValidationException`

- **LoanPro.Application**  
  Contains use cases (`LoanUseCase`), DTOs for request/response, and orchestrates calls to the domain layer.

- **LoanPro.Api**  
  Minimal REST API exposing loan calculation endpoints.
  Uses dependency injection to connect the layers.

- **tests**  
  Unit tests using xUnit

---

## Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-username/LoanPro.git
cd LoanPro
```

### 2. Build the solution

```bash
dotnet build
```

### 3. Run the API

```bash
dotnet run --project src/LoanPro.Api
```

By default, the API runs on `http://localhost:5000`.

Swagger: `http://localhost:5000/swagger`.

There you can:
- Explore all endpoints (`/loans/calculate`, etc.)
- View input and output schemas (DTOs)
- Execute sample requests directly from the UI

### How it works
Swagger is powered by **Swashbuckle.AspNetCore** and automatically reads:
- Endpoint metadata (e.g. `WithSummary`, `WithDescription`)
- XML documentation from DTOs (if enabled in `.csproj`)

**Example configuration summary:**
- Package: `Swashbuckle.AspNetCore`
- Enabled via `AddSwaggerGen()` and `UseSwagger()` in `Program.cs`
- XML comments included from:
  - `LoanPro.Api.xml`
  - `LoanPro.Application.xml`

---

## Example Request

### Endpoint
```
POST /loans/calculate
```

### Request Body
```json
{
  "principal": 100000,
  "annualNominalRate": 0.45,
  "months": 12,
  "generateSchedule": true,
  "currencyDecimals": 2
}
```

### Sample cURL command
```bash
curl -s http://localhost:5000/loans/calculate   -H "Content-Type: application/json"   -d '{
    "principal": 100000,
    "annualNominalRate": 0.45,
    "months": 12,
    "generateSchedule": true,
    "currencyDecimals": 2
  }'
```

---

## Project Structure

| Layer | Folder | Responsibility |
|-------|---------|----------------|
| **Domain** | `LoanPro.Domain` | Core business rules and calculations |
| **Application** | `LoanPro.Application` | Use cases and DTOs |
| **API** | `LoanPro.Api` | REST endpoints and DI setup |

---

## Unit Testing (xUnit)

The **LoanPro** solution includes unit tests for both the **Domain** and **Application** layers.  
Tests are written using the [xUnit.net](https://xunit.net/) framework and follow the **Arrange–Act–Assert (AAA)** pattern.

---

### Test Project Structure

```
tests/
├─ LoanPro.Domain.UnitTests/
│  └─ LoanCalculatorTests.cs
└─ LoanPro.Application.UnitTests/
   └─ LoanUseCaseTests.cs
```

Each test project references the corresponding production layer:
- `LoanPro.Domain.UnitTests` → `LoanPro.Domain`
- `LoanPro.Application.UnitTests` → `LoanPro.Application` and `LoanPro.Domain`

---

### Running the Tests

Execute all test projects in the solution:

```bash
dotnet test
```

Or run a specific test project:

```bash
dotnet test tests/LoanPro.Domain.UnitTests
```

xUnit will automatically build the projects, run the tests, and show a summary of passed/failed cases.

---

## Key Concepts

- Clean, decoupled architecture  
- Domain-driven design principles  
- Dependency injection  
- Deterministic, pure business logic  
- Minimal REST API (no database, no external dependencies)

---

## License

MIT License © 2025 — You are free to use, modify, and distribute this code.
