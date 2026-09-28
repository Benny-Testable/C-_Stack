# 9-Block Talent Matrix Platform (`9-Block-positive-Cases`)

The **9-Block (9-Box Grid)** platform plots employees across two axes —
**Performance** (X) and **Potential** (Y) — into one of nine quadrants
(Block 1 "Enigma" through Block 3 "Star" and Block 7 "Risk"), for
calibration reviews and succession planning.

This branch is the **positive counterpart** of
[`9-Block-Negative-Cases`](https://github.com/Mohammed-shihaf/C-_Stack/tree/9-Block-Negative-Cases):
same MVC architecture and file layout, but the quadrant-resolution logic
that branch duplicates on purpose (to trigger the White Box **Code
Duplication** metric) is instead shared from a single source here, so
that metric passes.

| | |
|---|---|
| Backend | C# / .NET 8.0 (LTS) ASP.NET Core Web API (MVC), EF Core |
| Frontend | ReactJS (Vite + esbuild) |
| Architecture | Model-View-Controller, single solution |

### Branch Stack Matrix
| Branch | Primary Pair | C# / Language Build Tool | Package Manager | Project Structure |
|---|---|---|---|---|
| `9-Block-positive-Cases` | C# (.NET 8.0) | dotnet CLI | NuGet | **MVC** |

| Branch | Language | Build Tool & Package Manager | Architecture Style | Description / Role |
| :--- | :--- | :--- | :--- | :--- |
| `9-Block-positive-Cases` | C# / .NET 8.0 (LTS) & JavaScript (ReactJS) | dotnet + NuGet & Vite (esbuild) + npm | MVC | C# ASP.NET Core Web API + SQL Server (EF Core) + ReactJS single solution. 9-Box talent matrix evaluation platform with clean, non-duplicated positive-case implementations (Code Duplication metric passes). |

## Structure

```
backend/
  NineBlock.slnx
  NineBlock.Api/
    Controllers/    AssessmentController, EmployeeController, NineBoxGridController
    Models/         Employee, Assessment, NineBoxQuadrant, ReviewCycle
    Services/       NineBoxMatrixService (single source of quadrant logic)
                     EmployeeEvaluationService (delegates to NineBoxMatrixService)
    Data/           NineBlockDbContext
frontend/
  src/
    components/     NineBoxGrid.jsx, CalibrationBoard.jsx
    views/          Dashboard.jsx
    utils/          quadrantUtils.js (single source of coordinate/color logic)
    services/       api.js
```

## No-duplication fix (vs. the negative-cases branch)

- **Backend**: `EmployeeEvaluationService.ResolveNineBoxQuadrant` delegates
  to `NineBoxMatrixService.ResolveNineBoxQuadrant` instead of re-implementing it.
- **Frontend**: `getQuadrantCoordinates` / `getQuadrantColor` live once in
  `frontend/src/utils/quadrantUtils.js`; both `NineBoxGrid.jsx` and
  `CalibrationBoard.jsx` import from there instead of each defining their own copy.

Verify with `npx jscpd backend/NineBlock.Api/Services/` and
`npx jscpd frontend/src/components/` — both should report 0% duplication.

## Running locally

Backend (requires .NET 9 SDK):
```bash
cd backend/NineBlock.Api
dotnet restore
dotnet run
```
API on `http://localhost:5253` (or `https://localhost:7263`).

Frontend (requires Node.js):
```bash
cd frontend
npm install
npm run dev
```
UI on `http://localhost:3000`.
