# Setup

Branch: **`Scholarship-CMgroups-Positive1`**  
Build stack: **Webpack + Yarn** (frontend), **MSBuild + NuGet** (backend)

## Prerequisites

- .NET SDK 8
- Node.js 20.19 or later
- **Yarn** (via Corepack: `corepack enable` then `corepack prepare yarn@stable --activate`)
- Microsoft SQL Server

## 1. Clone and branch

```bash
git clone https://github.com/Mohammed-shihaf/C-_Stack.git
cd C-_Stack
git checkout Scholarship-CMgroups-Positive1
```

## 2. SQL Server

Create an empty database, for example `ScholarshipCMGroups`.

Optional manual schema apply:

```bash
sqlcmd -S <server> -d ScholarshipCMGroups -i database/schema/001-initial-schema.sql
sqlcmd -S <server> -d ScholarshipCMGroups -i database/seed/001-reference-scholarships.sql
```

When `Database:ApplyMigrationsOnStartup` is `true` (Development default), `dotnet run` applies EF migrations instead.

## 3. Backend configuration

From `backend/src/ScholarshipCMGroups.Api`:

```bash
dotnet user-secrets set "ConnectionStrings:ScholarshipDatabase" "Server=localhost;Database=ScholarshipCMGroups;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Authentication:SigningKey" "<at-least-32-random-characters>"
dotnet user-secrets set "Bootstrap:AdministratorEmail" "admin@example.edu"
dotnet user-secrets set "Bootstrap:AdministratorPassword" "<at-least-12-characters>"
```

## 4. Backend build and run (MSBuild + NuGet)

```bash
cd backend
dotnet restore
dotnet build
dotnet test
dotnet run --project src/ScholarshipCMGroups.Api
```

- HTTPS: `https://localhost:7148`
- Swagger (Development): `https://localhost:7148/swagger`

## 5. Frontend build and run (Webpack + Yarn)

```bash
cd frontend
yarn install
yarn build
yarn start
```

Production output is written to `frontend/dist/`.

Optional API origin at build time:

```bash
set API_BASE_URL=
yarn build
```

Dev server: `http://localhost:5173` (proxies `/api` to the backend).

## 6. Tests

```bash
cd backend
dotnet test --collect:"XPlat Code Coverage"

cd ../frontend
yarn test
yarn test:coverage
yarn lint
```
