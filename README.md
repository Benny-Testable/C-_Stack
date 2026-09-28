# 9-Block Talent Matrix Platform (`9-Block-Negative-Cases-Rollup`)

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20(LTS)-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18+-61DAFB?logo=react)](https://react.dev/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server%20%7C%20EF%20Core-CC292B?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Build Tool](https://img.shields.io/badge/Build%20Tool-dotnet%20%2B%20NuGet%20%7C%20Rollup%20%2B%20npm-orange)](https://rollupjs.org/)
[![Architecture](https://img.shields.io/badge/Architecture-MVC%20%2F%20Layered%20Monorepo-orange)](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview)
[![Testing Strategy](https://img.shields.io/badge/White--Box-Multi--Metric%20Negative%20Testbed-red)](https://github.com/Mohammed-shihaf/C-_Stack/tree/9-Block-Negative-Cases-Rollup)

---

## 1. Project Description

The **9-Block (9-Box Grid)** platform is an enterprise-grade talent assessment, succession planning, and employee performance management system. It plots employees across two core axes:
* **Performance (X-Axis)**: Measures current job execution and delivery (Low, Medium, High).
* **Potential (Y-Axis)**: Measures future leadership capability and adaptability (Low, Medium, High).

The platform maps each employee into one of nine distinct quadrants—ranging from **Block 1 (Enigma / Rough Diamond)** to **Block 3 (Star / Top Talent)** and **Block 7 (Risk / Talent Action)**—enabling objective, data-driven calibration reviews, succession pipeline visibility, and tailored employee development programs.

---

## 2. Complete MVC Project Architecture

The solution is structured as an **enterprise Model-View-Controller (MVC)** monorepo:

```
9 Block/
├── global.json                            # .NET SDK Version Pin (8.0.100)
├── database/                              # SQL SERVER DATABASE LAYER (DDL, DML & Stored Procedures)
│   ├── schema.sql                         # Tables, Constraints, and Indexes DDL
│   ├── seed.sql                           # Quadrant, Employee & Assessment Data DML
│   └── procedures.sql                     # Stored Procedures & Dynamic SQL SAST Fixture
│
├── backend/                               # ASP.NET Core (.NET 8.0 LTS) Web API
│   ├── NineBlock.csproj                   # Project File (.NET 8.0 + SQL Server + SCA Dep)
│   ├── Controllers/                       # CONTROLLER LAYER (API Endpoints)
│   │   ├── NineBoxGridController.cs       # Grid matrix distribution & calculations
│   │   ├── EmployeeController.cs          # Employee profiles & departments
│   │   └── AssessmentController.cs        # [SAST FIXTURE] Input taint & hardcoded secret
│   ├── Models/                            # MODEL LAYER (Domain Entities & DTOs)
│   │   ├── Employee.cs                    # Employee entity
│   │   ├── Assessment.cs                  # Assessment ratings & coordinates
│   │   ├── NineBoxQuadrant.cs             # 9 Quadrant definitions & metadata
│   │   └── ReviewCycle.cs                 # Quarterly/Annual review cycles
│   ├── Services/                          # BUSINESS LOGIC & CALCULATION LAYER
│   │   ├── NineBoxMatrixService.cs        # [CC, COG, LINT & DUPLICATION FIXTURES]
│   │   └── EmployeeEvaluationService.cs   # [DUPLICATION, DATA-FLOW, MUTATION & COVERAGE FIXTURES]
│   ├── Data/                              # DATA ACCESS & PERSISTENCE (EF Core & SQL Server)
│   │   └── NineBlockDbContext.cs          # DbContext & quadrant seed data
│   ├── Program.cs                         # DI Container, SQL Server Provider & Pipeline setup
│   └── appsettings.json                   # SQL Server Connection String & Config
│
├── frontend/                              # ReactJS Single Page Application (Rollup Bundler)
│   ├── public/
│   │   └── index.html                     # HTML Template
│   ├── src/
│   │   ├── components/                    # VIEW LAYER (UI Components)
│   │   │   ├── NineBoxGrid.jsx            # Interactive 9-Box Matrix Grid Board
│   │   │   └── CalibrationBoard.jsx       # [DUPLICATION FIXTURE] Duplicated UI coordinate logic
│   │   ├── views/                         # PAGE VIEWS
│   │   │   └── Dashboard.jsx              # [FRONTEND CC & LINT FIXTURES]
│   │   ├── services/
│   │   │   └── api.js                     # API Client for Backend Communication
│   │   ├── App.jsx                        # Main Application Root
│   │   ├── index.js                       # React DOM Entrypoint
│   │   └── index.css                      # Modern Clean Styling
│   ├── rollup.config.mjs                  # Rollup 4.x Bundler Configuration
│   ├── .babelrc                           # Babel React Presets
│   └── package.json                       # Dependencies (includes axios 0.21.1 SCA fixture)
│
├── tests/                                 # UNIT TESTS (Deliberate Coverage Gap Suite)
│   └── NineBlock.Tests/
│       ├── NineBlock.Tests.csproj         # xUnit + Coverlet (.NET 8.0)
│       └── NineBoxMatrixTests.cs          # Single happy-path test (leaves < 30% coverage)
└── README.md                              # This Documentation
```

---

## 3. Platform Stack & Branch Specification

This branch (`9-Block-Negative-Cases-Rollup`) implements the full application with **Rollup + npm** bundling while intentionally incorporating **White-Box metric trigger cases** across the complete testing taxonomy based on `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`:

| Specification | Details |
| :--- | :--- |
| **Repository** | `https://github.com/Mohammed-shihaf/C-_Stack.git` |
| **Active Branch** | **`9-Block-Negative-Cases-Rollup`** |
| **Backend Language** | C# / .NET 8.0 (LTS) |
| **Frontend Language** | JavaScript / JSX (ReactJS) |
| **Build Tool & Package Manager** | **`dotnet + NuGet & Rollup + npm`** *(Unified: `dotnet + NuGet`)* |
| **Database** | **Microsoft SQL Server 2022 / Entity Framework Core 8.0** |
| **Architecture Pattern** | **MVC (Model-View-Controller)** |

### Branch Stack Matrix
| Branch | Primary Pair | C# / Language Build Tool | Package Manager | Project Structure | Database |
|---|---|---|---|---|---|
| `9-Block-Negative-Cases-Rollup` | C# (.NET 8.0) | dotnet CLI | NuGet | **MVC** | **SQL Server** |

| Branch | Language | Build Tool & Package Manager | Architecture Style | Description / Role |
| :--- | :--- | :--- | :--- | :--- |
| `9-Block-Negative-Cases-Rollup` | C# / .NET 8.0 (LTS) & JavaScript (ReactJS) | dotnet + NuGet & Rollup + npm | MVC | C# ASP.NET Core Web API + SQL Server (EF Core) + ReactJS with Rollup bundler. 9-Box talent matrix evaluation platform with complete 11-category White-Box negative metric fixtures and full SQL DB coverage. |

---

## 4. Branch Containing Files for Metric Covering

This branch embeds **concrete White-Box negative metric targets** mapped directly to all 11 techniques in `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`:

| # | L2 Testing Type | L3 Technique | L5 Target Metric | Concrete Application Files Covering the Metric | Triggered Tool / Scanner |
|:---:|:---|:---|:---|:---|:---|
| **1** | **Code Quality Auditing** | **Code Duplication** | Multi-Point Failure Probability / Redundancy Localization | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) & [`backend/Services/EmployeeEvaluationService.cs`](backend/Services/EmployeeEvaluationService.cs)<br>[`frontend/src/components/NineBoxGrid.jsx`](frontend/src/components/NineBoxGrid.jsx) & [`frontend/src/components/CalibrationBoard.jsx`](frontend/src/components/CalibrationBoard.jsx) | `jscpd` / SonarQube CPD |
| **2** | **Structural Analysis** | **Cyclomatic Complexity** | Execution Path Integrity ($\text{CC} > 15$) | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) (`CalculateComplexTalentRiskScore`, $\text{CC}=19$)<br>[`frontend/src/views/Dashboard.jsx`](frontend/src/views/Dashboard.jsx) (`calculateClientAttritionRisk`) | `Lizard` / `complexipy` / ESLint `complexity` |
| **3** | **Readability / Maintainability** | **Cognitive Complexity** | Technical Debt Impact ($\text{CogC} > 15$) | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) (`ComputeDepartmentCognitiveCalibration`, 4-level nesting) | SonarQube Cognitive Complexity / Roslyn |
| **4** | **Static Code Analysis** | **Lint / Style Violations** | Violation Density & Resource Waste | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) (`_unusedAuditCacheKey`, `temp_debug_log_str`, dead code)<br>[`frontend/src/views/Dashboard.jsx`](frontend/src/views/Dashboard.jsx) (`UNUSED_CALIBRATION_TOKEN`) | Roslyn Analyzers / `dotnet format` / `ESLint` |
| **5** | **Security White-box Testing** | **Static Vulnerabilities (SAST)** | Exploit Surface & Sensitive Tracking | [`backend/Controllers/AssessmentController.cs`](backend/Controllers/AssessmentController.cs) (`ExportDepartmentAssessmentsRaw` CWE-89 raw SQL taint & `FallbackAdminAuthSecret`)<br>[`database/procedures.sql`](database/procedures.sql) (`sp_SearchEmployeeAssessmentsRaw` dynamic SQL injection) | `Semgrep` / `Gitleaks` / `sqlfluff` / Roslyn Security Analyzers |
| **6** | **Security White-box Testing** | **Dependency Risk (SCA)** | Vulnerability Dependency / Known CVE Count | [`backend/NineBlock.csproj`](backend/NineBlock.csproj) (`Newtonsoft.Json 12.0.1` CVE-2024-21907)<br>[`frontend/package.json`](frontend/package.json) (`axios 0.21.1` CVE-2020-28168) | `dotnet list package --vulnerable` / `npm audit` / Snyk / Trivy |
| **7** | **Control Flow Testing** | **Statement & Branch Coverage** | Decision Coverage Gap Analysis (< 30%) | [`backend/Services/EmployeeEvaluationService.cs`](backend/Services/EmployeeEvaluationService.cs) (`UncoveredBranchEvaluation`) & [`tests/NineBlock.Tests/NineBoxMatrixTests.cs`](tests/NineBlock.Tests/NineBoxMatrixTests.cs) (covers only 1 case) | `coverlet` / `dotnet test` / Coverage.py |
| **8** | **Control Flow Testing** | **Path Coverage** | Ghost Code / Unreachable Path Gap | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) (nested condition paths in `CalculateComplexTalentRiskScore`) | OpenCover / Roslyn Control Flow |
| **9** | **Mutation Testing** | **Mutation Score** | Boundary Mutant Survival / Logic Error Sensitivity | [`backend/Services/EmployeeEvaluationService.cs`](backend/Services/EmployeeEvaluationService.cs) (`CalculatePerformanceBonusMultiplier` boundary `<` vs `<=`) | `Stryker.NET` / Stryker-JS |
| **10** | **Data Flow Testing** | **All-Def / All-Uses Coverage** | Dead Data / DU-Path Anomaly Identification | [`backend/Services/EmployeeEvaluationService.cs`](backend/Services/EmployeeEvaluationService.cs) (`CalculateDataFlowAnomalies` Def-Def dead store & unread definition) | SpotBugs / Roslyn DataFlowAnalysis |
| **11** | **Development Process** | **Code Churn & Diff Coverage** | Hotspot Risk & New Logic Proofing | Frequent commit modifications in [`NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) & 0% test coverage on PR diffs | Git Log Churn Analyzers / Diff-Cover |

---

### Concrete Verification Commands for White-Box Tools:

1. **SQL Database Linting & DDL Validation (`sqlfluff`)**:
   ```bash
   sqlfluff lint database/schema.sql --dialect tsql
   sqlfluff lint database/procedures.sql --dialect tsql
   ```

2. **Code Duplication (`jscpd`)**:
   ```bash
   npx jscpd backend/Services/
   npx jscpd frontend/src/components/
   ```

3. **Cyclomatic Complexity (`Lizard` / `complexipy`)**:
   ```bash
   python -m lizard backend/Services/
   python -m lizard frontend/src/views/
   ```

4. **Cognitive Complexity & Lint (`dotnet format` / `ESLint`)**:
   ```bash
   dotnet format whitespace --verify-no-changes
   npx eslint frontend/src/
   ```

5. **Security Vulnerabilities (SAST & SQL Injection Taint)**:
   ```bash
   npx semgrep --config=auto backend/Controllers/
   npx semgrep --config=auto database/
   ```

6. **Dependency Risk (SCA CVE Scan)**:
   ```bash
   dotnet list backend/NineBlock.csproj package --vulnerable
   cd frontend && npm audit
   ```

7. **Statement & Branch Coverage (< 30% Coverage Gap)**:
   ```bash
   dotnet test tests/NineBlock.Tests/NineBlock.Tests.csproj --collect:"XPlat Code Coverage"
   ```

8. **Mutation Testing (`Stryker.NET`)**:
   ```bash
   dotnet stryker --project-file=backend/NineBlock.csproj
   ```

---

## 5. Project Usage Guide

### Prerequisites
* **.NET SDK**: `v8.0` LTS (`8.0.100` via `global.json`)
* **SQL Server**: 2019 / 2022 or LocalDB (`(localdb)\mssqllocaldb`)
* **Node.js**: `v20.x` or higher (`node --version`)
* **npm**: `v10.x` or higher (`npm --version`)

### A. Initializing SQL Server Database
```bash
# Execute DDL, DML Seed, and Stored Procedures scripts
sqlcmd -S "(localdb)\mssqllocaldb" -i database/schema.sql
sqlcmd -S "(localdb)\mssqllocaldb" -i database/seed.sql
sqlcmd -S "(localdb)\mssqllocaldb" -i database/procedures.sql
```

### B. Running the Backend (.NET Core Web API)
```bash
# Navigate to backend directory
cd backend

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Launch API server
dotnet run
```
* Backend API runs on: `http://localhost:5000` (or `https://localhost:7001`)

### C. Running the Test Suite (Verifying Coverage Gaps)
```bash
cd tests/NineBlock.Tests
dotnet test
```

### D. Running the Frontend (ReactJS with Rollup)
```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Start Rollup in watch mode
npm run dev

# Create optimized production bundle
npm run build
```
* Bundled assets output to `frontend/dist/`.