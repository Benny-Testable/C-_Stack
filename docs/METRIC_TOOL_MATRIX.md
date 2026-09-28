# Metric and Tool Coverage Matrix — Scholarship CMGroups

Branch: `Scholarship-CMGroups-negative-1` · Repository: `Mohammed-shihaf/C-_Stack`

Source of truth: `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx` (sheet **White Box**) and `Whitebox_Tools_Registry 1.xlsx` (sheets **C# (.NET)** and **JavaScript**). Rows, thresholds and tool names are copied from those workbooks. Only the C# and JavaScript columns apply to this stack. "Row" is the row number in the White Box sheet.

## Status legend

| Status | Meaning |
| --- | --- |
| **NEGATIVE — targeted** | This branch is built to fail the threshold. Wired to the registry tool, with a deliberate fixture and a gate that fails. See [NEGATIVE_METRICS.md](NEGATIVE_METRICS.md). |
| Present (inherited) | Negative data inherited from `Scholarship-CMGroups-negative` (duplication, complexity, SAST samples, churn, thin tests). Detectable but not the target of this branch. |
| Not targeted | No deliberate test data on this branch. |

## Summary of targeted metrics

| L3 Technique | White Box rows | C# tool (registry) | JS tool (registry) | Threshold used by the gate | Result on this branch |
| --- | --- | --- | --- | --- | --- |
| Statement Coverage | 56–60 | `coverlet` — Coverlet 6.0.0 | `nyc-mocha` — NYC (Istanbul) 17.1.0 + Mocha | ≥ 80% statement coverage (row 60) | C# 9.17% · JS 11.53% → **FAIL** |
| Lint / Rule Violations | 27–38 | `roslyn-analyzers` NetAnalyzers 8.0.0 + `sonar-cs` SonarAnalyzer.CSharp 9.32.0.97167 | `eslint` ESLint 8.47.0 (+ `eslint-sonarjs` 0.25.1, `eslint-security` 3.0.1) | 0 blocking errors (rows 27, 32) | Roslyn 61 errors · ESLint 16 errors + 1 warning → **FAIL** |

## White Box metrics (C# and JavaScript)

| Row | L2 Testing Type | L3 Technique | L4 Classification | L5 Metric | C# primary / secondary | JS primary / secondary | Threshold | Status on this branch |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 6 | Structural Analysis | Cyclomatic Complexity | Static Analysis Metric | Execution Path Integrity | lizard / csharp cyclomatic | Lizard / Plato | <= 10 per function (lower is better) | Present (inherited) |
| 7 | Structural Analysis | Cyclomatic Complexity | Decision Coverage | Decision Outcome Verification | lizard / csharp cyclomatic | Lizard / Plato | >= 80% of modules in green zone (MI >= 65) | Present (inherited) |
| 8 | Structural Analysis | Cyclomatic Complexity | Condition Coverage | Logical Sub-expression Validation | lizard / csharp cyclomatic | Lizard / Plato | 0 modules in red zone (MI < 40); < 10 in yellow zone | Present (inherited) |
| 9 | Structural Analysis | Cyclomatic Complexity | Logic Coverage Metric | Total Logical Combinatorial Coverage | lizard / csharp cyclomatic | Lizard / Plato | AVG MI >= 65 across codebase | Present (inherited) |
| 10 | Structural Analysis | Cyclomatic Complexity | Maintainability Analysis | Technical Debt Impact | lizard / csharp cyclomatic | Lizard / Plato | MI Delta >= 0 (no degradation); flag if delta < -3 | Present (inherited) |
| 11 | Structural Analysis | Cyclomatic Complexity | Test Prioritization | QA Resource Allocation | lizard / csharp cyclomatic | Lizard / Plato | 0 low-MI modules with recent churn | Present (inherited) |
| 12 | Readability / Maintainability | Cognitive Complexity | Maintainability Evaluation | Technical Debt Impact | Roslyn / simple cognitive | eslint-plugin-sonarjs / sonarjs | 0 units exceeding CogCC 15; < 5 units in amber range (10–15) | Present (inherited) |
| 13 | Readability / Maintainability | Cognitive Complexity | Testability Analysis | Unit Test Complexity | Roslyn / simple cognitive | eslint-plugin-sonarjs / sonarjs | AVG CogCC <= 10 per module | Present (inherited) |
| 14 | Readability / Maintainability | Cognitive Complexity | Risk Detection | Defect Probability | Roslyn / simple cognitive | eslint-plugin-sonarjs / sonarjs | 0 functions with CogCC > 20 | Present (inherited) |
| 15 | Readability / Maintainability | Cognitive Complexity | Refactoring Guidance | Modularization Opportunity | Roslyn / simple cognitive | eslint-plugin-sonarjs / sonarjs | < 5% of functions flagged as refactor candidates | Present (inherited) |
| 16 | Readability / Maintainability | Cognitive Complexity | Code Review Support | Reviewer Fatigue Factor | Roslyn / simple cognitive | eslint-plugin-sonarjs / sonarjs | < 10% of PRs contain high-CogCC functions | Present (inherited) |
| 17 | Readability / Maintainability | Cognitive Complexity | Testing Effort Prioritization | QA Resource Allocation | Roslyn / simple cognitive | eslint-plugin-sonarjs / sonarjs | 100% of CogCC > 15 functions have dedicated test cases | Present (inherited) |
| 18 | Readability / Maintainability | Cognitive Complexity | Code Understandability Analysis | Human Cognitive Load | Roslyn / simple cognitive | eslint-plugin-sonarjs / sonarjs | <= 15 per unit (lower is better) | Present (inherited) |
| 20 | Code Quality Auditing | Code Duplication | Defect Propagation Risk Detection | Multi-Point Failure Probability | jscpd / pmd cpd | jscpd /  | 0 defect-linked duplicated blocks in codebase | Present (inherited) |
| 21 | Code Quality Auditing | Code Duplication | Refactoring Identification | Redundancy Localization | jscpd / pmd cpd | jscpd /  | 0 clone clusters with 3+ instances; < 5 two-instance clusters | Present (inherited) |
| 22 | Code Quality Auditing | Code Duplication | Code Quality Assessment | Structural Cleanliness Score | jscpd / pmd cpd | jscpd /  | >= 95% unique code (< 5% duplication) | Present (inherited) |
| 23 | Code Quality Auditing | Code Duplication | Test Maintenance Reduction | Test Suite Streamlining | jscpd / pmd cpd | jscpd /  | < 3% test code duplication | Present (inherited) |
| 24 | Code Quality Auditing | Code Duplication | Refactoring Opportunity Detection | Abstraction Potential | jscpd / pmd cpd | jscpd /  | >= 80% of duplicated lines are abstractable | Present (inherited) |
| 25 | Code Quality Auditing | Code Duplication | Risk-Based Testing Prioritization | Regression Focus Mapping | jscpd / pmd cpd | jscpd /  | 100% of duplicated modules included in regression suite | Present (inherited) |
| 26 | Code Quality Auditing | Code Duplication | Maintainability Testing | Synchronization Verification | jscpd / pmd cpd | jscpd /  | <= 5% duplicated lines preferred | Present (inherited) |
| 27 | Static Code Analysis | Lint / Rule Violations | Rule Detection Test | Violation Density per KLOC | roslyn sast / roslyn | eslint /  | 0 blocking errors; < 10 warnings per KLOC | **NEGATIVE — targeted** |
| 28 | Static Code Analysis | Lint / Rule Violations | Unused Variable Detection | Resource Waste Identification | roslyn sast / roslyn | eslint /  | < 1% unused variable declarations per module | **NEGATIVE — targeted** |
| 29 | Static Code Analysis | Lint / Rule Violations | Naming Convention Validation | Semantic Consistency Score | roslyn sast / roslyn | eslint /  | < 2% naming convention violations across codebase | **NEGATIVE — targeted** |
| 30 | Static Code Analysis | Lint / Rule Violations | Code Style Rule Validation | Syntactic Uniformity Score | roslyn sast / roslyn | eslint /  | < 5 style violations per KLOC | **NEGATIVE — targeted** |
| 31 | Static Code Analysis | Lint / Rule Violations | Complexity Rule Detection | Structural Threshold Monitoring | roslyn sast / roslyn | eslint /  | 0 functions breaching nesting or length thresholds | **NEGATIVE — targeted** |
| 32 | Static Code Analysis | Lint / Rule Violations | Rule Severity Classification | Impact Prioritization | roslyn sast / roslyn | eslint /  | 0 Error-level violations; < 10 Warning-level per KLOC | **NEGATIVE — targeted** |
| 33 | Static Code Analysis | Lint / Rule Violations | Multiple Violations Detection | Aggregated Risk Assessment | roslyn sast / roslyn | eslint /  | 0 files with > 10 violations; < 5 files in amber range | **NEGATIVE — targeted** |
| 34 | Static Code Analysis | Lint / Rule Violations | False Positive Prevention | Accuracy Tuning | roslyn sast / roslyn | eslint /  | < 10% suppression rate (high suppression signals misconfigured rules) | **NEGATIVE — targeted** |
| 35 | Static Code Analysis | Lint / Rule Violations | Custom Rule Validation | Project-Specific Enforcement | roslyn sast / roslyn | N/A /  | 100% of project-specific custom rules passing | **NEGATIVE — targeted** |
| 36 | Static Code Analysis | Lint / Rule Violations | Configuration File Handling | Environment Standardization | roslyn sast / roslyn | eslint /  | 0 developers with non-standard lint configuration | **NEGATIVE — targeted** |
| 37 | Static Code Analysis | Lint / Rule Violations | CI/CD Integration Validation | Automated Gatekeeping | roslyn sast / roslyn | eslint /  | 100% of builds pass lint quality gate before merge | **NEGATIVE — targeted** |
| 38 | Static Code Analysis | Lint / Rule Violations | Violation Reporting Validation | Quality Audit Trail | roslyn sast / roslyn | eslint /  | >= 95% of violations have complete audit log entry | **NEGATIVE — targeted** |
| 40 | Security White-box Testing | Static Vulnerabilities (SAST) | Secure Coding Validation | Best Practice Compliance | Semgrep / roslyn sast | eslint-plugin-security / security-eslint | >= 95% of functions compliant with secure coding guidelines | Present (inherited) |
| 41 | Security White-box Testing | Static Vulnerabilities (SAST) | Input Validation Testing | Entry Point Sanitization | Semgrep / roslyn sast | eslint-plugin-security / N/A | 100% of external entry points have input sanitization | Present (inherited) |
| 42 | Security White-box Testing | Static Vulnerabilities (SAST) | Data Flow Security Analysis | Sensitive Information Tracking | Semgrep / roslyn sast | eslint-plugin-security / N/A | 0 sensitive data paths reaching logs or unencrypted output | Present (inherited) |
| 43 | Security White-box Testing | Static Vulnerabilities (SAST) | Authentication & Authorization Weakness Detection | Access Control Verification | Semgrep / roslyn sast | eslint-plugin-security / N/A | 0 auth bypass paths; 0 weak session management findings | Present (inherited) |
| 44 | Security White-box Testing | Static Vulnerabilities (SAST) | Dependency & Library Vulnerability Detection | Supply Chain Security | Semgrep / roslyn sast | eslint-plugin-security / N/A | 0 vulnerable library imports in production build | Present (inherited) |
| 45 | Security White-box Testing | Static Vulnerabilities (SAST) | Compliance & Security Standard Validation | Regulatory Alignment | Semgrep / roslyn sast | eslint-plugin-security / N/A | >= 100% of regulatory controls passing static scan | Present (inherited) |
| 46 | Security White-box Testing | Static Vulnerabilities (SAST) | Security Vulnerability Detection | Exploit Surface Identification | Semgrep / roslyn sast | eslint-plugin-security / security-eslint | 0 Critical; 0 High findings per build | Present (inherited) |
| 47 | Security White-box Testing | Dependency Risk (SCA) | Transitive Dependency Analysis | Hidden Relationship Mapping | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | npm ls /  | 0 vulnerable transitive dependencies in resolved tree | Present (inherited) |
| 48 | Security White-box Testing | Dependency Risk (SCA) | License Compliance Testing | Legal Risk Validation | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | N/A /  | 0 copyleft licenses in production dependencies | Present (inherited) |
| 49 | Security White-box Testing | Dependency Risk (SCA) | Supply Chain Security Analysis | Trust Integrity Verification | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | npm audit + npm ls /  | 0 packages from unverified or deprecated sources | Present (inherited) |
| 50 | Security White-box Testing | Dependency Risk (SCA) | Dependency Health Monitoring | Community Vitality Tracking | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | npm audit + npm ls /  | 0 abandoned dependencies in production stack | Present (inherited) |
| 51 | Security White-box Testing | Dependency Risk (SCA) | Risk Prioritization | Mitigation Effort Ranking | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | npm audit + npm ls /  | 100% of Critical/High CVE deps have assigned remediation | Present (inherited) |
| 52 | Security White-box Testing | Dependency Risk (SCA) | Continuous Dependency Monitoring | Real-Time Alerting | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | npm audit + npm ls /  | 100% of new CVE alerts actioned within 24hrs (Critical) / 72hrs (High) | Present (inherited) |
| 53 | Security White-box Testing | Dependency Risk (SCA) | Vulnerability Dependency Detection | Known CVE Count | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | npm audit + npm ls /  | 0 Critical CVEs; 0 High CVEs in production dependencies | Present (inherited) |
| 54 | Security White-box Testing | Dependency Risk (SCA) | Outdated Dependency Detection | Version Lag Assessment | dotnet package list --include-transitive --vulnerable --format json (NuGet audit) / cvebin | N/A /  | 0 dependencies more than 2 major versions behind | Present (inherited) |
| 56 | Control Flow Testing | Statement Coverage | Unit Testing Support | Test Case Granularity | Coverlet / coverlet | nyc + mocha /  | >= 90% of functions have at least one dedicated unit test | **NEGATIVE — targeted** |
| 57 | Control Flow Testing | Statement Coverage | Dead Code Detection | Unreachable Logic Identification | Coverlet / coverlet | nyc + mocha /  | < 2% dead code in production modules | **NEGATIVE — targeted** |
| 58 | Control Flow Testing | Statement Coverage | Test Completeness Evaluation | Coverage Gap Analysis | Coverlet / coverlet | nyc + mocha /  | < 20% uncovered statement blocks (80% coverage floor) | **NEGATIVE — targeted** |
| 59 | Control Flow Testing | Statement Coverage | Basic Logic Validation | Surface-Level Correctness | Coverlet / coverlet | nyc + mocha /  | 100% of executed statements complete without runtime exception | **NEGATIVE — targeted** |
| 60 | Control Flow Testing | Statement Coverage | Code Execution Verification | Statement Coverage % | Coverlet / coverlet | nyc + mocha /  | >= 80% statement coverage | **NEGATIVE — targeted** |
| 61 | Control Flow Testing | Branch Coverage | Conditional Logic Testing | Boolean Accuracy Check | Coverlet / coverlet | nyc + mocha /  | >= 70% of boolean sub-expressions fully evaluated | Present (inherited) |
| 62 | Control Flow Testing | Branch Coverage | Control Flow Validation | Sequence Integrity Mapping | Coverlet / coverlet | nyc + mocha /  | 100% of expected code transitions execute in correct sequence | Present (inherited) |
| 63 | Control Flow Testing | Branch Coverage | Loop Condition Testing | Iteration Boundary Verification | Coverlet / coverlet | nyc + mocha /  | 100% of loops tested for zero-trip, one-trip, and n-trip paths | Present (inherited) |
| 64 | Control Flow Testing | Branch Coverage | Edge Case Detection | Boundary Failure Identification | Coverlet / coverlet | nyc + mocha /  | < 1% edge case branch failures | Present (inherited) |
| 65 | Control Flow Testing | Branch Coverage | Logic Error Detection | Branch Misdirection Discovery | Coverlet / coverlet | nyc + mocha /  | 0 branches producing semantically incorrect results | Present (inherited) |
| 66 | Control Flow Testing | Branch Coverage | Test Case Completeness | Decision Coverage Gap Analysis | Coverlet / coverlet | nyc + mocha /  | < 30% decision branches untested (70% branch coverage floor) | Present (inherited) |
| 67 | Control Flow Testing | Branch Coverage | Decision Outcome Verification | Branch Coverage % | Coverlet / coverlet | nyc + mocha /  | >= 70% branch coverage | Present (inherited) |
| 68 | Control Flow Testing | Path Coverage | Path Execution Tracking | (blank in source) | OpenTelemetry (.NET) + OTLP/Jaeger/Zipkin exporter / csharp call paths | nyc / N/A | >= 60% of feasible paths traced during test runs | Not targeted |
| 69 | Control Flow Testing | Path Coverage | Complete Coverage Path Verification | Full Logic Validation | altcover / csharp call paths | nyc / N/A | 100% of CC-derived paths traversed at least once | Not targeted |
| 70 | Control Flow Testing | Path Coverage | Partial Path Coverage Detection | Gap Identification | altcover / csharp call paths | nyc / N/A | < 40% of paths untested (60% floor) | Not targeted |
| 71 | Control Flow Testing | Path Coverage | Nested Condition Path Testing | Deep Logic Probing | altcover / csharp call paths | nyc / N/A | >= 60% of deeply nested paths (depth > 3) exercised | Not targeted |
| 72 | Control Flow Testing | Path Coverage | Loop Path Detection | Iterative Route Analysis | altcover / csharp call paths | N/A / N/A | 100% of loop variants (zero, one, n-trip) tested per loop | Not targeted |
| 73 | Control Flow Testing | Path Coverage | Unreachable Path Detection | Ghost Code Discovery | altcover / csharp call paths | N/A / N/A | < 2% of declared paths are unreachable (ghost code) | Not targeted |
| 74 | Control Flow Testing | Path Coverage | Exception Path Handling | Error Flow Verification | altcover / csharp call paths | N/A / N/A | >= 80% of exception paths exercised in test suite | Not targeted |
| 75 | Control Flow Testing | Path Coverage | Multi-Function Path Tracking | Cross-Component Mapping | altcover / csharp call paths | ESLint (eslint-scope) + nyc / N/A | >= 60% of cross-component paths verified | Not targeted |
| 76 | Control Flow Testing | Path Coverage | CI/CD Integration Test | Automated Quality Enforcement | altcover / csharp call paths | ESLint (eslint-scope) + nyc / N/A | 100% of builds pass minimum 60% path coverage gate | Not targeted |
| 77 | Control Flow Testing | Path Coverage | Path Detection Testing | Path Coverage % | altcover / csharp call paths | ESLint (eslint-scope) + nyc / js_paths | >= 60% path coverage (complex functions) | Not targeted |
| 79 | Mutation Testing | Mutation Score | Fault Detection Capability | Logic Error Sensitivity | Stryker.NET / stryker net | StrykerJS + Mocha / N/A | >= 70% of survived mutants caught by targeted assertion tests | Present (inherited) |
| 80 | Mutation Testing | Mutation Score | Test Coverage Quality Validation | Test Rigor Assessment | Stryker.NET / stryker net | StrykerJS + Mocha / N/A | >= 70% overall mutant kill rate | Present (inherited) |
| 81 | Mutation Testing | Mutation Score | Test Case Improvement Identification | Weak Spot Localization | Stryker.NET / stryker net | StrykerJS + Mocha / N/A | 0 modules with mutation kill rate below 50% | Present (inherited) |
| 82 | Mutation Testing | Mutation Score | Edge Case Detection | Boundary Mutant Analysis | Stryker.NET / stryker net | StrykerJS + Mocha / N/A | >= 80% of boundary operator mutants killed | Present (inherited) |
| 83 | Mutation Testing | Mutation Score | Fault Detection Capability | Logic Error Sensitivity | Stryker.NET / stryker net | StrykerJS + Mocha / N/A | Resilience Score >= 95% (< 5% kill rate degradation after change) | Present (inherited) |
| 84 | Mutation Testing | Mutation Score | Test Coverage Quality Validation | Test Rigor Assessment | Stryker.NET / stryker net | StrykerJS + Mocha / N/A | >= 75% semantic mutant kill rate | Present (inherited) |
| 85 | Mutation Testing | Mutation Score | Fault Detection Capability | Logic Error Sensitivity | Stryker.NET / stryker net | StrykerJS + Mocha / js_mutation | >= 70% mutation kill rate | Present (inherited) |
| 86 | Test Regression/Coverage Analysis | Coverage Delta | Regression Testing Monitoring | Coverage Delta % | Coverlet / coverlet delta | diff-cover /  | Delta >= 0% (no coverage regression); flag if delta < -2% | Not targeted |
| 87 | Test Regression/Coverage Analysis | Coverage Delta | Test Suite Effectiveness Tracking | Discovery Power Assessment | Coverlet / coverlet delta | diff-cover /  | >= 80% of new code lines covered by test suite | Not targeted |
| 88 | Test Regression/Coverage Analysis | Coverage Delta | CI/CD Quality Gate Enforcement | Deployment Readiness Guard | Coverlet / coverlet delta | diff-cover /  | 100% of builds show non-negative coverage delta | Not targeted |
| 89 | Test Regression/Coverage Analysis | Coverage Delta | Change Impact Analysis | Ripple Effect Mapping | Coverlet / coverlet delta | diff-cover /  | 100% of affected downstream modules re-verified | Not targeted |
| 90 | Test Regression/Coverage Analysis | Coverage Delta | New Code Testing Validation | Fresh Logic Proofing | Coverlet / coverlet delta | diff-cover /  | 100% of new or modified functions have test coverage | Not targeted |
| 91 | Test Regression/Coverage Analysis | Coverage Delta | Quality Improvement Measurement | Structural Health Benchmarking | Coverlet / coverlet delta | diff-cover /  | AVG rolling delta >= +1% (continuous improvement trend) | Not targeted |
| 93 | Data Flow Testing | All Definition Coverage | Variable Definition Detection | All-Defs Coverage % | Stryker.NET / csharp def use | ESLint (eslint-scope) + nyc /  | >= 75% all-definition coverage | Not targeted |
| 94 | Data Flow Testing | All Definition Coverage | Definition-Use Mapping | Data Path Correlation | Stryker.NET / csharp def use | ESLint (eslint-scope) + nyc /  | >= 75% of definitions linked to a verified use path | Not targeted |
| 95 | Data Flow Testing | All Definition Coverage | Coverage Measurement | DU-Path Validation | Stryker.NET / csharp def use | ESLint (eslint-scope) + nyc /  | >= 65% of all DU pairs exercised by test suite | Not targeted |
| 96 | Data Flow Testing | All Definition Coverage | Uncovered Definition Detection | Dead Data Identification | Stryker.NET / csharp def use | ESLint (eslint-scope) + nyc /  | < 5% dead variable definitions in codebase | Not targeted |
| 97 | Data Flow Testing | All Definition Coverage | Edge Case Handling | Null and Boundary Flow Analysis | Stryker.NET / csharp def use | ESLint (eslint-scope) + nyc /  | 0 unguarded null or boundary flows from definition to use | Not targeted |
| 98 | Data Flow Testing | All Definition Coverage | Reporting Validation | Audit Trail Verification | Stryker.NET / csharp def use | ESLint (eslint-scope) + nyc /  | >= 90% of DU pairs have traceable execution audit record | Not targeted |
| 99 | Data Flow Testing | All Uses Coverage | Computational Use Detection (C-Use) | Data Processing Validation | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 65% of computational use pairs exercised | Not targeted |
| 100 | Data Flow Testing | All Uses Coverage | Predicate Use Detection (P-Use) | Logic Influence Assessment | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 65% of predicate use pairs exercised (True + False outcomes) | Not targeted |
| 101 | Data Flow Testing | All Uses Coverage | Definition-Use Pair Identification | Path Correlation Mapping | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 90% of DU pairs identified and mapped in analysis | Not targeted |
| 102 | Data Flow Testing | All Uses Coverage | All-Uses Coverage Verification | Comprehensive Data Proofing | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 65% of DU pairs fully verified for both use types | Not targeted |
| 103 | Data Flow Testing | All Uses Coverage | Partial Uses Coverage Detection | Data Flow Gap Analysis | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | < 35% of DU pairs untested (65% coverage floor) | Not targeted |
| 104 | Data Flow Testing | All Uses Coverage | Multiple Definitions Handling | Ambiguity Resolution | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 80% of redefined variables tested across all definition states | Not targeted |
| 105 | Data Flow Testing | All Uses Coverage | Cross-Function Use Detection | Inter-procedural Tracking | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 60% of cross-function DU pairs exercised | Not targeted |
| 106 | Data Flow Testing | All Uses Coverage | Unreachable Use Detection | Ghost Use Identification | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | < 2% ghost uses in codebase | Not targeted |
| 107 | Data Flow Testing | All Uses Coverage | Coverage Reporting Validation | Data Integrity Audit | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 90% of DU pairs have complete data integrity audit record | Not targeted |
| 108 | Data Flow Testing | All Uses Coverage | Variable Use Detection | All-Uses Coverage % | Stryker.NET / csharp use coverage | ESLint (eslint-scope) + nyc /  | >= 65% all-uses coverage | Not targeted |
| 110 | Development Process Analysis | Code Churn | Risk-Based Testing Prioritization | Code Churn Score | pydriller / git log churn | pydriller /  | < 30% churn per sprint; flag modules with > 50% | Present (inherited) |
| 111 | Development Process Analysis | Code Churn | Regression Testing Focus | Impact-Driven Verification | pydriller / git log churn | pydriller /  | 100% of modules with churn > 30% included in regression suite | Present (inherited) |
| 112 | Development Process Analysis | Code Churn | Defect Prediction | Fault Probability Modeling | pydriller / git log churn | pydriller /  | Fault Probability Score < 5 (Churn% × Defect_Density) | Present (inherited) |
| 113 | Development Process Analysis | Code Churn | Test Case Maintenance Identification | Validation Suite Updates | pydriller / git log churn | pydriller /  | < 5% of test files stale relative to churned source modules | Present (inherited) |
| 114 | Development Process Analysis | Code Churn | Change Impact Analysis | Side Effect Mapping | pydriller / git log churn | pydriller /  | 100% of downstream impacted modules included in test run | Present (inherited) |

## Whitebox Tools Registry — C# (.NET)

| # | Platform name | Underlying tool | Version | Category | Registry status | On this branch |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | `roslyn-analyzers` | Microsoft.CodeAnalysis.NetAnalyzers | 8.0.0 | Linter / Code Style | Active | **Wired** — `Directory.Analyzers.props` (lint gate) |
| 2 | `sonar-cs` | SonarAnalyzer.CSharp | 9.32.0.97167 | SAST — SonarAnalyzer | Active | **Wired** — `Directory.Analyzers.props` (lint gate) |
| 3 | `security-code-scan` | SecurityCodeScan.VS2019 | 5.6.7 | SAST — Security | Active | Runs on the platform; no repo configuration needed |
| 4 | `jscpd-cs` | jscpd | 4.0.5 | Duplication Detection | Active | Runs on the platform; no repo configuration needed |
| 5 | `lizard` | Lizard | 1.17.10 | Complexity — Cyclomatic | Active | Runs on the platform; no repo configuration needed |
| 6 | `semgrep-perf-static` | Semgrep (Performance Rules) | 1.0.0 | Performance — Static Heuristics | Active | Runs on the platform; no repo configuration needed |
| 7 | `csharp-perf-dependency` | Roslyn + dotnet build | 1.0.0 | Performance — Dependency Analysis | Active | Runs on the platform; no repo configuration needed |
| 8 | `presidio` | Microsoft Presidio | 2.2.354 | Compliance — PII Detection | Active | Runs on the platform; no repo configuration needed |
| 9 | `semgrep-pii` | Semgrep (PII Rules) | 1.95.0 | Compliance — PII in Logs | Active | Runs on the platform; no repo configuration needed |
| 10 | `dotnet-sca` | dotnet list package (--vulnerable) | 8.0.0 | SCA — CVE Vulnerabilities | Active | Runs on the platform; no repo configuration needed |
| 11 | `git_churn` | Git (Log / Churn Analysis) | 1.0.0 | Git Metrics — Code Velocity | Active | Runs on the platform; no repo configuration needed |
| 12 | `pydriller` | PyDriller | 2.7 | Git Metrics — Technical Debt | Active | Runs on the platform; no repo configuration needed |
| 13 | `coverlet` | Coverlet | 6.0.0 | Coverage — Statement / Branch / Line | Active | **Wired** — `tests/backend/ScholarshipCMGroups.Tests` (coverage gate) |
| 14 | `altcover` | AltCover | 8.8.0 | Coverage — Structural Path | Active | Runs on the platform; no repo configuration needed |
| 15 | `semgrep` | Semgrep | 1.50.0 | SAST — Multi-Language Rules | Active | Runs on the platform; no repo configuration needed |
| 16 | `cs_coverage_delta` | diff-cover | 1.0.0 | Coverage — Delta | Active | Runs on the platform; no repo configuration needed |
| 17 | `cs_all_defs_uses` | Stryker.NET Def-Use Proxy | 0.0.1 | Coverage — Dataflow | Active | Runs on the platform; no repo configuration needed |
| 18 | `cs-side-effect` | Git Diff + AST Regex Analysis | 1.0.0 | Git Metrics — Side Effects | Active | Runs on the platform; no repo configuration needed |
| 19 | `stryker-net` | Stryker.NET | 4.0.0 | Mutation Testing | Inactive | Runs on the platform; no repo configuration needed |

## Whitebox Tools Registry — JavaScript

| # | Platform name | Underlying tool | Version | Category | Registry status | On this branch |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | `lizard` | Lizard | 1.17.31 | Complexity — Cyclomatic | Active | Runs on the platform; no repo configuration needed |
| 2 | `semgrep-perf-static` | Semgrep (Performance Rules) | 1.0.0 | Performance — Static Heuristics | Active | Runs on the platform; no repo configuration needed |
| 3 | `eslint` | ESLint | 8.47.0 | Linter / Code Quality | Active | **Wired** — `frontend/.eslintrc.cjs` (lint gate) |
| 4 | `eslint-sonarjs` | eslint-plugin-sonarjs | 0.25.1 | Complexity — Cognitive | Active | **Wired** — `frontend/.eslintrc.cjs` (lint gate) |
| 5 | `eslint-security` | eslint-plugin-security | 3.0.1 | SAST — Security | Active | **Wired** — `frontend/.eslintrc.cjs` (lint gate) |
| 6 | `jscpd-js` | jscpd | 4.0.5 | Duplication Detection | Active | Runs on the platform; no repo configuration needed |
| 7 | `npm-audit` | npm audit | 10.x | SCA — CVE Vulnerabilities | Active | Runs on the platform; no repo configuration needed |
| 8 | `madge` | Madge | 8.0.0 | Dependency Graph — Module | Active | Runs on the platform; no repo configuration needed |
| 9 | `dependency-cruiser` | dependency-cruiser | 17.3.10 | Dependency Graph — Rule Violations | Active | Runs on the platform; no repo configuration needed |
| 10 | `pydriller` | PyDriller | 2.7 | Git Metrics — Technical Debt | Active | Runs on the platform; no repo configuration needed |
| 11 | `git-churn` | Git (Log / Churn Analysis) | 1.0.0 | Git Metrics — Code Velocity | Active | Runs on the platform; no repo configuration needed |
| 12 | `npm-ls` | npm ls | 10.x | SCA — Dependency Inventory | Active | Runs on the platform; no repo configuration needed |
| 13 | `cyclonedx-npm` | @cyclonedx/cyclonedx-npm | 2.0.0 | SCA — SBOM Generation | Active | Runs on the platform; no repo configuration needed |
| 14 | `license-checker` | license-checker | 25.0.1 | SCA — Legal / License Risk | Active | Runs on the platform; no repo configuration needed |
| 15 | `osv-scanner` | OSV-Scanner (Google) | 1.9.1 | SCA — Cross-Ecosystem CVE | Active | Runs on the platform; no repo configuration needed |
| 16 | `npm-outdated` | npm outdated | 10.x | SCA — Version Lag | Active | Runs on the platform; no repo configuration needed |
| 17 | `libyear` | libyear / libyear-npm | 0.9.3 | SCA — Version Lag (lib-years) | Active | Runs on the platform; no repo configuration needed |
| 18 | `npm-view` | npm view | 10.x | SCA — Community Vitality | Active | Runs on the platform; no repo configuration needed |
| 19 | `javascript-perf-dependency` | depcheck + madge | 1.0.0 | Performance — Dependency Analysis | Active | Runs on the platform; no repo configuration needed |
| 20 | `presidio` | Microsoft Presidio | 2.2.354 | Compliance — PII Detection | Active | Runs on the platform; no repo configuration needed |
| 21 | `semgrep-pii` | Semgrep (PII Rules) | 1.95.0 | Compliance — PII in Logs | Active | Runs on the platform; no repo configuration needed |
| 22 | `nyc-mocha` | NYC (Istanbul) + Mocha | 17.1.0 | Coverage — Istanbul | Active | **Wired** — `frontend/.nycrc.json` + `.mocharc.json` (coverage gate) |
| 23 | `stryker-js` | Stryker.js | 9.6.0 | Mutation Testing | Active | Runs on the platform; no repo configuration needed |
| 24 | `coverage_delta` | diff-cover | 1.0.0 | Coverage — Delta | Active | Runs on the platform; no repo configuration needed |
| 25 | `js-path-coverage` | NYC + Espree AST | 1.0.0 | Coverage — Path | Active | Runs on the platform; no repo configuration needed |
| 26 | `js-all-defs-uses` | eslint-scope + NYC (Istanbul) | 1.0.0 | Coverage — Dataflow | Active | Runs on the platform; no repo configuration needed |
