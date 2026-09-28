# Dependencies

Only packages the application or its tests actually use. Purpose is stated so SCA/license metrics have a readable inventory next to the lockfiles.

## Backend (NuGet)

Defined in `backend/src/ScholarshipCMGroups.Api/ScholarshipCMGroups.Api.csproj` and the test project. Restore with `dotnet restore`.

| Package | Purpose |
| --- | --- |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server mapping |
| `Microsoft.EntityFrameworkCore.Design` | `dotnet ef migrations` |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT validation |
| `Microsoft.Extensions.Identity.Core` | `PasswordHasher<T>` only |
| `Swashbuckle.AspNetCore` | OpenAPI document |
| `Azure.Identity` | Direct pin to a non-vulnerable version pulled otherwise by SqlClient |
| `System.Formats.Asn1` | Same, transitive CVE pin |
| `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio` | Test host |
| `coverlet.collector` | Cobertura coverage |
| `Microsoft.EntityFrameworkCore.InMemory` | Tests without a live SQL Server |
| `Microsoft.AspNetCore.Mvc.Testing` | In-process API tests |

NuGet audit is enabled in `backend/Directory.Build.props` (`NuGetAudit=true`, mode `all`, level `low`).

## Frontend (npm)

Defined in `frontend/package.json`. Locked in `frontend/package-lock.json`. Install with `npm ci`.

| Package | Purpose |
| --- | --- |
| `react`, `react-dom` | UI |
| `react-router-dom` | Routes |
| `vite`, `@vitejs/plugin-react` | Dev server and production build (esbuild transform/minify) |
| `typescript` | Typecheck |
| `eslint`, `typescript-eslint`, `eslint-plugin-react-hooks`, `eslint-plugin-react-refresh` | Lint |
| `vitest`, `@vitest/coverage-v8`, `jsdom` | Unit/component tests and coverage |
| `@testing-library/react`, `@testing-library/user-event`, `@testing-library/jest-dom` | DOM tests |
| `jscpd` | Duplication scan (`npm run duplication`) |

No analytics, tracking, or payment SDKs.

## Build artefacts the metrics read

| Artefact | Produced by |
| --- | --- |
| `backend/ScholarshipCMGroups.sln` | Solution |
| `*.csproj` | Project + NuGet references |
| `frontend/package-lock.json` | npm lockfile |
| `frontend/vite.config.ts` | Vite/esbuild settings |
| `frontend/dist/` | `npm run build` (gitignored) |
| `coverage/cobertura-coverage.xml` | `npm run test:coverage` (gitignored) |
| Coverlet Cobertura under `TestResults/` | `dotnet test --collect:"XPlat Code Coverage"` (gitignored) |
