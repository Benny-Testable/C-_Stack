# Architecture

```text
ReactJS (Vite + esbuild)
   ↓  JSON over HTTPS
ASP.NET Core 8 API
   ↓  MVC / application layers
   Controllers → Services → Repositories → EF Core
   ↓
SQL Server
```

The Excel workbook describes **how to measure** the software (White Box, Black Box, Security, Compliance, Performance). It does not prescribe a different runtime topology, so the prompt's stack is used as-is.

## Frontend (View)

The React application is the view layer. It does not embed SQL or authorization rules.

| Area | Location |
| --- | --- |
| Pages | `frontend/src/pages/` |
| Shared components | `frontend/src/components/` |
| API client | `frontend/src/api/` |
| Session | `frontend/src/auth/` |
| Validation | `frontend/src/validation/` |
| Data hooks | `frontend/src/hooks/` |
| Tests | `frontend/src/**/*.test.ts(x)` |

Vite uses esbuild for TypeScript transform and production minify (`frontend/vite.config.ts`). npm is the package manager (`package.json`, `package-lock.json`).

## Backend (Model + Controller)

ASP.NET Core 8 hosts HTTP controllers. Domain types live in `Models`. Persistence is EF Core against SQL Server.

| Layer | Location |
| --- | --- |
| Controllers | `backend/src/ScholarshipCMGroups.Api/Controllers/` |
| Request/response contracts | `backend/src/ScholarshipCMGroups.Api/Contracts/` |
| Models | `backend/src/ScholarshipCMGroups.Api/Models/` |
| Services | `backend/src/ScholarshipCMGroups.Api/Services/` |
| Repositories | `backend/src/ScholarshipCMGroups.Api/Repositories/` |
| DbContext / migrations | `backend/src/ScholarshipCMGroups.Api/Data/` |
| Tests | `backend/tests/ScholarshipCMGroups.Api.Tests/` |

`Program.cs` is composition only: middleware order, authentication, rate limiting, CORS, and controller mapping.

## Database

SQL Server holds:

- `Scholarships` — catalogue
- `Applicants` — people who apply
- `UserAccounts` — login rows (password hashes, roles)
- `ScholarshipApplications` — one application per (scholarship, applicant)

Foreign keys, unique indexes, and check-equivalent constraints are in EF configuration and [database/schema/001-initial-schema.sql](../database/schema/001-initial-schema.sql).

## Security boundaries

- JWT bearer authentication; role names `Applicant` and `Administrator`.
- Object-level checks so an applicant cannot read another applicant's rows (BOLA).
- Parameterized access via EF Core (SQLi metric evidence).
- React text rendering (XSS metric evidence).
- Security headers middleware.
- Secrets supplied through user-secrets or environment variables, never committed files.

## What this architecture is not

The workbook's IaC metrics (open firewall rules, public buckets, CIS benchmarks) apply to infrastructure definitions. This repository is the application. Those metrics stay **Requirement clarification needed** until hosting IaC is supplied.
