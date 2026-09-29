# West Coast Fitness Club

Arizona fitness-club application: React + Vite + TypeScript, ASP.NET Core 8 Clean Architecture, PostgreSQL.

Branch `SR_west-coast-fitness-club`. The reference branch `Scholarship-CMGroups-positive` is unchanged.

## Layout

```text
frontend/                         React, Vite, TypeScript, Vitest, ESLint
backend/src/WestCoastFitness.Domain
backend/src/WestCoastFitness.Application
backend/src/WestCoastFitness.Infrastructure   EF Core 8 + Npgsql, migrations
backend/src/WestCoastFitness.Api              ASP.NET Core 8
backend/tests/WestCoastFitness.Application.Tests
database/schema/001-initial-schema.sql        generated from the EF migration
```

## Runtimes used

| Piece | Version |
| --- | --- |
| .NET SDK | 8.0.420, `global.json` asks for 8.0.100 with `latestFeature` roll-forward |
| ASP.NET Core / JWT bearer | 8.0.31 |
| EF Core + Npgsql provider | 8.0.11 (last 8.0 provider release; it does not track EF 8.0.31) |
| Node | 22.14.0 |
| Frontend packages | pinned in `frontend/package.json` and `frontend/package-lock.json` |

## Local run

PostgreSQL and a signing key are supplied by the environment. They are not committed.

```powershell
$env:POSTGRES_PASSWORD = "<local-password>"
$env:JWT_SIGNING_KEY = "<at-least-32-characters>"
docker compose up --build
```

Apply the schema once the database is up:

```powershell
$env:ConnectionStrings__Postgres = "Host=localhost;Port=5432;Database=westcoastfitness;Username=fitness;Password=$env:POSTGRES_PASSWORD"
dotnet tool restore
dotnet ef database update --project backend/src/WestCoastFitness.Infrastructure --startup-project backend/src/WestCoastFitness.Infrastructure
```

Then, for the API outside compose:

```powershell
$env:Jwt__SigningKey = $env:JWT_SIGNING_KEY
dotnet run --project backend/src/WestCoastFitness.Api
```

Frontend:

```powershell
cd frontend
npm ci
npm run dev
```

The UI is at http://localhost:5173 and proxies `/api` to http://localhost:5080.

Payments are a mock gateway in `MockPaymentGateway`. Amounts of zero or less are declined. No card data is stored.

## Tests

```powershell
dotnet test WestCoastFitness.sln --collect:"XPlat Code Coverage"
cd frontend
npm test
npm run lint
npm run build
```
