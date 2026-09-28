# Architecture

```text
                Scholarship CMGroups
                        │
                 ReactJS Frontend
                        │
                 Webpack + Yarn
                        │
                        ↓
                ASP.NET Core 8 API
                        │
                MVC Architecture
                        │
                  MSBuild + NuGet
                        │
                        ↓
                   SQL Server
```

The Excel workbook describes **how to measure** the software. It does not prescribe a different runtime topology.

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

**Build:** Webpack bundles TypeScript/JSX (`webpack.config.js`). Dependencies are installed with **Yarn** (`package.json`, `yarn.lock`).

## Backend (Model + Controller)

ASP.NET Core 8 hosts HTTP controllers. Domain types live in `Models`. Persistence is EF Core against SQL Server.

| Layer | Location |
| --- | --- |
| Controllers | `backend/src/ScholarshipCMGroups.Api/Controllers/` |
| Contracts | `backend/src/ScholarshipCMGroups.Api/Contracts/` |
| Models | `backend/src/ScholarshipCMGroups.Api/Models/` |
| Services | `backend/src/ScholarshipCMGroups.Api/Services/` |
| Repositories | `backend/src/ScholarshipCMGroups.Api/Repositories/` |
| DbContext / migrations | `backend/src/ScholarshipCMGroups.Api/Data/` |
| Tests | `backend/tests/ScholarshipCMGroups.Api.Tests/` |

**Build:** MSBuild compiles the solution through the `dotnet` CLI (`ScholarshipCMGroups.sln`, `*.csproj`). Packages restore through **NuGet**.

## Database

SQL Server holds `Scholarships`, `Applicants`, `UserAccounts`, and `ScholarshipApplications`. Schema scripts and EF migrations are kept in sync.

## Security boundaries

JWT bearer authentication, role checks, object-level authorization, EF Core parameterized access, React text rendering, security headers middleware, and secrets via user-secrets/environment variables — never committed files.
