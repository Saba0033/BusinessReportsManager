# BusinessReportsManager

Production-ready ASP.NET Core Web API backend for an internal tour company system where employees manage orders, parties, tours, passengers, suppliers, banks, payments, and exchange rates.

## Tech stack

- .NET 8 (C# 12)
- ASP.NET Core Web API (Controllers)
- EF Core 8 (SQL Server)
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger / Swashbuckle (+ examples)
- AutoMapper
- FluentValidation
- Logging
- Clean Architecture (Api → Application + Infrastructure; Application → Domain; Infrastructure → Domain)

## Prerequisites

- **.NET 8 SDK**
- **SQL Server** (LocalDB works): default connection string is in `BusinessReportsManager.Api/appsettings.json`  
  `Server=(localdb)\MSSQLLocalDB;Database=BusinessReportsManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

> **Note:** All dates/times are treated as **UTC** in the API.

## Getting started

```bash
# 1) Restore & build
dotnet restore
dotnet build

# 2) Update the database (applies EF Core migrations from Infrastructure)
dotnet ef database update --startup-project BusinessReportsManager.Api --project BusinessReportsManager.Infrastructure

# (Optional) Or simply run the API, it will apply migrations automatically on startup:
dotnet run --project BusinessReportsManager.Api
```

Once the API is running, open Swagger UI at the shown URL (typically `https://localhost:5001/swagger`).

## Seeded roles & users

Roles:
- Employee
- Accountant
- Supervisor

Users:
- `employee1@demo.local` / `P@ssw0rd!`
- `accountant1@demo.local` / `P@ssw0rd!`
- `supervisor1@demo.local` / `P@ssw0rd!`

Also seeded:
- Banks: “TBC Bank”, “Bank of Georgia”
- Suppliers: 2 sample suppliers
- One Tour with destinations
- One Order with passengers + partial payment (=> `PartiallyPaid`)
- Exchange rates for GEL↔USD, GEL↔EUR

## Login (JWT) in Swagger

1. Use **POST /api/auth/login** with a seeded user (e.g., `employee1@demo.local` / `P@ssw0rd!`).
2. Copy the returned token.
3. Click **Authorize** in Swagger and paste the token as: `Bearer {token}`.

## RBAC Policies

- `CanViewAllOrders` → **Accountant** or **Supervisor**
- `CanEditAllOrders` → **Accountant** or **Supervisor**
- `CanEditOwnOpenOrders` → **Employee** (enforced inside order operations for ownership + status)

## Concurrency

Order updates use a row-version column. Provide the `RowVersionBase64` field from the GET details in your PUT payload. Conflicts return **409** with ProblemDetails.

## Swagger Examples

Swagger includes example payloads for:

- `CreateOrderDto`
- `CreateTourDto`
- `CreatePassengerDto`
- `CreatePaymentDto`

## Project Layout

```
BusinessReportsManager.sln
├─ BusinessReportsManager.Domain
│  ├─ Entities/
│  ├─ ValueObjects/
│  ├─ Enums/
│  ├─ Interfaces/
│  └─ Queries/
├─ BusinessReportsManager.Application
│  ├─ DTOs/
│  ├─ Services/
│  ├─ Mappings/
│  ├─ Validation/
│  └─ Common/
├─ BusinessReportsManager.Infrastructure
│  ├─ DataAccess/
│  │  ├─ AppDbContext.cs
│  │  ├─ Configurations/
│  │  ├─ Migrations/
│  │  └─ Seeders/
│  ├─ Services/
│  ├─ Identity/
│  └─ Security/
└─ BusinessReportsManager.Api
   ├─ Controllers/
   ├─ Filters/
   ├─ Extensions/
   └─ Program.cs
```