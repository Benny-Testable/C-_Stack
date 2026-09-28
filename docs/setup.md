# Setup

## Prerequisites

- .NET SDK 8
- Node.js 20.19 or later (see `frontend/package.json` `engines`)
- npm
- Microsoft SQL Server (local instance, LocalDB, or a remote server you control)

## 1. Clone and branch

```bash
git clone https://github.com/Mohammed-shihaf/C-_Stack.git
cd C-_Stack
git checkout Scholarship-CMGroups-positive
```

## 2. SQL Server

Create an empty database, for example `ScholarshipCMGroups`.

Apply schema and catalogue seed if you are not using EF migrations at start-up:

```bash
sqlcmd -S <server> -d ScholarshipCMGroups -i database/schema/001-initial-schema.sql
sqlcmd -S <server> -d ScholarshipCMGroups -i database/seed/001-reference-scholarships.sql
```

When `Database:ApplyMigrationsOnStartup` is `true` (Development default), `dotnet run` applies EF migrations instead of requiring the SQL files first. The SQL files remain the readable schema artefact.

## 3. Backend configuration (no secrets in git)

From `backend/src/ScholarshipCMGroups.Api`:

```bash
dotnet user-secrets set "ConnectionStrings:ScholarshipDatabase" "Server=localhost;Database=ScholarshipCMGroups;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Authentication:SigningKey" "<at-least-32-random-characters>"
dotnet user-secrets set "Bootstrap:AdministratorEmail" "admin@example.edu"
dotnet user-secrets set "Bootstrap:AdministratorPassword" "<at-least-12-characters>"
```

Equivalent environment variables:

- `ConnectionStrings__ScholarshipDatabase`
- `Authentication__SigningKey`
- `Bootstrap__AdministratorEmail`
- `Bootstrap__AdministratorPassword`

Committed `appsettings.json` leaves the connection string and signing key empty so a missing secret fails start-up instead of using a baked-in value.

CORS in Development allows `http://localhost:5173`. Add other origins under `Cors:AllowedOrigins` for a deployed UI.

## 4. Run the API

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/ScholarshipCMGroups.Api
```

- HTTPS: `https://localhost:7148`
- HTTP: `http://localhost:5148`
- Swagger (Development): `https://localhost:7148/swagger`
- Health: `GET /api/health`

## 5. Frontend

```bash
cd frontend
npm ci
copy .env.example .env.local
npm run dev
```

Leave `VITE_API_BASE_URL` empty so the Vite proxy forwards `/api` to the API.

Production build (Vite + esbuild):

```bash
npm run build
npm run preview
```

## 6. Tests

```bash
cd backend
dotnet test --collect:"XPlat Code Coverage"

cd ../frontend
npm run test
npm run test:coverage
npm run lint
```

Backend API tests host the pipeline in-process with the EF Core in-memory provider, so they do not require SQL Server. Schema tests still assert the SQL Server model (keys, indexes, delete behaviour).

## 7. First-use flow

1. Start the API and the Vite dev server.
2. Open `http://localhost:5173`.
3. Register as an applicant, or sign in with the bootstrap administrator if you configured one.
4. Browse scholarships, open a draft, submit it.
5. Sign in as administrator to move the application through review.

Administrator accounts are not created from the public register form.
