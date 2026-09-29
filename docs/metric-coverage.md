# Metric coverage

This note records what was verified on this machine on 29 September 2026, against Testable at `qa` `9f8b9f41c`. A tool is listed as executed only when this workspace actually ran it. Platform registration is not the same thing as a local run.

## Executed here

| Check | Result |
| --- | --- |
| `dotnet build WestCoastFitness.sln` | Succeeded, 0 warnings. Roslyn `latest-Recommended` with warnings as errors. |
| `dotnet test --collect:"XPlat Code Coverage"` | 13 passed. Coverlet wrote `coverage.cobertura.xml` under the test project's `TestResults/`. |
| `dotnet ef migrations add InitialClub` and `migrations script` | Produced `database/schema/001-initial-schema.sql`, including plan and trainer seed rows. |
| `npm run lint` in `frontend/` | ESLint flat config, exit 0. |
| `npm test` | Vitest 5 passed. |
| `npm run build` | `tsc --build` and Vite production build succeeded. |
| `npm install` audit | 3 moderate advisories, 0 critical, 0 high. `npm audit fix --force` was not applied. |
| `jscpd` 4.0.5 on `Membership/` | 1 C# clone, 26 lines, `MembershipWindowText.cs` and `StaffMembershipWindowText.cs`. |
| `dotnet list package --vulnerable --include-transitive` | No vulnerable packages reported for the five projects. |

## Not executed here

Docker Desktop was not running, and `psql` is not installed, so the migration was not applied to a live PostgreSQL instance. `dotnet ef database update` is the command that does that once a server exists.

These Testable runners were not invoked, because their binaries are not part of this application repository and were not run from the platform worker:

| Tool | What would have to be true for it to score this repo | Why it is not claimed as run |
| --- | --- | --- |
| lizard | `.cs` files are present | Binary not executed |
| jscpd-cs / jscpd-ts | C# clone pair and TypeScript sources are present. Catalog name `jscpd-cs` is npm package `jscpd` 4.0.5 | Local `jscpd` 4.0.5 found the C# clone. The platform worker was not run |
| semgrep / semgrep-perf-static | `.cs` and `.ts` sources are present | Binary not executed. Catalog pins C# semgrep at 1.50.0 and Python semgrep at 1.70.0 |
| roslyn-analyzers, sonar-cs, security-code-scan | A `net8.0` solution that builds | The local build used the SDK analyzers. The platform's SARIF runners were not executed |
| dotnet-sca | `WestCoastFitness.sln` restores | Local `dotnet list package --vulnerable --include-transitive` reported no vulnerable packages. The platform `dotnet-sca` runner was not used |
| coverlet on the platform | Test project references `coverlet.collector` 6.0.2 and `Microsoft.NET.Test.Sdk` | Local `dotnet test --collect` did produce Cobertura. The platform runner was not used |
| vitest-coverage | `frontend/package.json`, Vitest, `vite.config.ts` coverage `cobertura` | `npm test` ran without the coverage flag |
| eslint / oxlint | `frontend/eslint.config.js` and `.ts`/`.tsx` files | Local ESLint ran. oxlint was not installed |
| gitleaks / secret scanners | Git history. No signing key or database password is committed | Scanner not executed |
| git_churn / pydriller | A git repository | This branch has history once it is committed. The runners were not executed |
| stryker-net | Mutation score | Catalog entry is `"active": false` (`catalog_master.py`, stryker-net). Not derivable on the current platform |
| cs_all_defs_uses | All-defs / all-uses | Registered stub. The runner does not implement the analyzer |
| altcover | Path coverage | Requires a successful `dotnet test` build inside the dotnet image. Not run here |
| IaC scanners (checkov, tfsec, kics) | Terraform or similar | This repo has a Dockerfile and compose file, not Terraform. Whether those scanners accept compose was not executed |

Language detection in the platform, read from `detection_service.py`:

- C# when a `.sln` or `.csproj` exists. This repo has `WestCoastFitness.sln` and five projects, `global.json`, and NuGet `PackageReference`s.
- TypeScript when `.ts` or `.tsx` files exist. They live under `frontend/src`.
- Several Node runners look for `package.json` in the scan root (`oxlint` does not). This repository keeps `package.json` in `frontend/`, the same place as `Scholarship-CMGroups-positive`. A scan whose root is the repository root will see the TypeScript files and may still fail a runner that only checks the root `package.json`.
