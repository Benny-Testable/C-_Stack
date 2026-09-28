# Negative Metric Traceability — Scholarship CMGroups

Branch: `Scholarship-CMGroups-negative-1` · Repository: `Mohammed-shihaf/C-_Stack`

This branch is the **negative validation baseline**. It deliberately fails two White Box metrics from
`Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`, using the tools listed in `Whitebox_Tools_Registry 1.xlsx`:

1. **Statement Coverage** (Control Flow Testing, White Box rows 56–60)
2. **Lint / Rule Violations** (Static Code Analysis, White Box rows 27–38)

Every failure is deterministic (fixed inputs, pinned tool versions, no random or time-based logic),
reproducible (`scripts/verify-negative-baseline.sh`) and traceable to a file, line and rule below.
The application itself stays valid. Fixture code returns correct values, and the lint fixtures are
not wired into the running app.

## Expected results

| Gate | Command | Expected | Measured |
| --- | --- | --- | --- |
| Backend build | `dotnet build ScholarshipCMGroups.sln` | PASS | PASS (analyzer findings are warnings) |
| Backend tests | `dotnet test ScholarshipCMGroups.sln` | PASS | PASS, 4/4 |
| Backend statement coverage | `dotnet test ScholarshipCMGroups.sln -p:CollectCoverage=true` | **FAIL** | 9.17% line/statement (340/3707) < 80% |
| Backend lint (Roslyn) | `dotnet build backend/ScholarshipCMGroups.csproj --no-incremental -p:LintGate=true` | **FAIL** | 61 errors |
| Frontend install | `pnpm install --frozen-lockfile` | PASS | PASS |
| Frontend build | `pnpm build` (Vite + esbuild) | PASS | PASS |
| Frontend tests | `pnpm test` (Mocha) | PASS | PASS, 5/5 |
| Frontend statement coverage | `pnpm coverage` (nyc) | **FAIL** | 11.53% statements (15/130) < 80% |
| Frontend lint (ESLint) | `pnpm lint` | **FAIL** | 16 errors, 1 warning |

`bash scripts/verify-negative-baseline.sh` runs all nine gates. It exits 0 only when every outcome
matches the Expected column.

**Documented exception:** no test fails on purpose. The coverage gates fail because the coverage
tool's threshold check returns a non-zero exit code *after* all tests have passed. The Coverlet
threshold is only active when `CollectCoverage=true`, so a plain `dotnet test` still passes.

---

## 1. Statement Coverage

### Configuration

| Side | Tool (registry) | Config file | Threshold |
| --- | --- | --- | --- |
| C# | `coverlet` — Coverlet 6.0.0 (`coverlet.msbuild` + `coverlet.collector`) | `tests/backend/ScholarshipCMGroups.Tests/ScholarshipCMGroups.Tests.csproj` | `Threshold=80`, `ThresholdType=line`, `ThresholdStat=total` |
| JS | `nyc-mocha` — nyc 17.1.0 + Mocha 10.8.2 (ESM via `@istanbuljs/esm-loader-hook`) | `frontend/.nycrc.json`, `frontend/.mocharc.json` | `statements: 80` (plus lines 80, functions 80, branches 70) |

Coverlet has no separate "statement" threshold type. It instruments sequence points, one per
executable statement, and reports them as line coverage, so `ThresholdType=line` is the statement
threshold. Reports go to `reports/coverage/backend/` (cobertura, json, opencover) and
`frontend/coverage/` (lcov, cobertura, json-summary).

### Scenarios

| ID | File | Test that covers it | Covered path | Intentionally uncovered | Measured |
| --- | --- | --- | --- | --- | --- |
| CS-NET-01 | `backend/Services/AwardEstimateService.cs` | `tests/backend/ScholarshipCMGroups.Tests/AwardEstimateServiceTests.cs` | `Estimate` → not-eligible early return | lines 11–14, 23–24, 34–46, 50–53, 56–66 (`IsEligible` true path, award maths, `Describe`, `Rank`) | 25% of class lines |
| CS-NET-02 | Pre-existing services and controllers (`ScholarshipService`, `ApplicationService`, `ReportService`, `AdminController`, …) | 3 inherited happy-path tests | `ScholarshipController.List`, `StudentService.Register` / `ValidateStudent` | error handling, eligibility branches, approval / rejection, documents, admin, reports | module total 9.17% |
| CS-JS-01 | `frontend/src/utils/awardCalculator.js` | `tests/frontend/awardCalculator.test.js` | `estimateAward(null, …)` → returns 0 | lines 15, 22–45 | 33.33% (8/24) |
| CS-JS-02 | `frontend/src/services/*.js` | none | none | every axios API wrapper | 0% (0/90) |
| CS-JS-03 | `frontend/src/utils/unusedLegacyScore.js` | none (dead code) | none | whole file | 0% (0/6) |
| CS-JS-04 | `frontend/src/models/entities.js` | `tests/frontend/entities.test.js` | `emptyStudent` | `emptyScholarship` and the other factories | 25% (1/4) |

