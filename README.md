# 9-Block Talent Matrix Platform (`9-Block-positive-Cases`)

The **9-Block (9-Box Grid)** talent assessment platform plots employees across two dimensions — **Performance** (X-axis) and **Potential** (Y-axis) — into one of nine calibrated quadrants (Block 1 "Enigma" to Block 3 "Star" and Block 7 "Risk") for succession planning and talent calibration.

This branch represents the **Golden Positive Benchmark** for White-Box static analysis and test coverage. It implements clean code, zero security vulnerabilities, low complexity, complete SQL Server schema & parameterized stored procedures, and a high-coverage test suite ($>85\%$) passing all quality gates.

---

## 1. Architecture & Tech Stack

| Component | Technology | Role & Details |
| :--- | :--- | :--- |
| **SDK Pinning** | `global.json` (.NET 8.0.100) | Root-level SDK specification for platform language detection |
| **Backend** | C# / .NET 8.0 (LTS) ASP.NET Core Web API | Clean MVC architecture, Single Responsibility, DRY design |
| **ORM / Data** | Entity Framework Core 8.0.11 & SQL Server | Relational persistence, connection resilience & retry policy |
| **SQL Database** | Microsoft SQL Server (T-SQL) | DDL schema, reference seeds, and secure parameterized procedures |
| **Frontend** | React 18.3.1 (Vite 5.4 + esbuild) | Modular UI with shared quadrant utility services |
| **Test Suite** | xUnit + Coverlet (.NET 8.0) | High statement, branch, and path coverage across all 9 quadrants |

---

## 2. White-Box Quality Gates & Metrics (Positive Case)

All 11 White-Box quality metrics pass with green status on this branch:

| # | White-Box Metric | Positive Benchmark Implementation | Status |
| :--- | :--- | :--- | :--- |
| **1** | **Code Duplication** | **0.0% Duplication (DRY)**: `EmployeeEvaluationService.cs` delegates cleanly to `NineBoxMatrixService.cs`. Frontend components import shared `quadrantUtils.js`. Zero copy-paste clones. | **PASS (0%)** |
| **2** | **Cyclomatic Complexity** | **Low CC ($\le 5$)**: Methods utilize clean C# 12 pattern matching and switch expressions instead of deeply nested if-else ladders. | **PASS** |
| **3** | **Cognitive Complexity** | **Low CogC ($\le 5$)**: Clean, linear control flow without nested loops or convoluted conditional branching. | **PASS** |
| **4** | **Lint / Code Style** | **0 Violations**: Follows Microsoft C# coding conventions and modern ESLint standard practices. | **PASS (0)** |
| **5** | **Security SAST** | **Zero Vulnerabilities**: 0 hardcoded secrets, 0 SQL injection points, parameterized queries, and defensive input validation. | **PASS (0)** |
| **6** | **Dependency SCA** | **Clean Dependencies**: Uses fully patched, modern packages (`Microsoft.EntityFrameworkCore.SqlServer 8.0.11`, `Microsoft.Data.SqlClient 5.1.6`). No vulnerable `Newtonsoft.Json 12.0.1`. | **PASS (0)** |
| **7** | **Statement & Branch Coverage**| **High Coverage ($>85\%$)**: xUnit test suite (`tests/NineBlock.Tests/`) tests all boundary values (1.0, 2.99, 3.0, 3.99, 4.0, 5.0) and all 9 quadrants. | **PASS (>85%)** |
| **8** | **Path Coverage** | Every execution path through the matrix coordinate resolution and exception handlers is covered by parameterized unit tests. | **PASS** |
| **9** | **Mutation Testing** | Tests assert exact block numbers, quadrant names, and hex codes, killing boundary mutations and logical mutants. | **PASS** |
| **10** | **Data-Flow (All-Defs / All-Uses)** | Pure functional mappings with deterministic variable lifetimes and zero uninitialized states. | **PASS** |
| **11** | **Code Churn** | Stable architecture with clean commit increments and zero anti-pattern churn. | **PASS** |

---

## 3. SQL Database Coverage (`database/`)

The repository includes complete SQL database coverage for enterprise talent calibration:

1. **`database/schema.sql`**:
   - `dbo.NineBoxQuadrants`: Reference table with performance/potential thresholds and hex color mappings.
   - `dbo.Employees`: Master record with tenure, key role indicators, and compa-ratio.
   - `dbo.ReviewCycles`: Annual and quarterly calibration review periods.
   - `dbo.Assessments`: Relational transactional assessments with foreign keys and check constraints (`1.00 <= score <= 5.00`).
   - Non-clustered analytical indexes for fast reporting.

2. **`database/seed.sql`**:
   - Seeds all 9 reference quadrants with official talent tiers and action plans.
   - Populates active calibration cycles and multi-department baseline employee evaluations.

3. **`database/procedures.sql`**:
   - **`dbo.usp_GetEmployeeAssessmentMatrix`**: 100% parameterized query filtering by cycle and department (Zero SQL injection).
   - **`dbo.usp_RecordAssessment`**: ACID-compliant transactional upsert with defensive range checks.
   - **`dbo.usp_GetDepartmentTalentDistribution`**: Analytical aggregation calculating department distribution percentages.

---

## 4. Repository Structure

```
├── global.json                         # Pin .NET SDK 8.0.100 (detectable by Whitebox platform)
├── Directory.Build.props               # Global .NET props with target net8.0 LTS
├── NineBlock.sln                       # Solution containing API and Tests projects
├── backend/
│   ├── NineBlock.csproj                # ASP.NET Core 8.0 Web API + EF Core SqlServer
│   ├── Program.cs                      # Clean DI, CORS, EF Core SQL Server + InMemory fallback
│   ├── appsettings.json                # Contains DefaultConnection for SQL Server
│   ├── Controllers/                    # RESTful endpoints (NineBoxGrid, Employees, Assessments)
│   ├── Data/                           # EF Core DbContext with model definitions
│   ├── Models/                         # Domain entities (Employee, Assessment, Quadrant)
│   └── Services/
│       ├── NineBoxMatrixService.cs     # Single Source of Truth for 9-Box logic
│       └── EmployeeEvaluationService.cs# Clean delegation (0% code duplication)
├── database/
│   ├── schema.sql                      # SQL Server DDL definitions, constraints & indexes
│   ├── seed.sql                        # Reference quadrants, active cycles & sample assessments
│   └── procedures.sql                  # Secure parameterized stored procedures
├── tests/
│   └── NineBlock.Tests/
│       ├── NineBlock.Tests.csproj      # xUnit + Coverlet test project targeting net8.0
│       ├── NineBoxMatrixServiceTests.cs# Parameterized tests for all 9 quadrants & boundaries
│       ├── EmployeeEvaluationServiceTests.cs # Delegation and consistency verification
│       └── NineBoxGridControllerTests.cs # Controller endpoint and status code verification
└── frontend/
    ├── package.json                    # React 18 + Vite 5 dependencies
    ├── vite.config.js                  # Vite configuration with React plugin
    └── src/
        ├── components/                 # NineBoxGrid, CalibrationBoard, BuildInfoBanner
        ├── views/                      # Dashboard view
        └── utils/
            └── quadrantUtils.js        # Shared quadrant and color resolution (DRY)
```

---

## 5. Verification & Running Locally

### Run Unit Tests with Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```
*Result: 43/43 tests passed (0 failures), coverage report generated.*

### Run Backend API
```bash
cd backend
dotnet run
```
API runs on `http://localhost:5253`.

### Run Frontend UI
```bash
cd frontend
npm install
npm run dev
```
UI runs on `http://localhost:3000`.
