# 9-Block Talent Matrix

9-Box talent evaluation platform: employees are plotted on a 3x3 grid (Performance x Potential) per review cycle.

| | |
|---|---|
| Backend | C# / .NET 9.0 ASP.NET Core Web API (MVC), EF Core, SQL Server |
| Frontend | ReactJS (Vite + esbuild) |
| Architecture | Model-View-Controller, single solution |

## Structure

```
server/
  NineBlock.sln
  src/NineBlock.Api/        API project (Controllers, Models, DTOs, Data, Services)
  tests/NineBlock.Api.Tests/ xUnit tests (positive, negative, boundary cases)
client/
  src/pages/                 route-level pages
  src/components/            GridCanvas, GridCell, EmployeeCard
  src/api/                   API client
data/
  whitebox_metrics_seed.*    Testable White Box metric reference data
```

## Running locally

Backend (requires .NET 9 SDK + SQL Server):
```bash
cd server
dotnet restore
dotnet ef database update --project src/NineBlock.Api
dotnet run --project src/NineBlock.Api
```

Frontend (requires Node.js):
```bash
cd client
npm install
npm run dev
```

Tests:
```bash
cd server
dotnet test
```
