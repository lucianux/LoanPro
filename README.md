# LoanPro

**LoanPro** is a lightweight .NET 8 application that calculates loan payments and generates amortization schedules.

---

## Architecture Overview

The project follows a **clean, layered architecture** with clear separation of concerns:

```
/src
├─ LoanPro.Api/          # Presentation layer (REST API)
├─ LoanPro.Application/  # Application layer (use cases, DTOs, orchestration)
└─ LoanPro.Domain/       # Domain layer (business rules, validation, pure logic)
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

Swagger: open your browser and navigate to: http://localhost:5000/swagger

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

## Testing (coming soon)

Unit tests will be added for:
- `LoanCalculator` (domain logic)
- `LoanUseCase` (application mapping and orchestration)

Run tests with:

```bash
dotnet test
```

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