nyc runs with `all: true`, so JS files that no test imports still count toward the total. JSX
components are not instrumented, because nyc cannot parse JSX without a Babel transform.
`src/lint-negative/**` is excluded so the lint fixture doesn't skew the coverage number.

Metric rows this data triggers (Testable_Strategy_Metrics_Mapping_v0.2, White Box):

| Row | L4 Classification | L5 Metric | Threshold | Outcome |
| --- | --- | --- | --- | --- |
| 56 | Unit Testing Support | Test Case Granularity | ≥ 90% of functions have a unit test | FAIL (C# methods 22.08%, JS functions 16%) |
| 57 | Dead Code Detection | Unreachable Logic Identification | < 2% dead code | FAIL (`unusedLegacyScore.js`, `UnusedLegacyCalculator.cs`) |
| 58 | Test Completeness Evaluation | Coverage Gap Analysis | < 20% uncovered statements | FAIL (~90% uncovered) |
| 59 | Basic Logic Validation | Surface-Level Correctness | 100% of executed statements run without exception | PASS (all tests pass) |
| 60 | Code Execution Verification | Statement Coverage % | ≥ 80% | **FAIL** (C# 9.17%, JS 11.53%) |

---

## 2. Lint / Rule Violations

### Configuration

| Side | Tool (registry) | Config | Gate |
| --- | --- | --- | --- |
| C# | `roslyn-analyzers` Microsoft.CodeAnalysis.NetAnalyzers 8.0.0 + `sonar-cs` SonarAnalyzer.CSharp 9.32.0.97167 | `Directory.Analyzers.props`, `.editorconfig` (curated rule set), `Directory.Build.targets` | `-p:LintGate=true` turns warnings into errors. SARIF output: `reports/lint/ScholarshipCMGroups.sarif` |
| JS | `eslint` ESLint 8.47.0 + `eslint-sonarjs` 0.25.1 + `eslint-security` 3.0.1 | `frontend/.eslintrc.cjs` | `eslint --max-warnings 0`. JSON output: `pnpm lint:report` → `frontend/reports/eslint.json` |

`.editorconfig` switches off all analyzer defaults (`dotnet_analyzer_diagnostic.severity = none`) and
turns on only these 12 rules, as warnings: S1481, CA1707, S100, CA1805, S1066, S125, S3776, S107,
CA2201, S1192, S4144, plus the compiler's own CS0219. A fixed rule list keeps the finding count the
same from run to run.

### C# fixture — `backend/LintNegative/ApplicantScoringRules.cs` (15 findings)

| ID | Line:Col | Rule | Technique (White Box row) | Finding |
| --- | --- | --- | --- | --- |
| LINT-CS-01 | 10:29 | CA1805 | Code Style Rule Validation (30) | `reviewCount` explicitly initialised to its default value |
| LINT-CS-02 | 12:16 | CA1707 | Naming Convention Validation (29) | Underscore in method name `Score_Applicant` |
| LINT-CS-03 | 16:13 | CS0219 | Unused Variable Detection (28) | `unusedWeight` assigned but never used |
| LINT-CS-04 | 16:13 | S1481 | Unused Variable Detection (28) | Unused local `unusedWeight` |
| LINT-CS-05 | 19:13 | S1066 | Code Style Rule Validation (30) | Mergeable nested `if` |
| LINT-CS-06 | 27:16 | S4144 | Multiple Violations Detection (33) | `CopyOfScoreApplicant` identical to `Score_Applicant` |
| LINT-CS-07 | 31:13 | CS0219 | Unused Variable Detection (28) | `unusedWeight` assigned but never used (copy) |
| LINT-CS-08 | 31:13 | S1481 | Unused Variable Detection (28) | Unused local `unusedWeight` (copy) |
| LINT-CS-09 | 34:13 | S1066 | Code Style Rule Validation (30) | Mergeable nested `if` (copy) |
| LINT-CS-10 | 42:19 | S100 | Naming Convention Validation (29) | Method `reviewLabel` not PascalCase |
| LINT-CS-11 | 45:9 | S125 | Code Style Rule Validation (30) | Commented-out code |
| LINT-CS-12 | 48:20 | S1192 | Multiple Violations Detection (33) | Literal `"Pending review"` repeated 4 times |
| LINT-CS-13 | 62:19 | CA2201 | Rule Severity Classification (32) | Throws the reserved type `System.Exception` |
| LINT-CS-14 | 67:19 | S3776 | Complexity Rule Detection (31) | `ClassifyApplicant` cognitive complexity 20 > 15 |
| LINT-CS-15 | 67:36 | S107 | Complexity Rule Detection (31) | `ClassifyApplicant` has 9 parameters > 7 |

### C# inherited findings (46, same rule set)

These come from the negative data already on `Scholarship-CMGroups-negative` and are listed in the SARIF file:

| Rule | Count | Where |
| --- | --- | --- |
| S1192 duplicated literals | 26 | Controllers (`"anonymous"`, entity names), Repositories (`"state"`), Services (status strings) |
| S107 too many parameters | 11 | `AdminController.calc` (10), `ScholarshipController` (12, 8), plus 8-parameter methods in controllers and services |
| S3776 cognitive complexity | 7 | `ScholarshipService` (113), `StudentService` (61), `ReportService` (53), `ApplicationService` (45, 28, 19), `ScholarshipRepository` (17) |
| S100 naming | 1 | `AdminController.calc` (line 194) |
| S1066 mergeable if | 1 | `ScholarshipService.cs` line 267 |

Total: 15 + 46 = **61**, the same as the lint gate's error count.

### JS fixture — `frontend/src/lint-negative/applicantScoring.js` (16 errors, 1 warning)

The rest of `frontend/src` is lint-clean, so every ESLint finding comes from this file.

| ID | Line:Col | Rule | Severity | Technique (White Box row) | Finding |
| --- | --- | --- | --- | --- | --- |
| LINT-JS-01 | 7:21 | sonarjs/no-duplicate-string | error | Multiple Violations Detection (33) | `'Pending review'` repeated 4 times |
| LINT-JS-02 | 10:3 | no-var | error | Code Style Rule Validation (30) | `var total` |
| LINT-JS-03 | 11:7 | prefer-const | error | Code Style Rule Validation (30) | `bonus` never reassigned |
| LINT-JS-04 | 12:9 | no-unused-vars | error | Unused Variable Detection (28) | `unusedWeight` unused |
| LINT-JS-05 | 13:21 | eqeqeq | error | Rule Severity Classification (32) | `==` instead of `===` |
| LINT-JS-06 | 19:17 | camelcase | error | Naming Convention Validation (29) | `applicant_label` |
| LINT-JS-07 | 20:3 | no-console | warning | Rule Detection Test (27) | `console.log` |
| LINT-JS-08 | 23:10 | no-else-return | error | Code Style Rule Validation (30) | `else` after `return` |
| LINT-JS-09 | 33:19 | security/detect-non-literal-regexp | error | Rule Severity Classification (32) | `new RegExp(pattern)` with a non-literal pattern |
| LINT-JS-10 | 37:8 | max-params | error | Complexity Rule Detection (31) | `classifyApplicant` has 6 params > 5 |
| LINT-JS-11 | 37:17 | sonarjs/cognitive-complexity | error | Complexity Rule Detection (31) | Cognitive complexity 20 > 15 |
| LINT-JS-12 | 42:9 | max-depth | error | Complexity Rule Detection (31) | Nesting depth 4 > 3 |
| LINT-JS-13 | 63:17 | sonarjs/no-identical-functions | error | Multiple Violations Detection (33) | `copyOfScoreApplicant` identical to `scoreApplicant` |
| LINT-JS-14 | 64:3 | no-var | error | Code Style Rule Validation (30) | `var total` (copy) |
| LINT-JS-15 | 65:7 | prefer-const | error | Code Style Rule Validation (30) | `bonus` (copy) |
| LINT-JS-16 | 66:9 | no-unused-vars | error | Unused Variable Detection (28) | `unusedWeight` (copy) |
| LINT-JS-17 | 67:21 | eqeqeq | error | Rule Severity Classification (32) | `==` (copy) |

### Metric rows this data triggers (White Box 27–38)

| Row | L4 Classification | Threshold | Outcome |
| --- | --- | --- | --- |
| 27 | Rule Detection Test — Violation Density per KLOC | 0 blocking errors; < 10 warnings/KLOC | **FAIL** (61 Roslyn + 16 ESLint errors) |
| 28 | Unused Variable Detection | < 1% unused declarations per module | FAIL (both fixtures) |
| 29 | Naming Convention Validation | < 2% naming violations | FAIL (fixture modules) |
| 30 | Code Style Rule Validation | < 5 style violations per KLOC | FAIL |
| 31 | Complexity Rule Detection | 0 functions breaching nesting / length thresholds | FAIL |
| 32 | Rule Severity Classification | 0 Error-level violations | **FAIL** |
| 33 | Multiple Violations Detection | 0 files with > 10 violations | FAIL (`applicantScoring.js`: 17, `ApplicantScoringRules.cs`: 15) |
| 34 | False Positive Prevention | < 10% suppression rate | PASS (no suppressions) |
| 35 | Custom Rule Validation (C# only) | 100% of custom rules passing | FAIL (curated `.editorconfig` rule set) |
| 36 | Configuration File Handling | 0 non-standard lint configs | PASS (single root `.editorconfig` / `.eslintrc.cjs`) |
| 37 | CI/CD Integration Validation | 100% of builds pass lint gate | **FAIL** (lint gate fails in CI) |
| 38 | Violation Reporting Validation | ≥ 95% violations logged | PASS (SARIF + ESLint JSON) |
