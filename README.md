# 9-Block Talent Matrix Platform (`9-Block-Negative-Cases`)

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20(LTS)-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18+-61DAFB?logo=react)](https://react.dev/)
[![Build Tool](https://img.shields.io/badge/Build%20Tool-dotnet%20%2B%20NuGet%20%7C%20Webpack%20%2B%20npm-blue)](https://webpack.js.org/)
[![Architecture](https://img.shields.io/badge/Architecture-MVC%20%2F%20Layered%20Monorepo-orange)](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview)
[![Testing Strategy](https://img.shields.io/badge/White--Box-Negative%20Metric%20Fixtures-red)](https://github.com/Mohammed-shihaf/C-_Stack/tree/9-Block-Negative-Cases)

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
├── backend/                               # ASP.NET Core (.NET 8.0 LTS) Web API
│   ├── NineBlock.csproj                   # Project File (.NET 8.0)
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
│   │   └── EmployeeEvaluationService.cs   # [DUPLICATION FIXTURE] Cloned calculation logic
│   ├── Data/                              # DATA ACCESS & PERSISTENCE (EF Core)
│   │   └── NineBlockDbContext.cs          # DbContext & quadrant seed data
│   ├── Program.cs                         # DI Container, CORS & Pipeline setup
│   └── appsettings.json                   # Configuration
│
├── frontend/                              # ReactJS Single Page Application (Webpack)
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
│   ├── webpack.config.js                  # Webpack Build Configuration
│   ├── .babelrc                           # Babel React Presets
│   └── package.json                       # Frontend Dependencies & Scripts
└── README.md                              # This Documentation
```

---

## 3. Platform Stack & Branch Specification

This branch (`9-Block-Negative-Cases`) implements the full application with **Webpack + npm** bundling while intentionally incorporating **White-Box metric trigger cases** across 5 distinct testing techniques based on `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`:

| Specification | Details |
| :--- | :--- |
| **Repository** | `https://github.com/Mohammed-shihaf/C-_Stack.git` |
| **Active Branch** | **`9-Block-Negative-Cases`** |
| **Backend Language** | C# / .NET 8.0 (LTS) |
| **Frontend Language** | JavaScript / JSX (ReactJS) |
| **Build Tool & Package Manager** | **`dotnet + NuGet & Webpack + npm`** *(Unified: `dotnet + NuGet`)* |
| **Database** | Microsoft SQL Server / Entity Framework Core |
| **Architecture Pattern** | **MVC (Model-View-Controller)** |

### Branch Stack Matrix
| Branch | Primary Pair | C# / Language Build Tool | Package Manager | Project Structure |
|---|---|---|---|---|
| `9-Block-Negative-Cases` | C# (.NET 8.0) | dotnet CLI | NuGet | **MVC** |

| Branch | Language | Build Tool & Package Manager | Architecture Style | Description / Role |
| :--- | :--- | :--- | :--- | :--- |
| `9-Block-Negative-Cases` | C# / .NET 8.0 (LTS) & JavaScript (ReactJS) | dotnet + NuGet & Webpack + npm | MVC | C# ASP.NET Core Web API + SQL Server (EF Core) + ReactJS single solution. 9-Box talent matrix evaluation platform with multi-category White-Box negative metric fixtures. |

---

## 4. Branch Containing Files for Metric Covering

This branch embeds **real-world White-Box metric targets** mapped directly to `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`:

| # | L2 Testing Type | L3 Technique | L4 Classification | L5 Metric | Application Files Covering the Metric |
|---|---|---|---|---|---|
| **1** | **Code Quality Auditing** | **Code Duplication** | Defect Propagation Risk | Multi-Point Failure Probability / Redundancy Localization | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) & [`backend/Services/EmployeeEvaluationService.cs`](backend/Services/EmployeeEvaluationService.cs)<br>[`frontend/src/components/NineBoxGrid.jsx`](frontend/src/components/NineBoxGrid.jsx) & [`frontend/src/components/CalibrationBoard.jsx`](frontend/src/components/CalibrationBoard.jsx) |
| **2** | **Structural Analysis** | **Cyclomatic Complexity** | Static Analysis Metric | Execution Path Integrity (CC > 15) | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) (`CalculateComplexTalentRiskScore`)<br>[`frontend/src/views/Dashboard.jsx`](frontend/src/views/Dashboard.jsx) (`calculateClientAttritionRisk`) |
| **3** | **Readability / Maintainability** | **Cognitive Complexity** | Maintainability Evaluation | Technical Debt Impact (CogC > 15) | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) (`ComputeDepartmentCognitiveCalibration`) |
| **4** | **Static Code Analysis** | **Lint / Rule Violations** | Unused Variable / Naming Rules | Violation Density / Resource Waste | [`backend/Services/NineBoxMatrixService.cs`](backend/Services/NineBoxMatrixService.cs) (`_unusedAuditCacheKey`, `temp_debug_log_str`, dead code)<br>[`frontend/src/views/Dashboard.jsx`](frontend/src/views/Dashboard.jsx) (`UNUSED_CALIBRATION_TOKEN`) |
| **5** | **Security White-box Testing** | **Static Vulnerabilities (SAST)** | Input Validation / Credentials | Exploit Surface / Sensitive Tracking | [`backend/Controllers/AssessmentController.cs`](backend/Controllers/AssessmentController.cs) (`ExportDepartmentAssessmentsRaw` CWE-89 & `FallbackAdminAuthSecret`) |

---

### Concrete Verification Commands for White-Box Tools:

1. **Code Duplication (`jscpd`)**:
   ```bash
   npx jscpd backend/Services/
   npx jscpd frontend/src/components/
   ```

2. **Cyclomatic Complexity (`Lizard` / `complexipy`)**:
   ```bash
   python -m lizard backend/Services/
   python -m lizard frontend/src/views/
   ```

3. **Cognitive Complexity & Lint (`dotnet format` / `ESLint`)**:
   ```bash
   dotnet format whitespace --verify-no-changes
   npx eslint frontend/src/
   ```

4. **Security Vulnerabilities (`Semgrep` / `Roslyn Security`)**:
   ```bash
   npx semgrep --config=auto backend/Controllers/
   ```

5. **Boundary & Negative Value Validation**:
   * File: [`backend/Controllers/AssessmentController.cs`](backend/Controllers/AssessmentController.cs)
   * Rejects out-of-bounds scores ($< 1.0$ or $> 5.0$) with `400 Bad Request`.

---

## 5. Project Usage Guide

### Prerequisites
* **.NET SDK**: `v8.0` LTS (`8.0.100` via `global.json`)
* **Node.js**: `v20.x` or higher (`node --version`)
* **npm**: `v10.x` or higher (`npm --version`)

### A. Running the Backend (.NET Core Web API)
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
* API Endpoints:
  * `GET /api/nineboxgrid/quadrants` — Retrieve all 9 quadrant configurations
  * `GET /api/nineboxgrid/distribution` — Retrieve employee quadrant distribution
  * `POST /api/nineboxgrid/calculate` — Calculate block from performance & potential scores
  * `GET /api/employee` — List employees
  * `POST /api/assessment` — Submit an employee assessment
  * `GET /api/assessment/export-raw?department=Engineering` — Seeded SAST raw query test

### B. Running the Frontend (ReactJS with Webpack)
```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Start Webpack Development Server (with HMR)
npm run dev

# Create optimized production build
npm run build
```
* Frontend client launches on: `http://localhost:3000`