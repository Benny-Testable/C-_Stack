# 9-Block Talent Matrix Platform (`9-Block-positive-Cases-Parcel`)

The **9-Block (9-Box Grid)** platform plots employees across two axes —
**Performance** (X) and **Potential** (Y) — into one of nine quadrants
(Block 1 "Enigma" through Block 3 "Star" and Block 7 "Risk"), for
calibration reviews and succession planning.

This branch is the same positive-case implementation as
[`9-Block-positive-Cases`](https://github.com/Mohammed-shihaf/C-_Stack/tree/9-Block-positive-Cases),
which is itself the de-duplicated counterpart of
[`9-Block-Negative-Cases`](https://github.com/Mohammed-shihaf/C-_Stack/tree/9-Block-Negative-Cases),
but built with a **different frontend build tool** (Parcel instead of
Vite) to validate the platform against a second Build Tool &
Package Manager combination.

| | |
|---|---|
| Backend | C# / .NET 9.0 ASP.NET Core Web API (MVC), EF Core |
| Frontend | ReactJS (Parcel) |
| Architecture | Model-View-Controller, single solution |

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
Platform_Stack_Matrix.csv
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

Frontend (requires Node.js, uses Parcel):
```bash
cd frontend
npm install
npm run dev
```
UI on `http://localhost:3000` (Parcel dev server; `/api` proxied to the
backend via `frontend/.proxyrc.json`).
