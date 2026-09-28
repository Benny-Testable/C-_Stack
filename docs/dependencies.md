# Dependencies

Branch **`Scholarship-CMgroups-Positive1`** uses **Webpack + Yarn** on the frontend and **MSBuild + NuGet** on the backend.

## Backend (NuGet / MSBuild)

Defined in `backend/src/ScholarshipCMGroups.Api/ScholarshipCMGroups.Api.csproj` and the test project.

| Package | Purpose |
| --- | --- |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server mapping |
| `Microsoft.EntityFrameworkCore.Design` | EF migrations |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT validation |
| `Microsoft.Extensions.Identity.Core` | Password hashing |
| `Swashbuckle.AspNetCore` | OpenAPI document |
| `Microsoft.NET.Test.Sdk`, `xunit`, `coverlet.collector` | Tests and coverage |
| `Microsoft.EntityFrameworkCore.InMemory` | In-memory tests |
| `Microsoft.AspNetCore.Mvc.Testing` | API integration tests |

Build: `dotnet build` invokes **MSBuild**. Restore: `dotnet restore` uses **NuGet**.

NuGet audit is enabled in `backend/Directory.Build.props`.

## Frontend (Yarn / Webpack)

Defined in `frontend/package.json`. Locked in **`yarn.lock`**. Install with `yarn install` only — not npm.

| Package | Purpose |
| --- | --- |
| `react`, `react-dom`, `react-router-dom` | UI |
| `webpack`, `webpack-cli`, `webpack-dev-server` | Bundling and dev server |
| `html-webpack-plugin` | HTML shell |
| `ts-loader`, `typescript` | TypeScript compilation |
| `css-loader`, `style-loader` | Stylesheets |
| `jest`, `ts-jest`, `jest-environment-jsdom` | Unit/component tests |
| `@testing-library/react`, `@testing-library/user-event`, `@testing-library/jest-dom` | DOM tests |
| `eslint`, `typescript-eslint` | Lint |
| `jscpd` | Duplication scan (`yarn duplication`) |

**Not used on this branch:** Vite, esbuild (as primary bundler), npm, `package-lock.json`.

## Build artefacts the metrics read

| Artefact | Produced by |
| --- | --- |
| `backend/ScholarshipCMGroups.sln` | Solution |
| `*.csproj` | MSBuild project files |
| `frontend/yarn.lock` | `yarn install` |
| `frontend/webpack.config.js` | Webpack configuration |
| `frontend/dist/` | `yarn build` |
| Coverlet Cobertura under `TestResults/` | `dotnet test --collect:"XPlat Code Coverage"` |
| `frontend/coverage/` | `yarn test:coverage` |
