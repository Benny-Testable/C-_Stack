# 9-Block Talent Matrix Platform (`9-Block-Negative-Cases`)

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0%20(LTS)-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18+-61DAFB?logo=react)](https://react.dev/)
[![Build Tool](https://img.shields.io/badge/Build%20Tool-dotnet%20%2B%20NuGet%20%7C%20Webpack%20%2B%20npm-blue)](https://webpack.js.org/)
[![Architecture](https://img.shields.io/badge/Architecture-MVC%20%2F%20Layered%20Monorepo-orange)](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview)
[![Testing Strategy](https://img.shields.io/badge/White--Box-Negative%20Metric%20Fixtures-red)](https://github.com/Mohammed-shihaf/C-_Stack/tree/9-Block-Negative-Cases)

---

## 1. Project Description

The **9-Block (9-Box Grid)** platform is an enterprise-grade talent assessment, succession planning, and performance management system. It evaluates organizational talent across two fundamental dimensions:
* **Performance (X-Axis)**: Measures current job delivery and operational results (Low, Medium, High).
* **Potential (Y-Axis)**: Measures future leadership readiness and cognitive adaptability (Low, Medium, High).

The platform maps each employee into one of nine distinct quadrants—ranging from **Block 1 (Enigma / Rough Diamond)** to **Block 3 (Star / Top Talent)** and **Block 7 (Risk / Talent Action)**—enabling objective, data-driven calibration reviews, succession pipeline visibility, and tailored employee development programs.

### Architecture Style: Model-View-Controller (MVC) Monorepo
* **View Tier (Client-Side)**: ReactJS single-page application with interactive 9-box drag-and-drop calibration boards, grid distribution graphs, and assessment modals.
* **Controller Tier (API Gateway)**: ASP.NET Core REST API controllers (`NineBoxGridController`, `EmployeeController`, `AssessmentController`) handling route dispatching, authorization, and validation.
* **Model Tier (Business Domain & Persistence)**: Domain entities (`Employee`, `Assessment`, `NineBoxQuadrant`), Entity Framework Core `DbContext`, and MS SQL Server database.

---

## 2. Platform Stack & Branch Specification

This repository branch is designated for **White-Box Negative Metric Fixtures** and boundary validation testing based on the master reference taxonomy (`Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`).

| Specification | Details |
| :--- | :--- |
| **Repository** | `https://github.com/Mohammed-shihaf/C-_Stack.git` |
| **Active Branch** | **`9-Block-Negative-Cases`** |
| **Backend Language** | C# / .NET 9.0 (LTS) |
| **Frontend Language** | JavaScript / JSX (ReactJS) |
| **Build Tool & Package Manager** | **`dotnet + NuGet & Webpack + npm`** *(Single orchestrator: `dotnet + NuGet`)* |
| **Database** | Microsoft SQL Server (Entity Framework Core) |
| **Branch Purpose / Role** | Provide reproducible code fixtures and datasets that **intentionally trigger White-Box metric scanners** (e.g., Code Duplication clones, cyclomatic complexity violations, unhandled branches). |

---

## 3. Project Usage & Execution Guide

### Prerequisites
* **.NET SDK**: `v9.0` or higher (`dotnet --version`)
* **Node.js**: `v20.x` or higher (`node --version`)
* **npm**: `v10.x` or higher (`npm --version`)
* **jscpd** (for running duplication scans): `npm install -g jscpd`

### A. Running the Backend (.NET Core Web API)
```bash
# Navigate to backend directory
cd backend/NineBlock.Api

# Restore dependencies
dotnet restore

# Build with Debug profile (for local dev) or Release profile
dotnet build -c Debug

# Launch API server with hot-reload
dotnet watch run
```
* API will launch at: `https://localhost:7001` / `http://localhost:5000`
* Swagger / OpenAPI documentation available at: `https://localhost:7001/swagger`

### B. Running the Frontend (ReactJS with Webpack)
```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Start Webpack Development Server (with HMR)
npm run dev

# Create optimized production bundle
npm run build
```
* Frontend client will launch at: `http://localhost:3000`

### C. Database Migrations (SQL Server)
```bash
# Apply EF Core migrations to SQL Server instance
dotnet ef database update --project backend/NineBlock.Data --startup-project backend/NineBlock.Api
```

---

## 4. Branch Containing Files for Metric Covering

This branch (`9-Block-Negative-Cases`) implements dedicated **tool trigger fixtures** directly mapped to the metrics defined in **`Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`**:

```
f:\9 Block\ (Branch: 9-Block-Negative-Cases)
├── CSharp_9.0/
│   └── jscpd/                          # C# Code Duplication Trigger Fixture
│       ├── EmployeeEvaluationService.cs# Target file 1 (Contains duplicate 9-block logic)
│       ├── NineBoxMatrixService.cs     # Target file 2 (Contains identical clone block)
│       ├── trigger.yaml                # Fixture metadata & expected result
│       ├── run_jscpd.bat               # Windows execution script
│       ├── run_jscpd.sh                # Bash execution script
│       └── README.md                   # Fixture documentation
├── JavaScript_React/
│   └── jscpd/                          # React Code Duplication Trigger Fixture
│       ├── CalibrationBoard.jsx        # Target component 1 (Duplicate color & grid calculation)
│       ├── NineBoxGrid.jsx             # Target component 2 (Duplicate clone component)
│       ├── trigger.yaml                # Fixture metadata & expected result
│       ├── run_jscpd.bat               # Windows execution script
│       ├── run_jscpd.sh                # Bash execution script
│       └── README.md                   # Fixture documentation
├── Platform_Stack_Matrix.xlsx          # Formatted Excel stack matrix
├── Platform_Stack_Matrix.csv           # CSV export of stack matrix
└── README.md                           # Main project documentation
```

### Detailed Metrics Mapping

| Metric Level | Taxonomy Value | Metric Coverage in this Branch |
| :--- | :--- | :--- |
| **L1 Strategy** | **White Box** | Structural static code analysis of internal logic across .NET & React. |
| **L2 Testing Type** | **Code Quality Auditing** | Evaluates code cleanliness, maintainability, and structural redundancy. |
| **L3 Technique** | **Code Duplication** | Detects duplicated or copy-pasted code blocks across distinct modules. |
| **L4 Classification** | **Defect Propagation Risk Detection** | Measures multi-point failure probability when common logic is repeated. |
| **L5 Metric** | **Multi-Point Failure Probability / Redundancy Localization** | Identifies exact token clones and file redundancy clusters. |

---

### Fixture 1: C# .NET 9.0 Code Duplication (`CSharp_9.0/jscpd/`)
* **Trigger Mechanism**: `NineBoxMatrixService.cs` and `EmployeeEvaluationService.cs` both implement an identical 30-line `ResolveNineBoxQuadrant` calculation block.
* **Scan Target**: C# source files (`.cs`)
* **Executing the Scan**:
  ```bash
  # Windows:
  cd CSharp_9.0/jscpd
  run_jscpd.bat

  # Linux / macOS / Git Bash:
  cd CSharp_9.0/jscpd
  bash run_jscpd.sh
  ```
* **Expected Outcome**:
  * Scanned Files: 2
  * Found Duplicates: **1 clone pair (30 duplicated lines, 120+ tokens)**
  * Result: **TRIGGERED (Exit code 0 or failure threshold met)**

---

### Fixture 2: JavaScript / ReactJS Code Duplication (`JavaScript_React/jscpd/`)
* **Trigger Mechanism**: `NineBoxGrid.jsx` and `CalibrationBoard.jsx` both implement identical helper functions for coordinate normalization (`getQuadrantCoordinates`), color mapping (`getQuadrantColor`), and score badge labeling.
* **Scan Target**: JSX source files (`.jsx`)
* **Executing the Scan**:
  ```bash
  # Windows:
  cd JavaScript_React/jscpd
  run_jscpd.bat

  # Linux / macOS / Git Bash:
  cd JavaScript_React/jscpd
  bash run_jscpd.sh
  ```
* **Expected Outcome**:
  * Scanned Files: 2
  * Found Duplicates: **1 clone pair (28 duplicated lines, 110+ tokens)**
  * Result: **TRIGGERED (Exit code 0 or failure threshold met)**

---

## 5. Upcoming Metric Coverage Fixtures (Roadmap)

The following negative fixtures are scheduled for implementation on this branch to cover all 7 White-Box domains in `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`:

1. **Cyclomatic & Cognitive Complexity (`complexity/`)**: Deeply nested 5-tier quadrant resolution switches to trigger complexity limits ($>15$).
2. **Control Flow / Branch Coverage Gaps (`coverage-gap/`)**: Untested conditional branches in score validation to trigger branch coverage alerts.
3. **Static Security Vulnerability (`sast/`)**: Intentional raw SQL parameter concatenation and dummy secret patterns for SAST scanners.
4. **Lint / Rule Violations (`lint-rules/`)**: Unused variables, non-standard naming, and unawaited async promises triggering ESLint and `dotnet format`.