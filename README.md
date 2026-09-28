# Scholarship CMGroups Negative Metrics

**Scholarship-CMGroups-negative-metrics** is a runnable Scholarship Management System named **Scholarship CMGroups**. It is intentionally designed for **negative metric validation** on the Testable platform.

The published branch is [Scholarship-CMGroups-negative](https://github.com/Mohammed-shihaf/C-_Stack/tree/Scholarship-CMGroups-negative).

The application builds and exercises student, scholarship, application, document, admin, and reporting flows. Selected areas are deliberately low quality so static-analysis, coverage, security, and Git metrics have something measurable to report. Do not treat this repository as a production system.

## Project overview

Reviewers and students can sign in, maintain profiles, publish scholarships, apply for awards, attach document metadata, and record approve or reject decisions. Server-rendered MVC pages and a React client talk to the same ASP.NET Core API. SQL Server scripts create the catalog and load synthetic rows.

All people, emails, and secrets in this repository are fictional. Email addresses use the `example.test` domain. Passwords and keys use the obvious placeholders `TEST_ONLY_FAKE_PASSWORD` and `TEST_ONLY_FAKE_API_KEY`.

## Technology stack

| Area | Choice |
| --- | --- |
| SCM | Git |
| Frontend | ReactJS |
| Backend | ASP.NET Core 8 / .NET 8 |
| Database | SQL Server |
| Backend build | MSBuild via `dotnet build` |
| Backend packages | NuGet |
| Frontend build | Vite |
| Vite bundler | esbuild |
| Frontend packages | npm |
| Architecture | MVC (Model, View, Controller) |

## Architecture

```text
Model
  ↓
View
  ↓
Controller
```

The backend keeps that shape and adds services and repositories behind the controllers:

```text
backend/
├── Controllers/
├── Models/
├── Views/
├── Services/
├── Repositories/
├── Data/
├── Helpers/
└── Middleware/
```

The React client lives in `frontend/` and calls `http://localhost:5080/api`.

## Prerequisites

- .NET SDK 8
- Node.js 18 or newer and npm
- SQL Server or LocalDB when you want to run the API against the scripts
- Git

`dotnet build` and `npm run build` do not require a running database. The API does.

## SQL Server setup

Create a database named `ScholarshipCMGroups`, then run the scripts in order:

```bash
sqlcmd -S localhost -d ScholarshipCMGroups -i database/schema.sql
sqlcmd -S localhost -d ScholarshipCMGroups -i database/seed.sql
sqlcmd -S localhost -d ScholarshipCMGroups -i database/procedures.sql
```

The default connection string is in `backend/appsettings.json`:

```text
Server=localhost;Database=ScholarshipCMGroups;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

Change that string if your instance name differs. Do not put a real password in the file.

### Synthetic sign-in

| Role | Email | Password |
| --- | --- | --- |
| Student | ava.nguyen@example.test | TEST_ONLY_FAKE_PASSWORD |
| Admin | riley.morgan@example.test | TEST_ONLY_FAKE_PASSWORD |

## Backend setup

```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

The MVC site and API listen on `http://localhost:5080`.

## Frontend setup

```bash
cd frontend
npm install
npm run build
npm run dev
```

The Vite dev server listens on `http://localhost:5173`.

## Build commands

Frontend:

```bash
npm install
npm run build
```

Backend:

```bash
dotnet restore
dotnet build
```

Run those commands from `frontend/` and `backend/` respectively.

## Test commands

Backend tests cover a narrow happy path and one HTTP 200 result:

```bash
dotnet test tests/backend/ScholarshipCMGroups.Tests
```

Frontend tests:

```bash
cd frontend
npm test
```

Coverage is intentionally low. Error handling, eligibility branches, rejection, document validation, admin operations, and report generation are largely untested.

The folder `tests/dependency-negative/` is an isolated vulnerability fixture. Do not install or restore it as part of the application build.

## Git commands

```bash
git status
git log --oneline
git log --stat
```

History is split so code-churn and file-frequency tools can see repeated edits to the controllers, services, and React lists.

## Known intentional negative patterns

These patterns are marked in source with `INTENTIONAL NEGATIVE TEST DATA` where a single example needs to be obvious.

- Repeated validation, logging, error envelopes, and query description blocks across controllers, services, and React lists
- High cyclomatic complexity in `CheckScholarshipEligibility`, `ProcessApplication`, `ApproveApplication`, `ValidateStudent`, and `GenerateScholarshipReport`
- Large methods and large controller classes
- Unused private methods, unused React modules, unused constants, and redundant branches
- Magic numbers, repeated string literals, a long parameter list, and a poorly named `calc` method
- `SELECT *`, repeated queries, in-memory filtering, and database access inside loops
- A test-only SQL string concatenation example
- Razor `Html.Raw` and React `dangerouslySetInnerHTML` for stored scholarship and student text
- Selected update, delete, approve, reject, and report endpoints without a session check
- Fake hardcoded values `TEST_ONLY_FAKE_API_KEY` and `TEST_ONLY_FAKE_PASSWORD`
- Thin tests with minimal assertions
- An isolated outdated-dependency manifest that is not part of the main build

## Expected metric findings

| Metric | Expected result |
| --- | --- |
| Code duplication | HIGH |
| Cyclomatic complexity | HIGH |
| Code smells | HIGH |
| Large methods | PRESENT |
| Large classes | PRESENT |
| Dead code | PRESENT |
| Security findings | PRESENT |
| SQL injection risk | PRESENT |
| XSS risk | PRESENT |
| Hardcoded test secrets | PRESENT |
| Test coverage | LOW |
| Branch coverage | LOW |
| Mutation score | LOW |
| Code churn | HIGH |

The application is functional but intentionally low quality, so these findings stay reproducible.
