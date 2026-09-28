# Scholarship CMGroups

Scholarship CMGroups is an EdTech scholarship-management application. Applicants browse programmes, open and submit applications, and can erase their own data. Administrators maintain the catalogue and record review decisions.

The Excel workbook `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx` is the source of truth for **engineering quality metrics** (structure, tests, security, compliance, performance). It does not define scholarship business rules. Application behaviour that is not in the workbook is listed in [docs/clarifications.md](docs/clarifications.md) and is marked provisional.

## Technology stack

| Layer | Choice |
| --- | --- |
| SCM | Git (compatible with GitHub, GitLab, and Bitbucket) |
| Frontend | React, TypeScript, Vite (esbuild), npm |
| Backend | ASP.NET Core 8, C#, dotnet CLI, NuGet |
| Database | Microsoft SQL Server |
| Architecture | MVC-style API (Models, Controllers, Services/Repositories) with a React view layer |

## Architecture

```text
ReactJS (Vite)
   ↓  HTTPS JSON API
ASP.NET Core 8 API
   ↓  Controllers → Services → Repositories
SQL Server
```

Details: [docs/architecture.md](docs/architecture.md).

## Repository structure

This branch lives in the existing [C-_Stack](https://github.com/Mohammed-shihaf/C-_Stack.git) repository. `main` contains only the original README; this project is added on the working branch without changing `main`.

```text
C-_Stack/
├── frontend/          React + Vite client
├── backend/           ASP.NET Core 8 solution
├── database/          SQL schema and catalogue seed
├── docs/              Architecture, setup, metric mapping
└── README.md
```

## Git branch

Work is on **`Scholarship-CMGroups-positive`**.

Git rejects branch names that contain spaces (`git check-ref-format`). The name `Scholarship CMGroups positive` is therefore not a legal ref. Hyphens replace the spaces. See [docs/clarifications.md](docs/clarifications.md) item C-01.

Do not commit or merge this work to `main` unless you are explicitly asked to.

## Setup and run

Step-by-step instructions: [docs/setup.md](docs/setup.md).

Short version, after SQL Server and user-secrets are configured:

```bash
# Backend
cd backend
dotnet restore
dotnet build
dotnet test
dotnet run --project src/ScholarshipCMGroups.Api

# Frontend (separate terminal)
cd frontend
npm ci
npm run test
npm run build
npm run dev
```

The Vite dev server proxies `/api` to `https://localhost:7148`.

## API

| Method | Route | Access |
| --- | --- | --- |
| GET | `/api/health` | Anonymous |
| POST | `/api/auth/register` | Anonymous |
| POST | `/api/auth/login` | Anonymous |
| GET | `/api/scholarships` | Anonymous |
| GET | `/api/scholarships/{id}` | Anonymous |
| POST/PUT/DELETE | `/api/scholarships` | Administrator |
| GET | `/api/scholarships/{id}/statistics` | Administrator |
| GET/POST/PUT | `/api/applications` | Authenticated (applicants see own rows) |
| POST | `/api/applications/{id}/transitions` | Authenticated; role-checked in the service |
| GET/PUT/DELETE | `/api/applicants/{id}` | Owner or administrator |

OpenAPI UI: `https://localhost:7148/swagger` in Development.

## Database

Four tables: `Scholarships`, `Applicants`, `UserAccounts`, `ScholarshipApplications`. Scripts:

- [database/schema/001-initial-schema.sql](database/schema/001-initial-schema.sql)
- [database/seed/001-reference-scholarships.sql](database/seed/001-reference-scholarships.sql)
- EF Core migrations under `backend/src/ScholarshipCMGroups.Api/Data/Migrations/`

No applicant or credential rows are seeded in source control.

## Metric mapping

Every Excel L5 metric is listed in [docs/metric-mapping.md](docs/metric-mapping.md). Rows that need a value the workbook does not give are marked **Requirement clarification needed**.

## Testing

```bash
cd backend
dotnet test --collect:"XPlat Code Coverage"

cd ../frontend
npm run test
npm run test:coverage
npm run lint
```

## Configuration

Secrets are not in source. Use `dotnet user-secrets` or environment variables for the SQL connection string, JWT signing key, and optional bootstrap administrator. See [docs/setup.md](docs/setup.md) and [docs/dependencies.md](docs/dependencies.md).
