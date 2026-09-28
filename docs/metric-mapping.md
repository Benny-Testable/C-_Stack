# Excel-to-project metric mapping

Source workbook: `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx` (v0.2).

This document maps every L5 metric row in the workbook onto Scholarship CMGroups. Formulas, thresholds, tools, and definitions are copied from the workbook. Nothing in this file invents a formula, weightage, or pass value.

Total metric rows: **206**.

Where the workbook depends on information it does not supply (previous baselines, PR history, SLA numbers, and similar), the row is marked **Requirement clarification needed**. The project still produces the artefacts those metrics would read; it does not fabricate the missing inputs. See [clarifications.md](clarifications.md).

## Worksheet counts

| Worksheet | Metric rows |
|---|---|
| White Box | 103 |
| Black Box | 20 |
| Security Code (Repository) | 9 |
| Security URL (API Service) | 17 |
| Compliance | 4 |
| Compliance URL (API Service) | 17 |
| Performance Code (Repository) | 10 |
| Performance URL (API Service) | 16 |
| Compliance Code (Repository) | 10 |

---

## White Box

### Execution Path Integrity

- **Metric:** Execution Path Integrity
- **Metric Category:** L1 White Box · L2 Structural Analysis · L3 Cyclomatic Complexity · L4 Static Analysis Metric
- **Excel Definition:** Number of independent execution paths through a function; reflects decision-point density and branch explosion risk
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: execution_path_integrity = ( functions_without_counterexample / max(total_functions_checked, 1) )
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: execution_path_complexity = wmc
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Execution Path Integrity Score = 1 / (1 + average_cyclomatic_complexity)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Execution Path Integrity Score = 1 / (1 + average_cyclomatic_complexity)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Execution Path Integrity Score = 1 / (1 + average_cyclomatic_complexity)
  - Raw Measurement Formula: CC = E – N + 2P (E=edges, N=nodes, P=connected components)
- **Expected Positive Condition:**
  - Expected Value / Threshold: <= 10 per function (lower is better)
  - Normalisation Formula (0–100): MAX(0, 100 – ((CC–1)/9 × 100)) capped 0–100
  - Execution Frequency: Every Commit / PR


### Decision Outcome Verification

- **Metric:** Decision Outcome Verification
- **Metric Category:** L1 White Box · L2 Structural Analysis · L3 Cyclomatic Complexity · L4 Decision Coverage
- **Excel Definition:** It measures whether every "fork in the road" (like an if statement) has been executed for both True and False results. It ensures that both the success and failure branches of a decision are validated.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: decision_coverage = covered_branches / max(num_branches, 1)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: decision_complexity = methodsInvokedQty
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Decision Density = average_cyclomatic_complexity Decision Outcome Verification Score = 1 / (1 + Decision Density)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Decision Density = average_cyclomatic_complexity Decision Outcome Verification Score = 1 / (1 + Decision Density)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Decision Density = average_cyclomatic_complexity Decision Outcome Verification Score = 1 / (1 + Decision Density)
  - Raw Measurement Formula: Health Score = (Modules with MI >= 65 / Total Modules) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% of modules in green zone (MI >= 65)
  - Normalisation Formula (0–100): Score = Health Score % [gate at 80%]
  - Execution Frequency: Every Commit / PR


### Logical Sub-expression Validation

- **Metric:** Logical Sub-expression Validation
- **Metric Category:** L1 White Box · L2 Structural Analysis · L3 Cyclomatic Complexity · L4 Condition Coverage
- **Excel Definition:** It measures whether each individual component of a compound logical statement (e.g., in if A and B, checking A and B separately) has been evaluated as both True and False.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: condition_coverage = covered_mcdc_conditions / max(total_mcdc_conditions, 1)
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: condition_complexity = wmc
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Condition Complexity = cyclomatic_complexity * (1 + max_nesting_depth) Logical Validation Score = 1 / (1 + Condition Complexity)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Condition Complexity = cyclomatic_complexity * (1 + max_nesting_depth) Logical Validation Score = 1 / (1 + Condition Complexity)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Condition Complexity = cyclomatic_complexity * (1 + max_nesting_depth) Logical Validation Score = 1 / (1 + Condition Complexity)
  - Raw Measurement Formula: Refactor ROI Score = Count(Modules with MI < 40) × 20 + Count(Modules with MI 40–64) × 5
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 modules in red zone (MI < 40); < 10 in yellow zone
  - Normalisation Formula (0–100): MAX(0, 100 – Refactor_ROI_Score)
  - Execution Frequency: Every Commit / PR


### Total Logical Combinatorial Coverage

- **Metric:** Total Logical Combinatorial Coverage
- **Metric Category:** L1 White Box · L2 Structural Analysis · L3 Cyclomatic Complexity · L4 Logic Coverage Metric
- **Excel Definition:** It measures the percentage of all possible unique sequences of branches that have been traveled. While branch coverage looks at individual forks, path coverage looks at the entire "journey" through the function.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: logical_combinatorial_coverage = ( functions_without_counterexample / max(total_functions_checked, 1) )
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: logical_combinatorial_complexity = wmc * methodsInvokedQty
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Total Logical Paths = cyclomatic_complexity * (1 + max_nesting_depth) Normalized Coverage Score = 1 / Total Logical Paths
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Total Logical Paths = cyclomatic_complexity * (1 + max_nesting_depth) Normalized Coverage Score = 1 / Total Logical Paths
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Total Logical Paths = cyclomatic_complexity * (1 + max_nesting_depth) Normalized Coverage Score = 1 / Total Logical Paths
  - Raw Measurement Formula: Testability Score = AVG(MI) across all modules in scope
- **Expected Positive Condition:**
  - Expected Value / Threshold: AVG MI >= 65 across codebase
  - Normalisation Formula (0–100): Score = MIN(100, AVG_MI) [already 0–100 scale]
  - Execution Frequency: Every Commit / PR


### Technical Debt Impact

- **Metric:** Technical Debt Impact
- **Metric Category:** L1 White Box · L2 Structural Analysis · L3 Cyclomatic Complexity · L4 Maintainability Analysis
- **Excel Definition:** It measures how likely a developer is to introduce a bug while trying to fix another one. High complexity scores here identify code that is "brittle" and expensive to change or support over time.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: technical_debt = sum(ccn_values)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: technical_debt = wmc + cbo + lcom
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Normalized CC = average_cyclomatic_complexity / 10 Normalized NLOC = average_nloc / 200 Normalized ND = ND / 10 Technical Debt Score = (Normalized CC * 0.5) + (Normalized NLOC * 0.3) + (Normalized ND * 0.2)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Normalized CC = average_cyclomatic_complexity / 10 Normalized NLOC = average_nloc / 200 Normalized ND = ND / 10 Technical Debt Score = (Normalized CC * 0.5) + (Normalized NLOC * 0.3) + (Normalized ND * 0.2)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Normalized CC = average_cyclomatic_complexity / 10 Normalized NLOC = average_nloc / 200 Normalized ND = ND / 10 Technical Debt Score = (Normalized CC * 0.5) + (Normalized NLOC * 0.3) + (Normalized ND * 0.2)
  - Raw Measurement Formula: MI Delta = AVG(MI_Current_Sprint) – AVG(MI_Previous_Sprint)
- **Expected Positive Condition:**
  - Expected Value / Threshold: MI Delta >= 0 (no degradation); flag if delta < -3
  - Normalisation Formula (0–100): MAX(0, 100 + (MI_Delta × 5)) capped 0–100
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires a previous sprint measurement, which a new repository does not have.


### QA Resource Allocation

- **Metric:** QA Resource Allocation
- **Metric Category:** L1 White Box · L2 Structural Analysis · L3 Cyclomatic Complexity · L4 Test Prioritization
- **Excel Definition:** It helps managers decide where to send the most experienced testers. Logic with high cognitive complexity is prioritized for deeper testing because it is the most likely place for edge-case bugs to hide.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: test_execution_efficiency = tests_saved / max(tests_all, 1)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: qa_priority = (wmc * loc) + cbo
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Function Risk Score = (cyclomatic_complexity * 0.5) + (max_nesting_depth * 0.3) + (fan_out * 0.2) QA Priority Rank = sort DESC
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Function Risk Score = (cyclomatic_complexity * 0.5) + (max_nesting_depth * 0.3) + (fan_out * 0.2) QA Priority Rank = sort DESC
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Function Risk Score = (cyclomatic_complexity * 0.5) + (max_nesting_depth * 0.3) + (fan_out * 0.2) QA Priority Rank = sort DESC
  - Raw Measurement Formula: Regression Risk Score = Count(Modules with MI < 40 AND recently churned) × 25
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 low-MI modules with recent churn
  - Normalisation Formula (0–100): MAX(0, 100 – Regression_Risk_Score)
  - Execution Frequency: Every Commit / PR


### Technical Debt Impact

- **Metric:** Technical Debt Impact
- **Metric Category:** L1 White Box · L2 Readability / Maintainability · L3 Cognitive Complexity · L4 Maintainability Evaluation
- **Excel Definition:** Evaluates how likely a developer is to introduce a bug while fixing another. High complexity scores flag code that is brittle and expensive to change or support over time.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cc - cognitive complexity value Technical Debt Score = min(100, cc * 5)
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: technical_debt = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Technical Debt Score = (Critical Issues * 5) + (High Issues * 3) + (Medium Issues * 2) + (Low Issues * 1)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Technical Debt Score = Weighted Issue Score / files_analysed
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Technical Debt Score = Weighted Issue Score / files_analysed
  - Raw Measurement Formula: Debt Score = Count(Units with CogCC > 15) × 10 + Count(Units with CogCC 10–15) × 3
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 units exceeding CogCC 15; < 5 units in amber range (10–15)
  - Normalisation Formula (0–100): MAX(0, 100 – Debt_Score)
  - Execution Frequency: Every Commit / PR


### Unit Test Complexity

- **Metric:** Unit Test Complexity
- **Metric Category:** L1 White Box · L2 Readability / Maintainability · L3 Cognitive Complexity · L4 Testability Analysis
- **Excel Definition:** Measures the difficulty of writing a test that covers all possible states. If code is hard to understand, it is mathematically harder to ensure the test suite is meaningful.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cc - cognitive complexity value Unit Test Complexity = cc + 1
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: unit_test_complexity = cognitive_violations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Test Complexity Score = (#ComplexityRelatedViolations * 3) + (#HighSeverityViolations * 2)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Complexity Issues = count(issues where ruleId indicates: "cognitive-complexity" OR "nested-control-flow" OR "complex-condition") Unit Test Complexity Score = Complexity Issues / Total Issues
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Complexity Issues = count(issues where ruleId indicates: "cognitive-complexity" OR "nested-control-flow" OR "complex-condition") Unit Test Complexity Score = Complexity Issues / Total Issues
  - Raw Measurement Formula: Test Complexity Score = AVG(CogCC) across all testable units in module
- **Expected Positive Condition:**
  - Expected Value / Threshold: AVG CogCC <= 10 per module
  - Normalisation Formula (0–100): MAX(0, 100 – ((AVG_CogCC – 1) / 9 × 100)) capped 0–100
  - Execution Frequency: Every Commit / PR


### Defect Probability

- **Metric:** Defect Probability
- **Metric Category:** L1 White Box · L2 Readability / Maintainability · L3 Cognitive Complexity · L4 Risk Detection
- **Excel Definition:** Identifies hotspots in the code where the logic is so dense that human error is almost guaranteed, acting as an early warning system before the code is deployed.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cc - cognitive complexity value Defect Probability (%) = min(90, cc * 4)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: defect_risk = cognitive_violations / max(total_violations, 1)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Defect Probability Score = (#CriticalIssues * 5) + (#BugProneIssues * 3)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: High Severity Issues = count(issues where severity >= medium) Defect Probability = High Severity Issues / Total Issues
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: High Severity Issues = count(issues where severity >= medium) Defect Probability = High Severity Issues / Total Issues
  - Raw Measurement Formula: Hotspot Count = Count(functions where CogCC > 20)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 functions with CogCC > 20
  - Normalisation Formula (0–100): MAX(0, 100 – (Hotspot_Count × 15))
  - Execution Frequency: Every Commit / PR


### Modularization Opportunity

- **Metric:** Modularization Opportunity
- **Metric Category:** L1 White Box · L2 Readability / Maintainability · L3 Cognitive Complexity · L4 Refactoring Guidance
- **Excel Definition:** Points to specific blocks of code that should be broken down into smaller, simpler functions, measuring the simplicity payoff if a specific area is refactored.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cc - cognitive complexity value Suggested Modules = ceil(cc / 5)
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: refactor_candidates = cognitive_violations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Modularization Score = (#DesignRelatedViolations * 3) + (#HighSeverityViolations * 2)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Repeated Issue Patterns = count of duplicate ruleId occurrences Modularization Opportunity Score = Repeated Issue Patterns / Total Issues
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Repeated Issue Patterns = count of duplicate ruleId occurrences Modularization Opportunity Score = Repeated Issue Patterns / Total Issues
  - Raw Measurement Formula: Refactor Candidate % = (Functions with CogCC > 15 / Total Functions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 5% of functions flagged as refactor candidates
  - Normalisation Formula (0–100): MAX(0, 100 – (Refactor_Candidate% × 10))
  - Execution Frequency: Every Commit / PR


### Reviewer Fatigue Factor

- **Metric:** Reviewer Fatigue Factor
- **Metric Category:** L1 White Box · L2 Readability / Maintainability · L3 Cognitive Complexity · L4 Code Review Support
- **Excel Definition:** Estimates how much time a peer reviewer will need to spend verifying logic. High scores suggest a code review might be ineffective because the logic is too convoluted to check visually.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cc - cognitive complexity value Fatigue Score = cc * 3
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: reviewer_fatigue = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Reviewer Fatigue = Total Violations / Number of Files
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Issue Density = Total Issues / files_analysed Reviewer Fatigue Score = Issue Density * average_severity_weight
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Issue Density = Total Issues / files_analysed Reviewer Fatigue Score = Issue Density * average_severity_weight
  - Raw Measurement Formula: Fatigue Score = Count(PRs containing functions with CogCC > 15) / Total PRs × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 10% of PRs contain high-CogCC functions
  - Normalisation Formula (0–100): MAX(0, 100 – (Fatigue_Score × 5))
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires pull-request history on the hosting provider.


### QA Resource Allocation

- **Metric:** QA Resource Allocation
- **Metric Category:** L1 White Box · L2 Readability / Maintainability · L3 Cognitive Complexity · L4 Testing Effort Prioritization
- **Excel Definition:** Helps managers direct the most experienced testers toward logic with high cognitive complexity, as it is the most likely place for edge-case bugs to hide.
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cc - cognitive complexity value if cc <= 3 → Low priority
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: qa_priority = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: QA Priority Score = (#CriticalIssues * 5) + (#BugProneIssues * 4) + (#ComplexityIssues * 2)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: File Risk Score = (# issues in file * avg severity in file) QA Priority Rank = sort(File Risk Score DESC)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: File Risk Score = (# issues in file * avg severity in file) QA Priority Rank = sort(File Risk Score DESC)
  - Raw Measurement Formula: Allocation Coverage % = (High-CogCC Functions with dedicated test cases / Total High-CogCC Functions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of CogCC > 15 functions have dedicated test cases
  - Normalisation Formula (0–100): Score = Allocation Coverage % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Human Cognitive Load

- **Metric:** Human Cognitive Load
- **Metric Category:** L1 White Box · L2 Readability / Maintainability · L3 Cognitive Complexity · L4 Code Understandability Analysis
- **Excel Definition:** Structural complexity penalising nesting depth and recursive flow; measures human comprehension difficulty
- **Required Evidence:** Source with bounded complexity
- **Project Component Producing Evidence:** ESLint `complexity`/`max-depth`, small C# service methods, table-driven transition policy
- **Source of Evidence:** Lizard / Roslyn / ESLint as named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cc - cognitive complexity value Cognitive Load Score = cc * 5
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: cognitive_load = cognitive_complexity_violations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Cognitive Load Score = (#ReadabilityViolations * 2) + (#NamingViolations * 1.5) + (#ComplexityRelatedViolations * 2)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: Unique Rule Types = count(unique(ruleId)) Node Spread = count(unique(nodeType)) Cognitive Load Score = (Total Issues + Unique Rule Types + Node Spread) / files_analysed
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Unique Rule Types = count(unique(ruleId)) Node Spread = count(unique(nodeType)) Cognitive Load Score = (Total Issues + Unique Rule Types + Node Spread) / files_analysed
  - Raw Measurement Formula: CogCC = sum of nesting-weighted increments for breaks in linear flow
- **Expected Positive Condition:**
  - Expected Value / Threshold: <= 15 per unit (lower is better)
  - Normalisation Formula (0–100): MAX(0, 100 – ((CogCC–1)/14 × 100)) capped 0–100
  - Execution Frequency: Every Commit / PR


### Multi-Point Failure Probability

- **Metric:** Multi-Point Failure Probability
- **Metric Category:** L1 White Box · L2 Code Quality Auditing · L3 Code Duplication · L4 Defect Propagation Risk Detection
- **Excel Definition:** Identifies the risk that a single bug exists in multiple locations due to copy-paste programming, measuring how likely an error is to spread across the system.
- **Required Evidence:** Duplication scan of source
- **Project Component Producing Evidence:** `npm run duplication` (jscpd) and modular C#/TS structure
- **Source of Evidence:** jscpd / copy-paste detectors named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: MPFP = (GroupSize \times RiskFactor) / TotalFragments
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: multi_point_failure = duplication_blocks
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: MultiPointFailureProbability = (duplicatedLines / lines)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: MultiPointFailureProbability = (duplicatedLines / lines)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: MultiPointFailureProbability = (duplicatedLines / lines)
  - Raw Measurement Formula: Propagation Risk = Count(Duplicated Blocks with confirmed defect history) × 20
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 defect-linked duplicated blocks in codebase
  - Normalisation Formula (0–100): MAX(0, 100 – Propagation_Risk)
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - Defect history requires a defect-tracking source, which the workbook does not define.


### Redundancy Localization

- **Metric:** Redundancy Localization
- **Metric Category:** L1 White Box · L2 Code Quality Auditing · L3 Code Duplication · L4 Refactoring Identification
- **Excel Definition:** Pinpoints specific clusters of identical or near-identical code that should be merged into a single reusable function, identifying exactly which files are bloating the project.
- **Required Evidence:** Duplication scan of source
- **Project Component Producing Evidence:** `npm run duplication` (jscpd) and modular C#/TS structure
- **Source of Evidence:** jscpd / copy-paste detectors named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Count of unique clone groups in report
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: redundancy_locations = duplication_blocks
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: RedundancyLocalization = clones / sources
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: RedundancyLocalization = clones / sources
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: RedundancyLocalization = clones / sources
  - Raw Measurement Formula: Redundancy Score = Count(Clone Clusters with >= 3 instances) × 10 + Count(Clone Clusters with 2 instances) × 3
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 clone clusters with 3+ instances; < 5 two-instance clusters
  - Normalisation Formula (0–100): MAX(0, 100 – Redundancy_Score)
  - Execution Frequency: Daily


### Structural Cleanliness Score

- **Metric:** Structural Cleanliness Score
- **Metric Category:** L1 White Box · L2 Code Quality Auditing · L3 Code Duplication · L4 Code Quality Assessment
- **Excel Definition:** Evaluates the overall cleanliness of the code by measuring the percentage of duplication. High duplication scores indicate a lack of modular design and poor abstraction.
- **Required Evidence:** Duplication scan of source
- **Project Component Producing Evidence:** `npm run duplication` (jscpd) and modular C#/TS structure
- **Source of Evidence:** jscpd / copy-paste detectors named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Derivation: 1 - (Total Duplicated Lines / Total Repository Lines)
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: structural_cleanliness_issues = duplicated_lines
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: StructuralCleanlinessScore = 1 - (duplicatedLines / lines)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: StructuralCleanlinessScore = 1 - (duplicatedLines / lines)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: StructuralCleanlinessScore = 1 - (duplicatedLines / lines)
  - Raw Measurement Formula: Cleanliness Score = 100 – ((Duplicated Lines / Total Lines) × 100)
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 95% unique code (< 5% duplication)
  - Normalisation Formula (0–100): Score = Cleanliness Score % [gate at 95%]
  - Execution Frequency: Daily


### Test Suite Streamlining

- **Metric:** Test Suite Streamlining
- **Metric Category:** L1 White Box · L2 Code Quality Auditing · L3 Code Duplication · L4 Test Maintenance Reduction
- **Excel Definition:** Measures the redundant effort spent writing similar tests for identical code blocks, reducing the number of tests that need to be managed, updated, and run.
- **Required Evidence:** Duplication scan of source
- **Project Component Producing Evidence:** `npm run duplication` (jscpd) and modular C#/TS structure
- **Source of Evidence:** jscpd / copy-paste detectors named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Duplicated Lines / Total lines
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: test_redundancy = duplication_blocks
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: TestSuiteStreamlining = duplicatedLines / lines
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: TestSuiteStreamlining = duplicatedLines / lines
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: TestSuiteStreamlining = duplicatedLines / lines
  - Raw Measurement Formula: Redundant Test % = (Duplicated Test Blocks / Total Test Lines) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 3% test code duplication
  - Normalisation Formula (0–100): MAX(0, 100 – (Redundant_Test% / 3 × 100))
  - Execution Frequency: Daily


### Abstraction Potential

- **Metric:** Abstraction Potential
- **Metric Category:** L1 White Box · L2 Code Quality Auditing · L3 Code Duplication · L4 Refactoring Opportunity Detection
- **Excel Definition:** Identifies patterns where common logic can be abstracted into a shared library or parent class, measuring the simplicity gain from replacing repeated blocks with a single well-defined interface.
- **Required Evidence:** Duplication scan of source
- **Project Component Producing Evidence:** `npm run duplication` (jscpd) and modular C#/TS structure
- **Source of Evidence:** jscpd / copy-paste detectors named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Avg Fragment Size \times Clone Group Size
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: abstraction_opportunities = duplication_blocks
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: AbstractionPotential = duplicatedTokens / tokens
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AbstractionPotential = duplicatedTokens / tokens
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: AbstractionPotential = duplicatedTokens / tokens
  - Raw Measurement Formula: Abstraction Score = (Lines Reducible via Abstraction / Total Duplicated Lines) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% of duplicated lines are abstractable
  - Normalisation Formula (0–100): Score = Abstraction Score % [gate at 80%]
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - The workbook does not define how reducible lines are determined.


### Regression Focus Mapping

- **Metric:** Regression Focus Mapping
- **Metric Category:** L1 White Box · L2 Code Quality Auditing · L3 Code Duplication · L4 Risk-Based Testing Prioritization
- **Excel Definition:** Helps prioritize testing for areas where duplication is high, directing QA resources to verify that all instances of duplicated logic behave identically under stress.
- **Required Evidence:** Duplication scan of source
- **Project Component Producing Evidence:** `npm run duplication` (jscpd) and modular C#/TS structure
- **Source of Evidence:** jscpd / copy-paste detectors named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Total Duplicated Tokens / Total Repository Tokens
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: regression_focus = duplication_blocks
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: RegressionFocusMapping = (duplicatedLines / lines) * (clones / sources)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: RegressionFocusMapping = (duplicatedLines / lines) * (clones / sources)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: RegressionFocusMapping = (duplicatedLines / lines) * (clones / sources)
  - Raw Measurement Formula: Regression Focus % = (Duplicated Modules Covered by Regression Tests / Total Duplicated Modules) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of duplicated modules included in regression suite
  - Normalisation Formula (0–100): Score = Regression Focus % [gate at 100%]
  - Execution Frequency: Daily


### Synchronization Verification

- **Metric:** Synchronization Verification
- **Metric Category:** L1 White Box · L2 Code Quality Auditing · L3 Code Duplication · L4 Maintainability Testing
- **Excel Definition:** % of code lines that are duplicated (cloned blocks) across the repository; indicates copy-paste debt
- **Required Evidence:** Duplication scan of source
- **Project Component Producing Evidence:** `npm run duplication` (jscpd) and modular C#/TS structure
- **Source of Evidence:** jscpd / copy-paste detectors named in Excel
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Lines in SYNC fragment / Lines in ASYNC fragment
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: synchronization_risk = files_involved
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: SynchronizationVerification = duplicatedLines / lines
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SynchronizationVerification = duplicatedLines / lines
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: SynchronizationVerification = duplicatedLines / lines
  - Raw Measurement Formula: Duplication % = (Duplicated Lines / Total Lines) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: <= 5% duplicated lines preferred
  - Normalisation Formula (0–100): MAX(0, 100 – (Dup% / 5 × 100)) [5% = score 0]
  - Execution Frequency: Daily


### Violation Density per KLOC

- **Metric:** Violation Density per KLOC
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Rule Detection Test
- **Excel Definition:** Count of lint violations bucketed by severity (error, warning, info) per 1000 lines of code
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: N = total number of JSON objects (violations)
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: violation_density = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Violation Density = (TotalDiagnostics / LineCount) * 1000
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: "TotalViolations = errorCount + warningCount ViolationDensity = TotalViolations / (LOC / 1000)"
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: totalViolations = errors + warnings + infos if totalViolations == 0 → return 0 (or 1 depending on metric intent)
  - Raw Measurement Formula: Violation Density = (Errors×3 + Warnings×1) / (LOC/1000)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 blocking errors; < 10 warnings per KLOC
  - Normalisation Formula (0–100): MAX(0, 100 – (Errors×5 + Warnings×1))
  - Execution Frequency: Every Commit / PR


### Resource Waste Identification

- **Metric:** Resource Waste Identification
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Unused Variable Detection
- **Excel Definition:** Scans the code for variables that are declared but never referenced in any operation, measuring technical debt from dead allocations that clutter the code and confuse future maintainers.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Total Duplicate Lines = SUM(endLine - line + 1)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: unused_variable_violations = count( error where source contains "Unused" )
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Resource Waste Proxy = VariableDeclarations - IdentifierUsageReferences
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: "UnusedVarViolations = count(messages where ruleId == ""no-unused-vars"") ResourceWasteIdentification = UnusedVarViolations / TotalViolations"
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Resource Waste Score = count(diagnostics where message/category contains "unused") / (errors + warnings + infos)
  - Raw Measurement Formula: Dead Allocation % = (Unused Variables / Total Declared Variables) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 1% unused variable declarations per module
  - Normalisation Formula (0–100): MAX(0, 100 – (Dead_Allocation% × 50))
  - Execution Frequency: Every Commit / PR


### Semantic Consistency Score

- **Metric:** Semantic Consistency Score
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Naming Convention Validation
- **Excel Definition:** Verifies that variables, functions, and classes follow specific casing and descriptive standards, measuring the guessability and professionalism of the code to improve readability.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Semantic Consistency = 1 - (Unique Rule Types / Total Violations)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: semantic_consistency = 1 / (1 + missing_javadoc)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Semantic Consistency = IdentifierName / SyntaxNodeTotal
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: "NamingViolations = count(messages where ruleId contains ""camelcase"" OR ""naming"") SemanticConsistencyScore = 1 - (NamingViolations / TotalViolations)"
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Semantic Consistency Score = 1 - ( count(diagnostics where category contains "naming") / (errors + warnings + infos) )
  - Raw Measurement Formula: Convention Violation Rate = (Naming Violations / Total Named Identifiers) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 2% naming convention violations across codebase
  - Normalisation Formula (0–100): MAX(0, 100 – (Convention_Violation_Rate × 25))
  - Execution Frequency: Every Commit / PR


### Syntactic Uniformity Score

- **Metric:** Syntactic Uniformity Score
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Code Style Rule Validation
- **Excel Definition:** Enforces rules regarding indentation, line length, and whitespace to ensure visual consistency so that code written by multiple developers looks uniform.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Uniformity Score = 1 - (STDEV(duplicate line counts) / AVERAGE(duplicate line counts))
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: syntactic_uniformity = 1 / (1 + indentation_issues)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Syntactic Uniformity = 1 - (ParseDiagnostics / SyntaxNodeTotal)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SyntacticUniformityScore = 1 - (fixableWarningCount / TotalViolations)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Syntactic Uniformity Score = 1 - ( count(diagnostics where category contains "style") / (errors + warnings + infos) )
  - Raw Measurement Formula: Style Violation Density = Style Violations / (LOC / 1000)
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 5 style violations per KLOC
  - Normalisation Formula (0–100): MAX(0, 100 – (Style_Violation_Density × 10))
  - Execution Frequency: Every Commit / PR


### Structural Threshold Monitoring

- **Metric:** Structural Threshold Monitoring
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Complexity Rule Detection
- **Excel Definition:** Flags functions that exceed specific limits for nesting depth or length, measuring the mental load required to process a module and identifying candidates for immediate refactoring.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Threshold Score = COUNT(duplicate lines > 10) / Total Violations
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: structural_threshold = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Structural Complexity Score = (IfStatement + ForStatement + WhileStatement) / LineCount
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: "ComplexityViolations = count(messages where ruleId == ""complexity"") StructuralThresholdMonitoring = ComplexityViolations / TotalViolations"
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Structural Risk Score = count(diagnostics where category contains "complexity") / (errors + warnings + infos)
  - Raw Measurement Formula: Complexity Breach Count = Count(Functions exceeding nesting depth > 4 or length > 50 lines)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 functions breaching nesting or length thresholds
  - Normalisation Formula (0–100): MAX(0, 100 – (Complexity_Breach_Count × 10))
  - Execution Frequency: Every Commit / PR


### Impact Prioritization

- **Metric:** Impact Prioritization
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Rule Severity Classification
- **Excel Definition:** Categorizes violations as Errors, Warnings, or Info based on their risk level, helping developers focus on fixing critical structural flaws before minor stylistic nitpicks.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Impact Score = Unique Modules / Unique Files
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: impact_priority = errors * 2 + warnings
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Impact Score = TotalDiagnostics / SyntaxNodeTotal
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: ImpactPrioritization = errorCount / TotalViolations
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Weighted Impact Score = (errors * 3 + warnings * 2 + infos) / (errors + warnings + infos)
  - Raw Measurement Formula: Severity Score = Count(Error violations) × 10 + Count(Warning violations) × 2 + Count(Info violations) × 0.5
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 Error-level violations; < 10 Warning-level per KLOC
  - Normalisation Formula (0–100): MAX(0, 100 – Severity_Score)
  - Execution Frequency: Every Commit / PR


### Aggregated Risk Assessment

- **Metric:** Aggregated Risk Assessment
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Multiple Violations Detection
- **Excel Definition:** Identifies modules that suffer from a high density of different rule breaks, measuring the instability of a file and highlighting parts of the system most likely to contain bugs.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Risk Score = Total Violations * Average Duplicate Lines
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: aggregated_risk = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Aggregated Risk Score = (TotalDiagnostics ^ 2) / LineCount
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AggregatedRiskAssessment = (errorCount * 2 + warningCount) / TotalViolations
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Aggregated Risk Score = (errors + warnings + infos) / count(distinct diagnostic.location.path)
  - Raw Measurement Formula: Hotfile Score = Count(Files with > 10 violations) × 15 + Count(Files with 5–10 violations) × 5
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 files with > 10 violations; < 5 files in amber range
  - Normalisation Formula (0–100): MAX(0, 100 – Hotfile_Score)
  - Execution Frequency: Every Commit / PR


### Accuracy Tuning

- **Metric:** Accuracy Tuning
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 False Positive Prevention
- **Excel Definition:** Filters out rule violations that are intentional or contextually irrelevant, measuring the reliability of the analysis tool to ensure developers do not ignore valid warnings.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Accuracy = 1 - (Unique Rule IDs - 1) / Total Violations
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: accuracy_proxy = 1 / (1 + warnings)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Diagnostic Stability = 1 - (ParseDiagnostics / TotalDiagnostics)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: "SuppressedCount = length(suppressedMessages) AccuracyTuning = 1 - (SuppressedCount / TotalViolations)"
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Accuracy Score = 1 - ( (diagnosticsNotPrinted + suggestedFixesSkipped) / (errors + warnings + infos) )
  - Raw Measurement Formula: False Positive Rate % = (Intentionally suppressed violations / Total Flagged Violations) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 10% suppression rate (high suppression signals misconfigured rules)
  - Normalisation Formula (0–100): MAX(0, 100 – (False_Positive_Rate% × 5))
  - Execution Frequency: Every Commit / PR


### Project-Specific Enforcement

- **Metric:** Project-Specific Enforcement
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Custom Rule Validation
- **Excel Definition:** Allows the team to create and enforce unique rules tailored to specific requirements, measuring compliance with internal business logic or architecture that generic tools might miss.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Project Enforcement = Unique Modules / Total Violations
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: custom_rule_enforcement = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Custom Rule Coverage = TotalDiagnostics / SyntaxNodeTotal
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: "CustomRuleViolations = count(messages where ruleId contains ""/"") ProjectSpecificEnforcement = CustomRuleViolations / TotalViolations"
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Custom Enforcement Score = count(diagnostics where category contains "custom") / (errors + warnings + infos)
  - Raw Measurement Formula: Custom Rule Pass Rate % = (Custom Rule Checks Passing / Total Custom Rule Checks) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of project-specific custom rules passing
  - Normalisation Formula (0–100): Score = Custom Rule Pass Rate % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Environment Standardization

- **Metric:** Environment Standardization
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Configuration File Handling
- **Excel Definition:** Manages the configuration files that define which rules are active across the team, ensuring every developer on the project is using the exact same quality settings.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Standardization = Unique Types / Total Violations
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: environment_uniformity = 1 / (1 + violations_count)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Standardization Score = 1 - (CompilationDiagnostics / SyntaxNodeTotal)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: EnvironmentStandardization = 1 - (usedDeprecatedRules.length / TotalViolations)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Standardization Score = (changed + unchanged) / (changed + unchanged + skipped)
  - Raw Measurement Formula: Config Drift Score = Count(Devs with lint config deviating from team standard)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 developers with non-standard lint configuration
  - Normalisation Formula (0–100): MAX(0, 100 – (Config_Drift_Score × 25))
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires multiple developer configurations and a declared team standard.
  - Requires multiple contributors, which a single-author project does not have.


### Automated Gatekeeping

- **Metric:** Automated Gatekeeping
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 CI/CD Integration Validation
- **Excel Definition:** Runs quality checks automatically during the build process, measuring readiness for merge by blocking any code that fails to meet the minimum quality bar before it can be merged.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Gate = IF(Total Violations > Threshold, "FAIL", "PASS")
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: gatekeeping_score = errors
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Gatekeeping Pass = (CompilationDiagnostics == 0) ? 1 : 0
  - JavaScript :: Metrics emitted directly: Yes
  - JavaScript :: Derivation: AutomatedGatekeeping = fatalErrorCount
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Gate Score = 1 - (errors / max(totalViolations, 1))
  - Raw Measurement Formula: Pipeline Gate Pass Rate % = (Builds passing lint gate / Total Builds) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of builds pass lint quality gate before merge
  - Normalisation Formula (0–100): Score = Pipeline Gate Pass Rate % [gate at 100%]
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires CI build history.


### Quality Audit Trail

- **Metric:** Quality Audit Trail
- **Metric Category:** L1 White Box · L2 Static Code Analysis · L3 Lint / Rule Violations · L4 Violation Reporting Validation
- **Excel Definition:** Generates detailed logs of all detected issues including line numbers and fix suggestions, providing a transparent record of code health for reviews and long-term quality tracking.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Audit Score = Filled Fields / Total Expected Fields
  - Java :: Metrics emitted directly: Yes
  - Java :: Derivation: audit_trail = violations_count
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Audit Trail Score = TotalDiagnostics
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: QualityAuditTrail = messages.length / TotalViolations
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Reporting Completeness = (errors + warnings + infos) / ((errors + warnings + infos) + diagnosticsNotPrinted) Audit Efficiency = scannerDuration / duration
  - Raw Measurement Formula: Report Coverage % = (Violations with line-number + fix suggestion logged / Total Violations) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 95% of violations have complete audit log entry
  - Normalisation Formula (0–100): Score = Report Coverage % [gate at 95%]
  - Execution Frequency: Every Commit / PR


### Best Practice Compliance

- **Metric:** Best Practice Compliance
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Static Vulnerabilities (SAST) · L4 Secure Coding Validation
- **Excel Definition:** Checks the code against industry-standard secure coding guidelines like OWASP, ensuring developers use safe functions and avoid dangerous coding habits that lead to vulnerabilities.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Compliance Score = 1 - ((Semgrep ERROR + Bandit HIGH + Bandit MEDIUM) / loc)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Best Practice Compliance (BPC) = 1 - (total_bugs / total_classes)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Best Practice Compliance Score = 1 - (WeightedCount(best_practices) / TotalWeightedFindings)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: BestPracticeCompliance = 1 - (errorCount / TotalVulnerabilities)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: BestPracticeCompliance = 1 - (errorCount / TotalVulnerabilities)
  - Raw Measurement Formula: Compliance Score = (OWASP-Compliant Functions / Total Functions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 95% of functions compliant with secure coding guidelines
  - Normalisation Formula (0–100): Score = Compliance Score % [gate at 95%]
  - Execution Frequency: Every Commit / PR


### Entry Point Sanitization

- **Metric:** Entry Point Sanitization
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Static Vulnerabilities (SAST) · L4 Input Validation Testing
- **Excel Definition:** Measures how effectively the code cleans data coming from external sources, identifying locations where user input is used directly in sensitive operations without being checked for malicious content.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Entry Points = count(Semgrep injection-related results)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Entry Point Sanitization Score (EPS) = 1 - (total_bugs / total_size)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Entry Point Sanitization Risk = WeightedCount(input_validation)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: InputValidationIssues = count(messages where ruleId contains "detect-non-literal" OR "unsafe-regex") EntryPointSanitization = 1 - (InputValidationIssues / TotalVulnerabilities)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: InputValidationIssues = count(messages where ruleId contains "detect-non-literal" OR "unsafe-regex") EntryPointSanitization = 1 - (InputValidationIssues / TotalVulnerabilities)
  - Raw Measurement Formula: Sanitization Coverage % = (Sanitized Entry Points / Total External Entry Points) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of external entry points have input sanitization
  - Normalisation Formula (0–100): Score = Sanitization Coverage % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Sensitive Information Tracking

- **Metric:** Sensitive Information Tracking
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Static Vulnerabilities (SAST) · L4 Data Flow Security Analysis
- **Excel Definition:** Tracks the movement of sensitive data through the application, ensuring private data is encrypted or masked and never leaks into logs or insecure output streams.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Sensitive Exposure Count = count(Semgrep secret-related results)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Sensitive Data Flow Risk (SDFR) = total_bugs / total_size
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Sensitive Data Exposure Score = WeightedCount(secrets + dataflow) Sensitive Data Density = Sensitive Data Exposure Score / FilesCount
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SensitiveDataIssues = count(messages where ruleId contains "detect-object-injection" OR "detect-possible-timing-attacks") SensitiveInformationTracking = 1 - (SensitiveDataIssues / TotalVulnerabilities)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: SensitiveDataIssues = count(messages where ruleId contains "detect-object-injection" OR "detect-possible-timing-attacks") SensitiveInformationTracking = 1 - (SensitiveDataIssues / TotalVulnerabilities)
  - Raw Measurement Formula: Data Leak Score = Count(Sensitive Data Paths reaching insecure output) × 30
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 sensitive data paths reaching logs or unencrypted output
  - Normalisation Formula (0–100): MAX(0, 100 – Data_Leak_Score) [any leak = BLOCK]
  - Execution Frequency: Every Commit / PR


### Access Control Verification

- **Metric:** Access Control Verification
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Static Vulnerabilities (SAST) · L4 Authentication & Authorization Weakness Detection
- **Excel Definition:** Identifies flaws in how the application verifies user identity and permissions, flagging hardcoded passwords, weak session management, or logic that allows users to bypass security checks.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Access Control Risk = count(Semgrep auth-related results)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Access Control Risk (ACR) = total_bugs / total_classes
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Access Control Risk Score = WeightedCount(auth + authorization) Access Control Density = Access Control Risk Score / FilesCount
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AuthIssues = count(messages where ruleId contains "detect-eval-with-expression") AccessControlVerification = 1 - (AuthIssues / TotalVulnerabilities)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: AuthIssues = count(messages where ruleId contains "detect-eval-with-expression") AccessControlVerification = 1 - (AuthIssues / TotalVulnerabilities)
  - Raw Measurement Formula: Auth Weakness Score = Count(Auth Bypass Paths) × 25 + Count(Weak Session Issues) × 10
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 auth bypass paths; 0 weak session management findings
  - Normalisation Formula (0–100): MAX(0, 100 – Auth_Weakness_Score)
  - Execution Frequency: Every Commit / PR


### Supply Chain Security

- **Metric:** Supply Chain Security
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Static Vulnerabilities (SAST) · L4 Dependency & Library Vulnerability Detection
- **Excel Definition:** Checks third-party libraries against databases of known security issues, measuring the risk of inherited vulnerabilities that exist in code you did not write yourself.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Supply Chain Risk = count(Semgrep dependency-related results)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Supply Chain Risk Proxy (SCRP) = (total_bugs / total_size) * total_classes
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Sensitive Data Exposure Score = WeightedCount(secrets + dataflow) Sensitive Data Density = Sensitive Data Exposure Score / FilesCount
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DynamicExecutionIssues = count(messages where ruleId contains "detect-non-literal-require") SupplyChainSecurity = DynamicExecutionIssues / TotalVulnerabilities
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: DynamicExecutionIssues = count(messages where ruleId contains "detect-non-literal-require") SupplyChainSecurity = DynamicExecutionIssues / TotalVulnerabilities
  - Raw Measurement Formula: Supply Chain Score = Count(Vuln Library Imports) × 20 + Count(Outdated Imports) × 5
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 vulnerable library imports in production build
  - Normalisation Formula (0–100): MAX(0, 100 – Supply_Chain_Score)
  - Execution Frequency: Every Commit / PR


### Regulatory Alignment

- **Metric:** Regulatory Alignment
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Static Vulnerabilities (SAST) · L4 Compliance & Security Standard Validation
- **Excel Definition:** Verifies if the code meets specific regulatory requirements such as GDPR or HIPAA, providing a report on whether the application is legally and technically secure based on external standards.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Regulatory Coverage = count(Semgrep results with CWE/OWASP)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Regulatory Alignment Score (RAS) = 1 - (total_bugs / total_size)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Regulatory Compliance Score = 1 - (WeightedCount(compliance_rules) / TotalWeightedFindings)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: HighSeverityIssues = errorCount RegulatoryAlignment = 1 - (HighSeverityIssues / TotalVulnerabilities)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: HighSeverityIssues = errorCount RegulatoryAlignment = 1 - (HighSeverityIssues / TotalVulnerabilities)
  - Raw Measurement Formula: Regulatory Score = (Controls Passing Scan / Total Required Controls) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 100% of regulatory controls passing static scan
  - Normalisation Formula (0–100): Score = Regulatory Score % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Exploit Surface Identification

- **Metric:** Exploit Surface Identification
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Static Vulnerabilities (SAST) · L4 Security Vulnerability Detection
- **Excel Definition:** Count of high/critical severity vulnerabilities found by static analysis; identifies injection risks, insecure patterns, auth weaknesses
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Exploit Surface = Semgrep ERROR + Semgrep WARNING + Bandit HIGH + Bandit MEDIUM
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Exploit Surface (ES) = total_bugs / total_size
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Exploit Surface Score = WeightedCount(high_risk_vulnerabilities) Exploit Surface Density = Exploit Surface Score / FilesCount
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: ExploitSurfaceIdentification = TotalVulnerabilities / (TotalVulnerabilities + 1)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: ExploitSurfaceIdentification = TotalVulnerabilities / (TotalVulnerabilities + 1)
  - Raw Measurement Formula: SAST Score = Count(Critical)×25 + Count(High)×10 + Count(Medium)×3 + Count(Low)×1
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 Critical; 0 High findings per build
  - Normalisation Formula (0–100): MAX(0, 100 – SAST_Score)
  - Execution Frequency: Every Commit / PR


### Hidden Relationship Mapping

- **Metric:** Hidden Relationship Mapping
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 Transitive Dependency Analysis
- **Excel Definition:** Identifies and analyzes dependencies of dependencies to uncover hidden risks deep in your software stack, measuring the total depth of your external code tree.
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Hidden Relationship Risk = count(total vulnerabilities) / count(dependencies)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: total_packages = sum(len(dep.packages) for dep in dependencies) hidden_relationship_score = total_packages / TD
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Transitive Dependency Ratio = TransitivePackages / TotalPackages Hidden Dependency Score = TransitivePackages
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: HiddenRelationshipMapping = TotalVulnerabilities === 0 ? 0 : TransitiveVulns / TotalVulnerabilities
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Hidden Dependency Ratio = count(dependencies where dependsOn.length > 0) / count(components)
  - Raw Measurement Formula: Transitive Risk Score = Count(Vulnerable Transitive Deps) × 20 + Count(Flagged Transitive Deps) × 5
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 vulnerable transitive dependencies in resolved tree
  - Normalisation Formula (0–100): MAX(0, 100 – Transitive_Risk_Score)
  - Execution Frequency: Daily


### Legal Risk Validation

- **Metric:** Legal Risk Validation
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 License Compliance Testing
- **Excel Definition:** Checks the legal licenses of every library used to ensure they align with your project goals, measuring the risk of legal action or forced open-sourcing due to restrictive licenses.
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Legal Risk Proxy = count(CVE-linked vulnerabilities)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: License Risk Score (LRS) = count(dependencies with license != approved_list) / TD
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Legal Risk Score = (Critical * 5) + (High * 4) + (Moderate * 2) + (Low * 1) + ((TransitivePackages / TotalPackages) * 2) + ((OutdatedPackages / TotalPackages) * 2)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: RiskyLicenses = count(license contains "GPL" OR "AGPL" OR "UNLICENSED")
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: License Risk Score = count(evaluator.violations) / count(packages)
  - Raw Measurement Formula: License Risk = Count(Copyleft Deps) × 20 + Count(Restricted Deps) × 10
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 copyleft licenses in production dependencies
  - Normalisation Formula (0–100): MAX(0, 100 – License_Risk)
  - Execution Frequency: Weekly


### Trust Integrity Verification

- **Metric:** Trust Integrity Verification
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 Supply Chain Security Analysis
- **Excel Definition:** Evaluates the integrity of the source and delivery path of your libraries to prevent poisoned packages, measuring the reliability of maintainers and security of repositories.
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Supply Chain Risk = count(all vulnerabilities across dependencies)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: deps_with_hash = count(dep for dep in dependencies if dep.sha256 exists) TCS = deps_with_hash / TD
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Trust Risk Score = (Critical * 5) + (High * 4) + (Moderate * 2) + (Low * 1)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: TrustIntegrityVerification = 1 - (TotalVulnerabilities / TotalDependencies)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Vulnerability Density = count(matches) / count(components) Critical Vulnerability Ratio = count(vulnerabilities where severity == "CRITICAL") / count(matches)
  - Raw Measurement Formula: Trust Score = Count(Unverified Package Sources) × 25 + Count(Deprecated Registries) × 10
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 packages from unverified or deprecated sources
  - Normalisation Formula (0–100): MAX(0, 100 – Trust_Score)
  - Execution Frequency: Daily


### Community Vitality Tracking

- **Metric:** Community Vitality Tracking
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 Dependency Health Monitoring
- **Excel Definition:** Measures the activity level and support of open-source projects you rely on, identifying zombie libraries abandoned by their creators that are unlikely to be updated if a bug is found.
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Community Vitality Score = count(vulnerabilities with fix_versions) / count(total vulnerabilities)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Health Risk Proxy (HRP) = count(dependencies with vulnerabilities) / TD
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Outdated Dependency Ratio = OutdatedPackages / TotalPackages
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: VulnerableDependencies = count(unique nodes from npm audit) CommunityVitalityTracking = 1 - (VulnerableDependencies / TotalDependencies)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Dependency Health Monitoring = count(dependencies where current == latest) / count(dependencies)
  - Raw Measurement Formula: Vitality Score = Count(Abandoned Deps, no commit > 12 months) × 20 + Count(Low-activity Deps) × 5
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 abandoned dependencies in production stack
  - Normalisation Formula (0–100): MAX(0, 100 – Vitality_Score)
  - Execution Frequency: Weekly


### Mitigation Effort Ranking

- **Metric:** Mitigation Effort Ranking
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 Risk Prioritization
- **Excel Definition:** Ranks identified dependency issues based on severity and exploitability, helping decide which patch to apply first when development time is limited.
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Mitigation Effort = count(vulnerabilities with fix_versions)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: risk_score = sum(all_cvss_scores) / len(all_cvss_scores)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Mitigation Priority Score = (Critical * 5) + (High * 4) + (Moderate * 2) + (Low * 1)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: MitigationEffortRanking = WeightedRisk / (TotalVulnerabilities * 4)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Weighted Vulnerability Score = sum(weight(match.vulnerability.severity)) / count(matches)
  - Raw Measurement Formula: Prioritization Coverage % = (Critical/High CVE Deps with assigned fix / Total Critical/High CVE Deps) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of Critical/High CVE deps have assigned remediation
  - Normalisation Formula (0–100): Score = Prioritization Coverage % [gate at 100%]
  - Execution Frequency: Daily


### Real-Time Alerting

- **Metric:** Real-Time Alerting
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 Continuous Dependency Monitoring
- **Excel Definition:** Provides an ongoing watchdog service that alerts immediately when a new vulnerability is discovered in your existing stack, acting as a permanent quality gate as the MVP evolves.
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Alert Signal = count(new vulnerabilities detected in scan)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: high_risk_vulns = count( vuln for dep in dependencies for vuln in dep.vulnerabilities if vuln.severity in ["HIGH", "CRITICAL"] ) alert_score = high_risk_vulns
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Real-Time Alert Indicator = (VulnerablePackages > 0) ? 1 : 0
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: RealTimeAlerting = TotalVulnerabilities > 0 ? 1 : 0
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Alert Density = count(security_advisories) / count(components)
  - Raw Measurement Formula: Alert Response Rate % = (New CVE Alerts Actioned within SLA / Total New CVE Alerts) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of new CVE alerts actioned within 24hrs (Critical) / 72hrs (High)
  - Normalisation Formula (0–100): Score = Alert Response Rate % [gate at 100%]
  - Execution Frequency: Daily


### Known CVE Count

- **Metric:** Known CVE Count
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 Vulnerability Dependency Detection
- **Excel Definition:** Count of known CVEs in third-party dependencies, categorised by CVSS severity
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Known CVE Count = count(vulnerabilities with CVE in aliases)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: cve_count = sum(len(dep.vulnerabilities) for dep in dependencies)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Known CVE Count = Total Vulnerabilities
  - JavaScript :: Metrics emitted directly: Yes
  - JavaScript :: Derivation: KnownCVECount = TotalVulnerabilities
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Known CVE Count = count(distinct vulnerability.id) Fix Availability Ratio = count(matches where vulnerability.fix exists) / count(matches)
  - Raw Measurement Formula: CVE Score = Count(Crit)×25 + Count(High)×10 + Count(Med)×3 + Count(Low)×1
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 Critical CVEs; 0 High CVEs in production dependencies
  - Normalisation Formula (0–100): MAX(0, 100 – CVE_Score)
  - Execution Frequency: Daily


### Version Lag Assessment

- **Metric:** Version Lag Assessment
- **Metric Category:** L1 White Box · L2 Security White-box Testing · L3 Dependency Risk (SCA) · L4 Outdated Dependency Detection
- **Excel Definition:** Measures the age of your libraries by comparing installed versions against the latest stable releases, identifying components that no longer receive security patches.
- **Required Evidence:** Lockfiles and restore/audit output
- **Project Component Producing Evidence:** `package-lock.json`, NuGet references, `NuGetAudit` in `Directory.Build.props`
- **Source of Evidence:** npm/NuGet audit
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Version Lag = count(vulnerabilities with fix_versions)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: outdated_deps = count( dep for dep in dependencies for vuln in dep.vulnerabilities if dep.version < vuln.vulnerableSoftware.versionEndExcluding ) version_lag_score = outdated_deps / total_dependencies
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Version Lag Ratio = OutdatedPackages / TotalPackages
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: OutdatedDeps = count(dependencies where currentVersion < latestVersion) VersionLagAssessment = OutdatedDeps / TotalDependencies
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Version Lag Score = count(dependencies where current != latest) / count(dependencies) Update Ratio = count(dependencies where current != latest) / count(dependencies)
  - Raw Measurement Formula: Version Lag Score = Count(Deps > 2 major versions behind) × 15 + Count(Deps 1 major version behind) × 5
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 dependencies more than 2 major versions behind
  - Normalisation Formula (0–100): MAX(0, 100 – Version_Lag_Score)
  - Execution Frequency: Daily


### Test Case Granularity

- **Metric:** Test Case Granularity
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Statement Coverage · L4 Unit Testing Support
- **Excel Definition:** Identifies exactly which lines of a function are covered by unit tests and which are ignored, helping developers write more focused tests by highlighting hidden blocks of code.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Granularity Proxy = num_statements / count(dependencies)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: test_granularity_score = 1 - (total_bugs / total_classes)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Test Case Granularity = coveredMethods / totalMethods
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: TestCaseGranularity = TotalTests / TestSuites
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Test Case Granularity = statements.covered / functions.total
  - Raw Measurement Formula: Granularity Score = (Functions with >= 1 dedicated unit test / Total Functions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 90% of functions have at least one dedicated unit test
  - Normalisation Formula (0–100): Score = Granularity Score % [gate at 90%]
  - Execution Frequency: Every Commit / PR


### Unreachable Logic Identification

- **Metric:** Unreachable Logic Identification
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Statement Coverage · L4 Dead Code Detection
- **Excel Definition:** Identifies ghost code — lines that can never be executed regardless of input provided — measuring technical debt by finding unused functions or conditions that clutter the codebase.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Dead Code = missing_lines
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: dead_code_risk = total_bugs / total_size dead_code_score = 1 - dead_code_risk
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Unreachable Logic Ratio = (totalLines - coveredLines) / totalLines
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: UnreachableLogic = statements_skipped / statements_total
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Dead Code Ratio = (unusedFiles + unusedExports) / count(modules)
  - Raw Measurement Formula: Dead Code % = (Unexecuted Statements across all runs / Total Statements) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 2% dead code in production modules
  - Normalisation Formula (0–100): MAX(0, 100 – (Dead_Code% × 25))
  - Execution Frequency: Every Commit / PR


### Coverage Gap Analysis

- **Metric:** Coverage Gap Analysis
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Statement Coverage · L4 Test Completeness Evaluation
- **Excel Definition:** Calculates the ratio of executed lines to total lines to show exactly where the holes are in your quality, measuring the thoroughness of the testing process.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Coverage Gap = missing_lines / num_statements
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: coverage_gap = total_bugs / total_classes test_completeness_score = 1 - coverage_gap
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Coverage Gap = 1 - (coveredLines / totalLines)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: StatementCoverage = statements_covered / statements_total CoverageGap = 1 - StatementCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Coverage Gap = 1 - (statements.pct / 100)
  - Raw Measurement Formula: Gap Score = (Uncovered Statement Blocks / Total Statement Blocks) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 20% uncovered statement blocks (80% coverage floor)
  - Normalisation Formula (0–100): MAX(0, 100 – Gap_Score)
  - Execution Frequency: Every Commit / PR


### Surface-Level Correctness

- **Metric:** Surface-Level Correctness
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Statement Coverage · L4 Basic Logic Validation
- **Excel Definition:** Verifies that the code can at least run from start to finish without crashing on a fundamental level, measuring the smoke test success of your logic to ensure the most basic paths are operational.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Basic Validation = percent_covered
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: logic_error_density = total_bugs / total_size surface_correctness_score = 1 - logic_error_density
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Surface Correctness Score = coveredLines / totalLines
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SurfaceLevelCorrectness = PassedTests / TotalTests
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Test Pass Rate = passedTests / totalTests
  - Raw Measurement Formula: Smoke Pass Rate % = (Statements Executed without runtime error / Total Statements Executed) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of executed statements complete without runtime exception
  - Normalisation Formula (0–100): Score = Smoke Pass Rate % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Statement Coverage %

- **Metric:** Statement Coverage %
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Statement Coverage · L4 Code Execution Verification
- **Excel Definition:** % of executable source statements exercised by the test suite at least once
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Statement Coverage % = percent_covered
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: statement_coverage_percent = (1 - (total_bugs / total_size)) * 100
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Statement Coverage % = (coveredLines / totalLines) * 100
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: StatementCoveragePercent = statements_pct / 100
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Execution Coverage = lines.pct
  - Raw Measurement Formula: Statement Coverage % = (Statements Executed / Total Statements) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% statement coverage
  - Normalisation Formula (0–100): Score = Statement Coverage % [gate at 80%]
  - Execution Frequency: Every Commit / PR


### Boolean Accuracy Check

- **Metric:** Boolean Accuracy Check
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Branch Coverage · L4 Conditional Logic Testing
- **Excel Definition:** Evaluates the behavior of complex logical expressions to ensure they branch correctly under different data inputs, measuring the correctness of how your code handles logical operators.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Boolean Accuracy = covered_branches / num_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: boolean_risk = total_bugs / total_classes boolean_accuracy_score = 1 - boolean_risk
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Boolean Accuracy = coveredBranches / totalBranches
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: BooleanAccuracy = branches_covered / branches_total
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Branch Coverage % = branches.pct
  - Raw Measurement Formula: Boolean Coverage % = (Logical Sub-expressions evaluated True AND False / Total Sub-expressions × 2) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 70% of boolean sub-expressions fully evaluated
  - Normalisation Formula (0–100): Score = Boolean Coverage % [gate at 70%]
  - Execution Frequency: Every Commit / PR


### Sequence Integrity Mapping

- **Metric:** Sequence Integrity Mapping
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Branch Coverage · L4 Control Flow Validation
- **Excel Definition:** Verifies the physical transitions between different blocks of code to ensure execution order matches the intended design, measuring the reliability of jumps and calls within your function.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Control Flow Integrity = covered_branches - missing_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: control_flow_risk = total_bugs / total_size sequence_integrity_score = 1 - control_flow_risk
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Control Flow Integrity = coveredBranches / totalBranches
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SequenceIntegrity = functions_covered / functions_total
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Function Coverage % = functions.pct
  - Raw Measurement Formula: Sequence Pass Rate % = (Valid Execution Transitions / Total Expected Transitions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of expected code transitions execute in correct sequence
  - Normalisation Formula (0–100): Score = Sequence Pass Rate % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Iteration Boundary Verification

- **Metric:** Iteration Boundary Verification
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Branch Coverage · L4 Loop Condition Testing
- **Excel Definition:** Measures whether loops correctly handle the decision to start, continue, and terminate, specifically testing the zero-trip, one-trip, and n-trip paths.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Loop Boundary Risk = missing_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: loop_risk = total_bugs / total_size loop_boundary_score = 1 - loop_risk
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Loop Boundary Coverage = coveredBranches / totalBranches
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: IterationBoundaryVerification = branches_covered / branches_total
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Branch Hit Ratio = BRH / BRF
  - Raw Measurement Formula: Loop Boundary Coverage % = (Loop Paths Tested: zero-trip + one-trip + n-trip / Total Loop Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of loops tested for zero-trip, one-trip, and n-trip paths
  - Normalisation Formula (0–100): Score = Loop Boundary Coverage % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Boundary Failure Identification

- **Metric:** Boundary Failure Identification
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Branch Coverage · L4 Edge Case Detection
- **Excel Definition:** Identifies logical failures at the extreme limits of input values such as empty lists or None types, measuring the robustness of your branches when they encounter unexpected data.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Edge Case Risk = missing_branches / num_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: boundary_failure_risk = total_bugs / total_classes boundary_detection_score = 1 - boundary_failure_risk
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Boundary Risk = (totalBranches - coveredBranches) / totalBranches
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: BoundaryFailureRate = FailedTests / TotalTests
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Failure Discovery Rate = failedRuns / totalRuns
  - Raw Measurement Formula: Edge Failure Rate % = (Branch failures at boundary inputs / Total Boundary Input Tests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 1% edge case branch failures
  - Normalisation Formula (0–100): MAX(0, 100 – (Edge_Failure_Rate% × 50))
  - Execution Frequency: Every Commit / PR


### Branch Misdirection Discovery

- **Metric:** Branch Misdirection Discovery
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Branch Coverage · L4 Logic Error Detection
- **Excel Definition:** Uncovers flaws where the code takes the wrong turn due to a mistake in the conditional expression, acting as a diagnostic tool for semantic bugs that produce the wrong result without crashing.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Logic Error Proxy = missing_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: branch_error_risk = total_bugs / total_size branch_accuracy_score = 1 - branch_error_risk
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Branch Risk Score = (totalBranches - coveredBranches) / totalBranches
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: BranchMisdirection = 1 - (branches_covered / branches_total)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Lint Error Density = (errorCount + fatalErrorCount) / totalFiles
  - Raw Measurement Formula: Misdirection Count = Count(Branches producing incorrect output despite execution)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 branches producing semantically incorrect results
  - Normalisation Formula (0–100): MAX(0, 100 – (Misdirection_Count × 20))
  - Execution Frequency: Every Commit / PR


### Decision Coverage Gap Analysis

- **Metric:** Decision Coverage Gap Analysis
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Branch Coverage · L4 Test Case Completeness
- **Excel Definition:** Calculates the percentage of successfully executed branches versus total available decision points, showing exactly which else or elif blocks have never been visited by your tests.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Branch Coverage Gap = missing_branches / num_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: decision_gap = total_bugs / total_classes decision_coverage_score = 1 - decision_gap
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Decision Coverage Gap = 1 - (coveredBranches / totalBranches)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DecisionCoverageGap = 1 - (branches_covered / branches_total)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Branch Gap = 1 - (branches.pct / 100)
  - Raw Measurement Formula: Decision Gap % = (Untested Decision Branches / Total Decision Branches) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 30% decision branches untested (70% branch coverage floor)
  - Normalisation Formula (0–100): MAX(0, 100 – Decision_Gap%)
  - Execution Frequency: Every Commit / PR


### Branch Coverage %

- **Metric:** Branch Coverage %
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Branch Coverage · L4 Decision Outcome Verification
- **Excel Definition:** % of true/false branches of every decision point executed by tests
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Branch Coverage % = percent_branches_covered
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: branch_coverage_percent = (1 - (total_bugs / total_classes)) * 100
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: Branch Coverage % = (coveredBranches / totalBranches) * 100
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: BranchCoveragePercent = branches_pct / 100
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Decision Coverage = branches.pct
  - Raw Measurement Formula: Branch Coverage % = (Branches Hit / Total Branches) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 70% branch coverage
  - Normalisation Formula (0–100): Score = Branch Coverage % [gate at 70%]
  - Execution Frequency: Every Commit / PR


### Retrieving data. Wait a few seconds and try to cut or copy again.

- **Metric:** Retrieving data. Wait a few seconds and try to cut or copy again.
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Path Execution Tracking
- **Excel Definition:** Records exactly which sequences of statements and branches are hit during active testing, providing physical proof of which routes in the code were traveled by a specific test case.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Path Execution Proxy = covered_lines + covered_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Runtime Journey Monitoring = covered_instructions / (covered_instructions + missed_instructions)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: RuntimePathCount = UniquePaths ExecutionVolume = TotalTraces
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: RuntimeJourneyMonitoring = branches_covered / branches_total
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Average Execution Time = sum(span.duration) / count(spans)
  - Raw Measurement Formula: Execution Trace Coverage % = (Recorded Execution Paths / Total Feasible Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 60% of feasible paths traced during test runs
  - Normalisation Formula (0–100): Score = Execution Trace Coverage % [gate at 60%]
  - Execution Frequency: Every Commit / PR


### Full Logic Validation

- **Metric:** Full Logic Validation
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Complete Coverage Path Verification
- **Excel Definition:** Confirms that every unique path identified by the complexity metric has been successfully traveled, measuring completeness of the test suite against the theoretical maximum of the code logic.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Full Path Coverage Proxy = percent_covered * percent_branches_covered
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Full Logic Validation = covered_branches / (covered_branches + missed_branches)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: FullLogicCoverage = coveredSeq / totalSeq
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: FullLogicValidation = ( (branches_covered / branches_total) + (functions_covered / functions_total) + (statements_covered / statements_total) ) / 3
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Full Coverage Score = (statements.pct + branches.pct + functions.pct + lines.pct) / 4
  - Raw Measurement Formula: Full Path Validation % = (Paths fully traversed start-to-end / Total CC-derived Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of CC-derived paths traversed at least once
  - Normalisation Formula (0–100): Score = Full Path Validation % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Gap Identification

- **Metric:** Gap Identification
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Partial Path Coverage Detection
- **Excel Definition:** Highlights logical routes that have only been partially tested or completely ignored, acting as a diagnostic tool to find blind spots where complex logic remains unverified.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Path Gap = missing_lines + missing_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Gap Identification = missed_branches / (covered_branches + missed_branches)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: PathCoverageGap = 1 - (coveredBranches / totalBranches)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: GapIdentification = 1 - (branches_covered / branches_total)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Partial Coverage Ratio = (statements.total - statements.covered) / statements.total
  - Raw Measurement Formula: Path Gap % = (Partially or untested paths / Total Feasible Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 40% of paths untested (60% floor)
  - Normalisation Formula (0–100): MAX(0, 100 – Path_Gap%)
  - Execution Frequency: Every Commit / PR


### Deep Logic Probing

- **Metric:** Deep Logic Probing
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Nested Condition Path Testing
- **Excel Definition:** Measures system stability when multiple decision points are stacked within one another, ensuring the deepest levels of code hierarchy are reachable and functional.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Nested Logic Risk = num_branches - covered_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Deep Logic Probing = covered_branches / (covered_branches + missed_branches)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DeepPathCoverage = coveredBranches / totalBranches
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: TotalMutants = count(mutants) KilledMutants = count(mutants where status == "Killed") DeepLogicProbing = KilledMutants / TotalMutants
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Complex Branch Ratio = branches.total / functions.total
  - Raw Measurement Formula: Nesting Depth Coverage % = (Paths through nesting depth > 3 tested / Total Deep Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 60% of deeply nested paths (depth > 3) exercised
  - Normalisation Formula (0–100): Score = Nesting Depth Coverage % [gate at 60%]
  - Execution Frequency: Every Commit / PR


### Iterative Route Analysis

- **Metric:** Iterative Route Analysis
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Loop Path Detection
- **Excel Definition:** Specifically focuses on paths that enter, repeat, or skip loops entirely, measuring how the code handles different iteration counts including the zero-trip path where the loop never executes.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Loop Path Risk = missing_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Iterative Route Analysis = covered_complexity / (covered_complexity + missed_complexity)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: LoopExecutionFrequency = SpanCount / TotalTraces
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: IterativeRouteAnalysis = branches_covered / branches_total
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Loop Path Risk = count(branches where covered < total) / branches.total
  - Raw Measurement Formula: Loop Path Coverage % = (Zero-trip + One-trip + N-trip paths tested / Total Loop Path variants) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of loop variants (zero, one, n-trip) tested per loop
  - Normalisation Formula (0–100): Score = Loop Path Coverage % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Ghost Code Discovery

- **Metric:** Ghost Code Discovery
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Unreachable Path Detection
- **Excel Definition:** Identifies logical routes that can never be executed due to contradictory conditions or dead code, reducing technical debt by pointing out logic that clutters the system without providing value.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Unreachable Paths = missing_lines + missing_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Ghost Code Discovery = missed_instructions / (covered_instructions + missed_instructions)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: UnreachablePathRatio = (totalSeq - coveredSeq) / totalSeq
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SurvivedMutants = count(mutants where status == "Survived") GhostCodeDiscovery = SurvivedMutants / TotalMutants
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Unreachable Code Ratio = (unusedFiles + uncoveredLines) / totalFiles
  - Raw Measurement Formula: Ghost Path % = (Logically unreachable paths detected / Total Declared Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 2% of declared paths are unreachable (ghost code)
  - Normalisation Formula (0–100): MAX(0, 100 – (Ghost_Path% × 25))
  - Execution Frequency: Every Commit / PR


### Error Flow Verification

- **Metric:** Error Flow Verification
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Exception Path Handling
- **Excel Definition:** Measures the code ability to gracefully handle and recover from unexpected errors or try-except blocks, ensuring the system does not crash when forced into a failure state.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Error Path Risk = missing_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Error Flow Verification = covered_branches / (covered_branches + missed_branches)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: ErrorPathRatio = ErrorSpans / TotalSpans
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: ErrorFlowVerification = failures / tests
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Error Handling Issue Density = count(messages where ruleId relates to error-handling) / totalFiles
  - Raw Measurement Formula: Exception Path Coverage % = (Exception / Error paths exercised / Total Exception Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% of exception paths exercised in test suite
  - Normalisation Formula (0–100): Score = Exception Path Coverage % [gate at 80%]
  - Execution Frequency: Every Commit / PR


### Cross-Component Mapping

- **Metric:** Cross-Component Mapping
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Multi-Function Path Tracking
- **Excel Definition:** Tracks how logic flows across multiple functions or modules to complete a single task, measuring integration quality between different parts of the application.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Cross Function Coverage = sum(covered_lines across functions)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Cross-Component Mapping = covered_methods / (covered_methods + missed_methods)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: CrossComponentPathDepth = AvgSpanDepth
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: OrphanModules = count(modules where is_orphan == true) TotalModules = total modules CrossComponentMapping = 1 - (OrphanModules / TotalModules)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Dependency Interaction Density = count(modules with dependencies) / count(modules)
  - Raw Measurement Formula: Integration Path Coverage % = (Cross-function paths tested / Total Cross-function paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 60% of cross-component paths verified
  - Normalisation Formula (0–100): Score = Integration Path Coverage % [gate at 60%]
  - Execution Frequency: Every Commit / PR


### Automated Quality Enforcement

- **Metric:** Automated Quality Enforcement
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 CI/CD Integration Test
- **Excel Definition:** Embeds path verification into the automated pipeline to prevent untested logic from reaching production, measuring compliance with quality standards in real-time.
- **Required Evidence:** Static analysis configuration and clean findings
- **Project Component Producing Evidence:** Roslyn (`Directory.Build.props`, `.editorconfig`), ESLint `eslint.config.js`
- **Source of Evidence:** Build and `npm run lint`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Automation Readiness = percent_covered + percent_branches_covered
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Automated Quality Enforcement = ( (covered_instructions / total_instructions) + (covered_branches / total_branches) + (covered_methods / total_methods) ) / 3
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: CoverageGate = (coveredSeq / totalSeq) >= Threshold ? 1 : 0
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AutomatedQualityEnforcement = failures > 0 ? 1 : 0
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Quality Gate Score = 1 - ((errorCount + fatalErrorCount) / totalIssues)
  - Raw Measurement Formula: Pipeline Path Gate % = (Builds passing path coverage gate / Total Builds) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of builds pass minimum 60% path coverage gate
  - Normalisation Formula (0–100): Score = Pipeline Path Gate % [gate at 100%]
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires CI build history.


### Path Coverage %

- **Metric:** Path Coverage %
- **Metric Category:** L1 White Box · L2 Control Flow Testing · L3 Path Coverage · L4 Path Detection Testing
- **Excel Definition:** % of all distinct execution paths through a function that are exercised by the test suite
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Path Coverage % (proxy) = (percent_covered + percent_branches_covered) / 2
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Path Coverage % = (covered_branches / (covered_branches + missed_branches)) * 100
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: PathCoveragePercent = (coveredBranches / totalBranches) * 100
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: PathCoveragePercent = branches_covered / branches_total
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Path Coverage Proxy = branches.pct
  - Raw Measurement Formula: Path Coverage % = (Paths Exercised / Total Feasible Paths) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 60% path coverage (complex functions)
  - Normalisation Formula (0–100): Score = Path Coverage % [gate at 60%]
  - Execution Frequency: Every Commit / PR


### Logic Error Sensitivity

- **Metric:** Logic Error Sensitivity
- **Metric Category:** L1 White Box · L2 Mutation Testing · L3 Mutation Score · L4 Fault Detection Capability
- **Excel Definition:** Evaluates the ability of tests to fail when the underlying code logic is altered, measuring whether tests are sensitive enough to catch subtle mistakes that structural coverage might miss.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Logic Error Sensitivity = (total jobs - surviving mutants) / total jobs
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Logic Error Sensitivity = killed_mutations / total_mutations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: LogicErrorSensitivity = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: LogicErrorSensitivity = KilledMutants / TotalMutants
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Fault Detection Capability = count(mutants where status == "Killed") / count(mutants)
  - Raw Measurement Formula: Sensitivity Score = (Survived Mutants caught by assertion-only tests / Total Survived Mutants) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 70% of survived mutants caught by targeted assertion tests
  - Normalisation Formula (0–100): Score = Sensitivity Score % [gate at 70%]
  - Execution Frequency: Daily / Per Sprint


### Test Rigor Assessment

- **Metric:** Test Rigor Assessment
- **Metric Category:** L1 White Box · L2 Mutation Testing · L3 Mutation Score · L4 Test Coverage Quality Validation
- **Excel Definition:** Serves as a meta-metric that validates the quality of your existing coverage. While statement coverage tells you if a line was executed, this measures how well that line was actually tested.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Test Rigor = (total jobs - surviving mutants) / total jobs
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Test Rigor Assessment = covered_mutations / total_mutations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: TestRigorScore = killedMutants / (killedMutants + survivedMutants)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: TestRigorAssessment = KilledMutants / CoveredMutants
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Test Rigor Assessment = count(mutants where status == "Killed") / (count(mutants where status == "Killed") + count(mutants where status == "Survived"))
  - Raw Measurement Formula: Rigor Score = (Killed Mutants / Total Mutants Generated) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 70% overall mutant kill rate
  - Normalisation Formula (0–100): Score = Rigor Score % [gate at 70%]
  - Execution Frequency: Daily / Per Sprint


### Weak Spot Localization

- **Metric:** Weak Spot Localization
- **Metric Category:** L1 White Box · L2 Mutation Testing · L3 Mutation Score · L4 Test Case Improvement Identification
- **Excel Definition:** Pinpoints exactly which surviving mutants were not caught, identifying specific areas where test cases need to be added or strengthened to write more meaningful assertions.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Weak Spots = surviving mutants
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Weak Spot Localization = survived_mutations / total_mutations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: WeakSpotRatio = survivedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: WeakSpotLocalization = SurvivedMutants / TotalMutants
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Weak Spot Localization = count(mutants where status == "Survived") / count(mutants)
  - Raw Measurement Formula: Weak Spot Count = Count(Modules where Mutation Kill Rate < 50%)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 modules with mutation kill rate below 50%
  - Normalisation Formula (0–100): MAX(0, 100 – (Weak_Spot_Count × 15))
  - Execution Frequency: Daily / Per Sprint


### Boundary Mutant Analysis

- **Metric:** Boundary Mutant Analysis
- **Metric Category:** L1 White Box · L2 Mutation Testing · L3 Mutation Score · L4 Edge Case Detection
- **Excel Definition:** Measures the test suite ability to catch errors at boundaries by specifically mutating operators, identifying if tests are robust enough to handle the precise limits of the code logic.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Boundary Weakness = count(TestOutcome.SURVIVED)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Boundary Mutant Analysis = count( mutation where mutator contains "Conditionals" ) / total_mutations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: BoundaryWeakness = survivedMutants / (killedMutants + survivedMutants)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: BoundaryMutantAnalysis = SurvivedMutants / CoveredMutants
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Boundary Mutant Analysis = count(mutants where mutatorName contains "boundary") / count(mutants)
  - Raw Measurement Formula: Boundary Kill Rate % = (Boundary Operator Mutants Killed / Total Boundary Mutants) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% of boundary operator mutants killed
  - Normalisation Formula (0–100): Score = Boundary Kill Rate % [gate at 80%]
  - Execution Frequency: Daily / Per Sprint


### Logic Error Sensitivity

- **Metric:** Logic Error Sensitivity
- **Metric Category:** L1 White Box · L2 Mutation Testing · L3 Mutation Score · L4 Fault Detection Capability
- **Excel Definition:** Evaluates the ability of tests to fail when the underlying code logic is altered, measuring whether tests are sensitive enough to catch subtle mistakes that structural coverage might miss.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Logic Error Sensitivity = (total jobs - surviving mutants) / total jobs
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Change Resilience Testing = killed_mutations / covered_mutations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: RegressionResilience = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: ChangeResilienceTesting = KilledMutants / (KilledMutants + SurvivedMutants)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Change Resilience Score = count(mutants where status == "Killed") / count(mutants where testsCompleted > 0)
  - Raw Measurement Formula: Resilience Score = (Post-Change Mutation Kill Rate / Pre-Change Mutation Kill Rate) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: Resilience Score >= 95% (< 5% kill rate degradation after change)
  - Normalisation Formula (0–100): Score = MIN(100, Resilience_Score)
  - Execution Frequency: Daily / Per Sprint


### Test Rigor Assessment

- **Metric:** Test Rigor Assessment
- **Metric Category:** L1 White Box · L2 Mutation Testing · L3 Mutation Score · L4 Test Coverage Quality Validation
- **Excel Definition:** Serves as a meta-metric that validates the quality of your existing coverage. While statement coverage tells you if a line was executed, this measures how well that line was actually tested.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Test Rigor = (total jobs - surviving mutants) / total jobs
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Semantic Integrity Check = killed_mutations / total_mutations
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: SemanticIntegrity = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SemanticIntegrityCheck = 1 - (SurvivedMutants / TotalMutants)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Semantic Integrity Check = count(mutants where status == "Killed") / count(mutants)
  - Raw Measurement Formula: Semantic Pass Rate % = (Mutants testing semantic behavior killed / Total Semantic Mutants) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 75% semantic mutant kill rate
  - Normalisation Formula (0–100): Score = Semantic Pass Rate % [gate at 75%]
  - Execution Frequency: Daily / Per Sprint


### Logic Error Sensitivity

- **Metric:** Logic Error Sensitivity
- **Metric Category:** L1 White Box · L2 Mutation Testing · L3 Mutation Score · L4 Fault Detection Capability
- **Excel Definition:** Evaluates the ability of tests to fail when the underlying code logic is altered, measuring whether tests are sensitive enough to catch subtle mistakes that structural coverage might miss.
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Logic Error Sensitivity = (total jobs - surviving mutants) / total jobs
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Mutation Kill Rate % = (killed_mutations / total_mutations) * 100
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: MutationKillRatePercent = (killedMutants / totalMutants) * 100 NoCoverageRatio = noCoverageMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: MutationKillRate = KilledMutants / TotalMutants
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Mutation Kill Rate % = count(mutants where status == "Killed") / count(mutants)
  - Raw Measurement Formula: Mutation Kill Rate % = (Killed Mutants / Total Non-Equivalent Mutants) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 70% mutation kill rate
  - Normalisation Formula (0–100): Score = Mutation Kill Rate % [gate at 70%]
  - Execution Frequency: Daily / Per Sprint


### Coverage Delta %

- **Metric:** Coverage Delta %
- **Metric Category:** L1 White Box · L2 Test Regression/Coverage Analysis · L3 Coverage Delta · L4 Regression Testing Monitoring
- **Excel Definition:** Change in test coverage percentage between current build and previous baseline — monitors coverage trends
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Coverage Delta % = current.percent_covered - previous.percent_covered
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Coverage Delta % = ( (current_covered_lines / (current_covered_lines + current_missed_lines)) - (baseline_covered_lines / (baseline_covered_lines + baseline_missed_lines)) ) * 100
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: BranchCoverageDeltaPercent = ((coveredBranches_current / totalBranches_current) - (coveredBranches_prev / totalBranches_prev)) * 100
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: CoverageDeltaPercent = total_coverage_percent / 100
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Coverage Delta % = new_statements.pct - old_statements.pct
  - Raw Measurement Formula: Coverage Delta = Coverage_Current% – Coverage_Previous%
- **Expected Positive Condition:**
  - Expected Value / Threshold: Delta >= 0% (no coverage regression); flag if delta < -2%
  - Normalisation Formula (0–100): MAX(0, 100 + (Delta × 10)) [capped 0–100]
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires a previous baseline measurement, which a new repository does not have.
  - Requires a previous measurement, which a new repository does not have.


### Discovery Power Assessment

- **Metric:** Discovery Power Assessment
- **Metric Category:** L1 White Box · L2 Test Regression/Coverage Analysis · L3 Coverage Delta · L4 Test Suite Effectiveness Tracking
- **Excel Definition:** Evaluates how many unique logical journeys through the code are validated by current test cases, measuring the ratio of executed paths to total possible paths.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Discovery Power = (previous.missing_lines - current.missing_lines)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Discovery Power Assessment = ( (covered_lines / (covered_lines + missed_lines)) + (covered_branches / (covered_branches + missed_branches)) ) / 2
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DiscoveryPower = coveredBranches / totalBranches
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DiscoveryPower = covered_lines / (covered_lines + uncovered_lines)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Discovery Power Assessment = (statements.covered + branches.covered) / (statements.total + branches.total)
  - Raw Measurement Formula: Discovery Power % = (New Code Lines Covered by Tests / Total New Code Lines Added) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% of new code lines covered by test suite
  - Normalisation Formula (0–100): Score = Discovery Power % [gate at 80%]
  - Execution Frequency: Every Commit / PR


### Deployment Readiness Guard

- **Metric:** Deployment Readiness Guard
- **Metric Category:** L1 White Box · L2 Test Regression/Coverage Analysis · L3 Coverage Delta · L4 CI/CD Quality Gate Enforcement
- **Excel Definition:** Acts as an automated threshold that prevents code from being merged if the coverage delta is negative, ensuring that only logically verified code moves toward production.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Quality Gate = IF current.percent_covered >= threshold AND current.percent_branches_covered >= threshold THEN PASS ELSE FAIL
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Deployment Readiness Guard = ( (covered_lines / total_lines >= line_threshold) AND (covered_branches / total_branches >= branch_threshold) )
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: CoverageGate = (coveredLines / totalLines) >= Threshold ? 1 : 0
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DeploymentReadinessGuard = total_coverage_percent > 0 ? 1 : 0
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Deployment Readiness Guard = (statements.pct + branches.pct + functions.pct + lines.pct) / 4
  - Raw Measurement Formula: Gate Compliance % = (Builds with Coverage Delta >= 0 / Total Builds) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of builds show non-negative coverage delta
  - Normalisation Formula (0–100): Score = Gate Compliance % [gate at 100%]
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires CI build history.


### Ripple Effect Mapping

- **Metric:** Ripple Effect Mapping
- **Metric Category:** L1 White Box · L2 Test Regression/Coverage Analysis · L3 Coverage Delta · L4 Change Impact Analysis
- **Excel Definition:** Identifies which specific logical paths are altered by a code change and which downstream paths might be affected, measuring the logical surface area of a modification to predict unintended side effects.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Impact Ratio = |Coverage Delta %| / previous.percent_covered
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Ripple Effect Mapping = covered_changed_lines / total_changed_lines
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: RippleEffect = abs((coveredBranches_current / totalBranches_current) - (coveredBranches_prev / totalBranches_prev))
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: RippleEffectMapping = uncovered_lines / added_lines
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Ripple Effect Mapping = (changed_statements.covered - unchanged_statements.covered) / statements.total
  - Raw Measurement Formula: Ripple Coverage % = (Downstream Modules re-tested after coverage delta trigger / Total Affected Modules) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of affected downstream modules re-verified
  - Normalisation Formula (0–100): Score = Ripple Coverage % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Fresh Logic Proofing

- **Metric:** Fresh Logic Proofing
- **Metric Category:** L1 White Box · L2 Test Regression/Coverage Analysis · L3 Coverage Delta · L4 New Code Testing Validation
- **Excel Definition:** Specifically measures whether newly added lines or paths have corresponding test cases, ensuring that the MVP growth is supported by an equal growth in testing depth.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: New Code Coverage = current.covered_lines - previous.covered_lines
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Fresh Logic Proofing = covered_new_lines / total_new_lines
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: FreshLogicCoverage = (totalLines_current - totalLines_prev) > 0 ? ((coveredLines_current - coveredLines_prev) / (totalLines_current - totalLines_prev)) : 0
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: FreshLogicProofing = covered_lines / added_lines
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Fresh Logic Proofing = new_statements.covered / new_statements.total
  - Raw Measurement Formula: Fresh Coverage % = (New/Modified Functions with test coverage / Total New/Modified Functions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of new or modified functions have test coverage
  - Normalisation Formula (0–100): Score = Fresh Coverage % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Structural Health Benchmarking

- **Metric:** Structural Health Benchmarking
- **Metric Category:** L1 White Box · L2 Test Regression/Coverage Analysis · L3 Coverage Delta · L4 Quality Improvement Measurement
- **Excel Definition:** Provides a quantitative score of how much simpler or safer the code becomes after refactoring, measuring the successful reduction of complex untestable paths into cleaner sequences.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: Quality Improvement = (Coverage Delta % + Branch Delta %) / 2
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Structural Health Benchmarking = ( current_line_coverage - baseline_line_coverage + current_branch_coverage - baseline_branch_coverage + current_method_coverage - baseline_method_coverage ) / 3
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: StructuralHealthScore = (LineCoverage + BranchCoverage + MethodCoverage) / 3
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: StructuralHealthBenchmarking = 1 - (total_violations / total_files_measured)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Structural Health Benchmarking = (statements.pct + branches.pct) / 2
  - Raw Measurement Formula: Health Benchmark Score = AVG(Coverage Delta) over rolling 5-sprint window
- **Expected Positive Condition:**
  - Expected Value / Threshold: AVG rolling delta >= +1% (continuous improvement trend)
  - Normalisation Formula (0–100): MAX(0, 100 + (AVG_Rolling_Delta × 10)) capped 0–100
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires a previous baseline measurement, which a new repository does not have.
  - Requires historical sprint data, which a new repository does not have.


### All-Defs Coverage %

- **Metric:** All-Defs Coverage %
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Definition Coverage · L4 Variable Definition Detection
- **Excel Definition:** % of variable definition points (where variables are assigned) that are exercised by at least one test path
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: all_defs_coverage = (used_definitions / max(definitions, 1)) * 100
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: All-Defs Coverage % = (count(definitions with at least one covered use) / total_definitions) * 100
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: AllDefsCoverage = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AllDefsCoverage = 1 - (UnusedVarViolations / TotalViolations)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: All-Defs Coverage % = count(definitions with referenceCount > 0) / total_definitions
  - Raw Measurement Formula: All-Defs Coverage % = (Definition Points Covered / Total Definition Points) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 75% all-definition coverage
  - Normalisation Formula (0–100): Score = All-Defs Coverage %
  - Execution Frequency: Daily / Per Sprint


### Data Path Correlation

- **Metric:** Data Path Correlation
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Definition Coverage · L4 Definition-Use Mapping
- **Excel Definition:** Creates a bridge between where a variable is defined and at least one line of code where that specific value is later read or modified, measuring the integrity of the data journey.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: data_path_correlation = used_definitions / max(definitions, 1)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Data Path Correlation = count(covered_du_pairs) / total_du_pairs
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DataPathCorrelation = killedMutants / (killedMutants + survivedMutants)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DataPathCorrelation = 1 - (UndefinedVarViolations / TotalViolations)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Data Path Correlation = count(definitions with same name appearing across multiple locations) / total_definitions
  - Raw Measurement Formula: DU Correlation % = (Definitions linked to at least one Use / Total Definitions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 75% of definitions linked to a verified use path
  - Normalisation Formula (0–100): Score = DU Correlation % [gate at 75%]
  - Execution Frequency: Daily / Per Sprint


### DU-Path Validation

- **Metric:** DU-Path Validation
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Definition Coverage · L4 Coverage Measurement
- **Excel Definition:** Calculates the percentage of Definition-Use pairs that have been successfully exercised by your test suite, providing a deeper quality check than just seeing if a line was executed.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: du_path_validation = covered_lines / max(num_statements, 1)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: DU-Path Validation = count(covered_du_paths) / total_du_paths
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DUPathValidation = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DUPathValidation = (StatementCoverage + BranchCoverage) / 2
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: DU-Path Validation = count(definitions with referenceCount > 0) / total_definitions
  - Raw Measurement Formula: DU-Path Coverage % = (DU Pairs Exercised / Total DU Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 65% of all DU pairs exercised by test suite
  - Normalisation Formula (0–100): Score = DU-Path Coverage % [gate at 65%]
  - Execution Frequency: Daily / Per Sprint


### Dead Data Identification

- **Metric:** Dead Data Identification
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Definition Coverage · L4 Uncovered Definition Detection
- **Excel Definition:** Identifies variables that are assigned a value but are never actually used by the program, helping reduce technical debt by spotting zombie variables that clutter memory without purpose.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: dead_data_count = unused_variable + unused_import
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Dead Data Identification = count(definitions with no covered use) / total_definitions
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DeadDataRatio = noCoverageMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DeadDataIdentification = UnusedVarViolations / TotalViolations
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Dead Data Identification = count(definitions with referenceCount == 0) / total_definitions
  - Raw Measurement Formula: Dead Variable % = (Definitions with no reachable Use / Total Definitions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 5% dead variable definitions in codebase
  - Normalisation Formula (0–100): MAX(0, 100 – (Dead_Variable% × 10))
  - Execution Frequency: Daily / Per Sprint


### Null and Boundary Flow Analysis

- **Metric:** Null and Boundary Flow Analysis
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Definition Coverage · L4 Edge Case Handling
- **Excel Definition:** Measures how the data flow reacts when a definition results in an unexpected state such as None or an overflow, identifying risks where valid variable definitions might lead to a crash at use.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: boundary_issue_ratio = ( functions_with_counterexample / max(total_functions_checked, 1) )
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Null and Boundary Flow Analysis = count(du_pairs where (cb + mb > 0) AND mb > 0) / total_du_pairs
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: BoundaryDataWeakness = survivedMutants / (killedMutants + survivedMutants)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: NullBoundaryFlowAnalysis = 1 - BranchCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Null/Boundary Analysis Proxy = count(definitions where type includes "null" OR "undefined") / total_definitions
  - Raw Measurement Formula: Null Flow Risk Score = Count(Definitions reaching a Use with no null/boundary guard) × 10
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 unguarded null or boundary flows from definition to use
  - Normalisation Formula (0–100): MAX(0, 100 – Null_Flow_Risk_Score)
  - Execution Frequency: Daily / Per Sprint


### Audit Trail Verification

- **Metric:** Audit Trail Verification
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Definition Coverage · L4 Reporting Validation
- **Excel Definition:** Provides a verifiable report of the variable state at every stage of execution for debugging and compliance, ensuring that any data-related failure can be traced back to its specific definition point.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: audit_activity = insertions + deletions
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Audit Trail Verification = (count(definitions) == count(unique(definition_ids_in_report))) AND (report_timestamp exists)
  - C# :: Metrics emitted directly: Yes
  - C# :: Derivation: DataFlowAuditScore = totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AuditTrailVerification = messages.length > 0 ? 1 : 0
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Audit Trail Completeness = count(definitions with valid line and type metadata) / total_definitions
  - Raw Measurement Formula: Audit Completeness % = (Variable state transitions logged and traceable / Total DU pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 90% of DU pairs have traceable execution audit record
  - Normalisation Formula (0–100): Score = Audit Completeness % [gate at 90%]
  - Execution Frequency: Daily / Per Sprint


### Data Processing Validation

- **Metric:** Data Processing Validation
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Computational Use Detection (C-Use)
- **Excel Definition:** Identifies instances where a variable is used in a calculation or output statement, measuring the accuracy of data transformations and ensuring variable values correctly influence final results.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: c_use = len([d for d in definitions if d.user_count > 0])
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Data Processing Validation = count(covered_c_uses) / total_c_uses
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: CUseValidation = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DataProcessingValidation = StatementCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: C-Use Coverage = count(definitions where kind == "variable" AND used in computation) / total_definitions
  - Raw Measurement Formula: C-Use Coverage % = (Computational DU Pairs Exercised / Total C-Use Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 65% of computational use pairs exercised
  - Normalisation Formula (0–100): Score = C-Use Coverage % [gate at 65%]
  - Execution Frequency: Daily / Per Sprint


### Logic Influence Assessment

- **Metric:** Logic Influence Assessment
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Predicate Use Detection (P-Use)
- **Excel Definition:** Identifies where a variable is used to determine the outcome of a decision, measuring how data values control the program execution flow and ensuring both True and False outcomes are tested.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: p_use = covered_branches
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Logic Influence Assessment = count(covered_p_uses) / total_p_uses
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: PUseValidation = killedMutants / (killedMutants + survivedMutants)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: LogicInfluenceAssessment = BranchCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: P-Use Coverage = count(definitions where kind == "parameter" OR used in condition) / total_definitions
  - Raw Measurement Formula: P-Use Coverage % = (Predicate DU Pairs Exercised / Total P-Use Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 65% of predicate use pairs exercised (True + False outcomes)
  - Normalisation Formula (0–100): Score = P-Use Coverage % [gate at 65%]
  - Execution Frequency: Daily / Per Sprint


### Path Correlation Mapping

- **Metric:** Path Correlation Mapping
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Definition-Use Pair Identification
- **Excel Definition:** Links every definition of a variable to all possible locations where that specific value could be read, creating a map of data influence across the function to ensure no use-case is left untested.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: def_use_pairs = sum(d.user_count for d in definitions)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Path Correlation Mapping = count(covered_du_pairs) / total_du_pairs
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DUCorrelation = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: PathCorrelationMapping = (StatementCoverage + BranchCoverage + FunctionCoverage) / 3
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Definition-Use Pair Coverage = count(unique (definition → usage) pairs) / total_definitions
  - Raw Measurement Formula: DU Pair Map Coverage % = (Identified and Mapped DU Pairs / Total Estimated DU Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 90% of DU pairs identified and mapped in analysis
  - Normalisation Formula (0–100): Score = DU Pair Map Coverage % [gate at 90%]
  - Execution Frequency: Daily / Per Sprint


### Comprehensive Data Proofing

- **Metric:** Comprehensive Data Proofing
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 All-Uses Coverage Verification
- **Excel Definition:** Confirms that every single identified Definition-Use pair (both c-use and p-use) has been executed at least once, measuring the total thoroughness of the test suite regarding data integrity.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: all_uses_coverage = covered_lines / num_statements
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Comprehensive Data Proofing = count(covered_du_pairs) / total_du_pairs
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: AllUsesCoverage = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AllUsesCoverage = (StatementCoverage + BranchCoverage) / 2
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: All-Uses Coverage % = count(definitions with referenceCount > 0) / total_definitions
  - Raw Measurement Formula: Full DU Verification % = (DU Pairs with both C-Use and P-Use tested / Total DU Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 65% of DU pairs fully verified for both use types
  - Normalisation Formula (0–100): Score = Full DU Verification % [gate at 65%]
  - Execution Frequency: Daily / Per Sprint


### Data Flow Gap Analysis

- **Metric:** Data Flow Gap Analysis
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Partial Uses Coverage Detection
- **Excel Definition:** Highlights specific variable uses that have never been reached during testing, helping developers find blind spots where a variable is defined but its impact on a calculation or branch is unverified.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: partial_uses = len([d for d in definitions if d.user_count == 0]) + len(missing_lines)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Data Flow Gap Analysis = count(partially_covered_du_pairs) / total_du_pairs
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DataFlowGap = 1 - (killedMutants / totalMutants)
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DataFlowGapAnalysis = 1 - AllUsesCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Data Flow Gap Analysis = count(definitions with referenceCount == 0) / total_definitions
  - Raw Measurement Formula: Gap Score = (DU Pairs with no test exercise / Total DU Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 35% of DU pairs untested (65% coverage floor)
  - Normalisation Formula (0–100): MAX(0, 100 – Gap_Score)
  - Execution Frequency: Daily / Per Sprint


### Ambiguity Resolution

- **Metric:** Ambiguity Resolution
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Multiple Definitions Handling
- **Excel Definition:** Tracks variables that are redefined multiple times such as inside a loop or multiple if blocks, measuring the complexity of the data lifecycle to ensure the current value is always the intended one.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: multiple_definitions = len([d for d in definitions if d.user_count > 1])
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Ambiguity Resolution = count(uses linked to multiple definitions AND covered) / total_uses_with_multiple_defs
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: AmbiguityRisk = survivedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AmbiguityResolution = 1 - (UnusedVarViolations / TotalViolations)
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Ambiguity Resolution Score = count(definitions with unique name and type) / total_definitions
  - Raw Measurement Formula: Redefinition Coverage % = (Redefined Variables with all active definitions tested / Total Redefined Variables) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% of redefined variables tested across all definition states
  - Normalisation Formula (0–100): Score = Redefinition Coverage % [gate at 80%]
  - Execution Frequency: Daily / Per Sprint


### Inter-procedural Tracking

- **Metric:** Inter-procedural Tracking
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Cross-Function Use Detection
- **Excel Definition:** Monitors how variables such as arguments or global objects move between different functions, measuring the safety of the interfaces between code modules to prevent data corruption during hand-offs.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: cross_function_uses = len([ d for d in definitions if len(d.users) > 1 ])
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Inter-procedural Tracking = count(covered_interprocedural_du_pairs) / total_interprocedural_du_pairs
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: InterProceduralCoverage = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: InterProceduralTracking = FunctionCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Inter-Procedural Tracking = count(definitions used across multiple functions/files) / total_definitions
  - Raw Measurement Formula: Inter-procedural Coverage % = (Cross-function DU Pairs Exercised / Total Cross-function DU Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 60% of cross-function DU pairs exercised
  - Normalisation Formula (0–100): Score = Inter-procedural Coverage % [gate at 60%]
  - Execution Frequency: Daily / Per Sprint


### Ghost Use Identification

- **Metric:** Ghost Use Identification
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Unreachable Use Detection
- **Excel Definition:** Identifies code blocks that attempt to use a variable but can never be executed due to logical constraints, helping clean up technical debt by removing logic that relies on impossible data states.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: unreachable_uses = len(missing_lines)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Ghost Use Identification = count(uses where ci == 0) / total_uses
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: GhostUseRatio = noCoverageMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: GhostUseIdentification = 1 - StatementCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Ghost Use Identification = count(definitions declared but never referenced) / total_definitions
  - Raw Measurement Formula: Ghost Use % = (Uses that can never be reached / Total Declared Uses) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 2% ghost uses in codebase
  - Normalisation Formula (0–100): MAX(0, 100 – (Ghost_Use% × 25))
  - Execution Frequency: Daily / Per Sprint


### Data Integrity Audit

- **Metric:** Data Integrity Audit
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Coverage Reporting Validation
- **Excel Definition:** Provides a detailed report showing which data paths are safe and which are risky, ensuring your MVP meets high-quality standards by providing an audit trail for every variable in the system.
- **Required Evidence:** Application source, tests, and static-analysis configuration
- **Project Component Producing Evidence:** ASP.NET Core 8 backend and Vite React/TypeScript frontend
- **Source of Evidence:** Tools named in the Excel row for C# / JavaScript / TypeScript
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: coverage_valid = (covered_lines + len(missing_lines)) == num_statements
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Data Integrity Audit = (all uses and definitions mapped in report) AND (no missing DU links)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: DataIntegrityScore = killedMutants / totalMutants
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: DataIntegrityAudit = TotalViolations > 0 ? 1 : 0
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: Data Integrity Audit = count(definitions with consistent type across usages) / total_definitions
  - Raw Measurement Formula: Report Completeness % = (DU pairs with full audit record / Total DU Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 90% of DU pairs have complete data integrity audit record
  - Normalisation Formula (0–100): Score = Report Completeness % [gate at 90%]
  - Execution Frequency: Daily / Per Sprint


### All-Uses Coverage %

- **Metric:** All-Uses Coverage %
- **Metric Category:** L1 White Box · L2 Data Flow Testing · L3 All Uses Coverage · L4 Variable Use Detection
- **Excel Definition:** % of definition-use pairs (variable defined, then used in computation or predicate) exercised by tests
- **Required Evidence:** Test execution with coverage artefacts
- **Project Component Producing Evidence:** xUnit + coverlet (Cobertura), Vitest + v8 coverage (Cobertura)
- **Source of Evidence:** Coverage reports from `dotnet test` and `npm run test:coverage`
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: all_uses_coverage_percent = ( len([d for d in definitions if d.user_count > 0]) / len(definitions) ) * 100
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: All-Uses Coverage % = (count(covered_du_pairs) / total_du_pairs) * 100
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: AllUsesCoveragePercent = (killedMutants / totalMutants) * 100
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: AllUsesCoveragePercent = StatementCoverage
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: All-Uses Coverage % = count(definitions where used == true) / total_definitions
  - Raw Measurement Formula: All-Uses Coverage % = (Covered DU Pairs / Total DU Pairs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 65% all-uses coverage
  - Normalisation Formula (0–100): Score = All-Uses Coverage %
  - Execution Frequency: Daily / Per Sprint


### Code Churn Score

- **Metric:** Code Churn Score
- **Metric Category:** L1 White Box · L2 Development Process Analysis · L3 Code Churn · L4 Risk-Based Testing Prioritization
- **Excel Definition:** Rate of code change (lines added + deleted) over a rolling window; high churn signals instability and elevated regression risk
- **Required Evidence:** Git history over the window named in Excel
- **Project Component Producing Evidence:** Meaningful commits on `Scholarship-CMGroups-positive`
- **Source of Evidence:** Git log (a 30-day window may not yet exist — see clarifications C-07)
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: Code Churn Score = lines
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Code Churn Score = (lines_added + lines_deleted) / (total_files_changed * time_window_days)
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: CodeChurnScore = churn / commit_count
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: CodeChurnScore = churn / commit_count
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: CodeChurnScore = churn / commit_count
  - Raw Measurement Formula: Churn = (Lines Added + Lines Deleted) / Total LOC over rolling 30-day window
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 30% churn per sprint; flag modules with > 50%
  - Normalisation Formula (0–100): MAX(0, 100 – (Churn% × 2)) [50% churn = 0 pts]
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - Requires a 30-day history, which a new repository does not have.


### Impact-Driven Verification

- **Metric:** Impact-Driven Verification
- **Metric Category:** L1 White Box · L2 Development Process Analysis · L3 Code Churn · L4 Regression Testing Focus
- **Excel Definition:** Isolates specific modules that have undergone recent modifications, allowing QA to focus regression suites only on affected areas to prevent existing features from breaking during updates.
- **Required Evidence:** Git history over the window named in Excel
- **Project Component Producing Evidence:** Meaningful commits on `Scholarship-CMGroups-positive`
- **Source of Evidence:** Git log (a 30-day window may not yet exist — see clarifications C-07)
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: impact_score = (insertions + deletions) / max(files, 1)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Impact-Driven Verification = covered_changed_lines / total_changed_lines
  - C# :: Metrics emitted directly: Yes
  - C# :: Derivation: ImpactDrivenVerification = churn
  - JavaScript :: Metrics emitted directly: Yes
  - JavaScript :: Derivation: ImpactDrivenVerification = churn
  - TypeScript :: Metrics emitted directly: Yes
  - TypeScript :: Derivation: ImpactDrivenVerification = churn
  - Raw Measurement Formula: Regression Coverage % = (High-Churn Modules in Regression Suite / Total High-Churn Modules) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of modules with churn > 30% included in regression suite
  - Normalisation Formula (0–100): Score = Regression Coverage % [gate at 100%]
  - Execution Frequency: Daily


### Fault Probability Modeling

- **Metric:** Fault Probability Modeling
- **Metric Category:** L1 White Box · L2 Development Process Analysis · L3 Code Churn · L4 Defect Prediction
- **Excel Definition:** Uses historical churn data to predict where future bugs will likely occur, measuring the correlation between high change volume and defect density as an early warning system for unstable modules.
- **Required Evidence:** Git history over the window named in Excel
- **Project Component Producing Evidence:** Meaningful commits on `Scholarship-CMGroups-positive`
- **Source of Evidence:** Git log (a 30-day window may not yet exist — see clarifications C-07)
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: fault_probability = (insertions + deletions) * complexity
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Fault Probability = ( (lines_added + lines_deleted) * 0.6 + commit_count * 0.4 ) / normalization_factor
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: FaultProbability = (added_lines + deleted_lines) / commit_count
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: FaultProbability = (added_lines + deleted_lines) / commit_count
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: FaultProbability = (added_lines + deleted_lines) / commit_count
  - Raw Measurement Formula: Fault Probability Score = Churn% × Defect_Density (bugs per KLOC) over rolling 30-day window
- **Expected Positive Condition:**
  - Expected Value / Threshold: Fault Probability Score < 5 (Churn% × Defect_Density)
  - Normalisation Formula (0–100): MAX(0, 100 – (Fault_Probability_Score × 10))
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - Requires a 30-day history, which a new repository does not have.
  - Defect density requires a defect-tracking source, which the workbook does not define.


### Validation Suite Updates

- **Metric:** Validation Suite Updates
- **Metric Category:** L1 White Box · L2 Development Process Analysis · L3 Code Churn · L4 Test Case Maintenance Identification
- **Excel Definition:** Identifies when code has changed so significantly that existing test cases are likely outdated or irrelevant, signaling when tests need to be rewritten to maintain accuracy.
- **Required Evidence:** Git history over the window named in Excel
- **Project Component Producing Evidence:** Meaningful commits on `Scholarship-CMGroups-positive`
- **Source of Evidence:** Git log (a 30-day window may not yet exist — see clarifications C-07)
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: Yes
  - Python :: Derivation: test_impact_score = files
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Validation Suite Updates = changed_test_files / changed_prod_files
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: ValidationSuiteUpdates = added_lines / churn
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: ValidationSuiteUpdates = added_lines / churn
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: ValidationSuiteUpdates = added_lines / churn
  - Raw Measurement Formula: Stale Test % = (Test Files not updated despite source churn > 20% / Total Test Files) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 5% of test files stale relative to churned source modules
  - Normalisation Formula (0–100): MAX(0, 100 – (Stale_Test% × 10))
  - Execution Frequency: Daily


### Side Effect Mapping

- **Metric:** Side Effect Mapping
- **Metric Category:** L1 White Box · L2 Development Process Analysis · L3 Code Churn · L4 Change Impact Analysis
- **Excel Definition:** Measures the ripple effect of a code change by identifying all connected modules that may be affected, helping developers understand the full scope before merging to prevent unintended consequences.
- **Required Evidence:** Git history over the window named in Excel
- **Project Component Producing Evidence:** Meaningful commits on `Scholarship-CMGroups-positive`
- **Source of Evidence:** Git log (a 30-day window may not yet exist — see clarifications C-07)
- **Calculation/Derivation:**
  - Python :: Metric emitted directly?: No
  - Python :: Derivation: side_effect_scope = sum(len(file.changed_methods) for file in modified_files)
  - Java :: Metrics emitted directly: No
  - Java :: Derivation: Side Effect Mapping = count(commits where file_A and file_B changed together) / total_commits
  - C# :: Metrics emitted directly: No
  - C# :: Derivation: SideEffectMapping = deleted_lines / churn
  - JavaScript :: Metrics emitted directly: No
  - JavaScript :: Derivation: SideEffectMapping = deleted_lines / churn
  - TypeScript :: Metrics emitted directly: No
  - TypeScript :: Derivation: SideEffectMapping = deleted_lines / churn
  - Raw Measurement Formula: Impact Coverage % = (Affected Downstream Modules Regression-Tested / Total Identified Downstream Modules) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of downstream impacted modules included in test run
  - Normalisation Formula (0–100): Score = Impact Coverage % [gate at 100%]
  - Execution Frequency: Daily


## Black Box

### Critical Path Success Rate

- **Metric:** Critical Path Success Rate
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 User Journey Confidence · L4 Critical Path Testing
- **Excel Definition:** % of critical end-to-end user journeys completing successfully without failure; mapped to PM acceptance criteria
- **Required Evidence:** Passing functional tests of the application lifecycle
- **Project Component Producing Evidence:** `ApplicationWorkflowApiTests`, `ApplicationStatusTransitionPolicy`, Application pages
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#
- **Calculation/Derivation:**
  - Raw Measurement Formula: Critical Path Pass Rate = (Passing Journey Scripts / Total Journey Scripts) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% critical paths passing per build
  - Normalisation Formula (0–100): Score = Critical Path Pass Rate % [gate: 100%]
  - Execution Frequency: Every Commit / PR


### Happy Path Pass Rate

- **Metric:** Happy Path Pass Rate
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 User Journey Confidence · L4 Happy Path Testing
- **Excel Definition:** % of primary intended workflows completing successfully under normal conditions using valid inputs
- **Required Evidence:** Passing functional tests of the application lifecycle
- **Project Component Producing Evidence:** `ApplicationWorkflowApiTests`, `ApplicationStatusTransitionPolicy`, Application pages
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#
- **Calculation/Derivation:**
  - Raw Measurement Formula: Happy Path Pass Rate = (Passing Happy Path Tests / Total Happy Path Tests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% happy paths passing per build
  - Normalisation Formula (0–100): Score = Happy Path Pass Rate %
  - Execution Frequency: Every Commit / PR


### Workflow Execution Success %

- **Metric:** Workflow Execution Success %
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 User Journey Confidence · L4 End-to-End Workflow Testing
- **Excel Definition:** % of full multi-step workflows executing successfully across all integrated systems and services
- **Required Evidence:** Passing functional tests of the application lifecycle
- **Project Component Producing Evidence:** `ApplicationWorkflowApiTests`, `ApplicationStatusTransitionPolicy`, Application pages
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#, PHP, Ruby
- **Calculation/Derivation:**
  - Raw Measurement Formula: E2E Success % = (Passed E2E Flows / Total E2E Flows) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 95% workflow success rate
  - Normalisation Formula (0–100): Score = E2E Success %
  - Execution Frequency: Daily


### State Transition Accuracy %

- **Metric:** State Transition Accuracy %
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 Flow Assurance Confidence · L4 Step Transition Testing
- **Excel Definition:** % of individual step transitions within a multi-step workflow producing correct next-state and data handoff
- **Required Evidence:** Passing functional tests of the application lifecycle
- **Project Component Producing Evidence:** `ApplicationWorkflowApiTests`, `ApplicationStatusTransitionPolicy`, Application pages
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#
- **Calculation/Derivation:**
  - Raw Measurement Formula: Step Accuracy = (Correct Step Transitions / Total Step Transitions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 95% step transitions accurate
  - Normalisation Formula (0–100): Score = Step Accuracy %
  - Execution Frequency: Every Commit / PR


### Min Value Pass Rate

- **Metric:** Min Value Pass Rate
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 Input Boundary Validity · L4 Minimum Boundary Testing
- **Excel Definition:** % of minimum boundary input values handled correctly without error or unexpected behaviour
- **Required Evidence:** Passing and rejecting boundary inputs
- **Project Component Producing Evidence:** `ContractValidationTests`, frontend `validators.test.ts` and `scholarshipForm.test.ts`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#, PHP, Ruby
- **Calculation/Derivation:**
  - Raw Measurement Formula: Min Pass Rate = (Min Boundary Tests Passing / Total Min Boundary Tests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% min boundary values handled correctly
  - Normalisation Formula (0–100): Score = Min Pass Rate %
  - Execution Frequency: Every Commit / PR


### Max Value Pass Rate

- **Metric:** Max Value Pass Rate
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 Input Boundary Validity · L4 Maximum Boundary Testing
- **Excel Definition:** % of maximum boundary input values handled correctly without overflow or unexpected result
- **Required Evidence:** Passing and rejecting boundary inputs
- **Project Component Producing Evidence:** `ContractValidationTests`, frontend `validators.test.ts` and `scholarshipForm.test.ts`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#, PHP, Ruby
- **Calculation/Derivation:**
  - Raw Measurement Formula: Max Pass Rate = (Max Boundary Tests Passing / Total Max Boundary Tests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% max boundary values handled correctly
  - Normalisation Formula (0–100): Score = Max Pass Rate %
  - Execution Frequency: Every Commit / PR


### Out-of-Range Rejection Rate

- **Metric:** Out-of-Range Rejection Rate
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 Input Boundary Validity · L4 Just-Outside Boundary Testing
- **Excel Definition:** % of out-of-range inputs that are correctly rejected or gracefully handled by validation logic
- **Required Evidence:** Passing and rejecting boundary inputs
- **Project Component Producing Evidence:** `ContractValidationTests`, frontend `validators.test.ts` and `scholarshipForm.test.ts`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#, PHP, Ruby
- **Calculation/Derivation:**
  - Raw Measurement Formula: Rejection Rate = (Correctly Rejected OOB Inputs / Total OOB Inputs) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% out-of-range inputs rejected
  - Normalisation Formula (0–100): Score = Rejection Rate %
  - Execution Frequency: Every Commit / PR


### Valid Class Pass Rate

- **Metric:** Valid Class Pass Rate
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 Partition Class Coverage · L4 Valid Partition Testing
- **Excel Definition:** % of equivalence input partition classes (valid inputs) tested and behaving consistently within each class
- **Required Evidence:** Passing and rejecting boundary inputs
- **Project Component Producing Evidence:** `ContractValidationTests`, frontend `validators.test.ts` and `scholarshipForm.test.ts`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#, PHP, Ruby
- **Calculation/Derivation:**
  - Raw Measurement Formula: Valid Class Coverage = (Valid Partitions Passing / Total Valid Partitions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 90% valid classes covered
  - Normalisation Formula (0–100): Score = Valid Class Coverage %
  - Execution Frequency: Every Commit / PR


### Valid Transition Pass Rate

- **Metric:** Valid Transition Pass Rate
- **Metric Category:** L1 Black Box · L2 Functional Testing · L3 Transition Correctness · L4 Valid State Transition Testing
- **Excel Definition:** % of valid state transitions that produce the correct next state and output
- **Required Evidence:** Passing functional tests of the application lifecycle
- **Project Component Producing Evidence:** `ApplicationWorkflowApiTests`, `ApplicationStatusTransitionPolicy`, Application pages
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#, PHP, Ruby
- **Calculation/Derivation:**
  - Raw Measurement Formula: Valid Transition Pass Rate = (Correct Valid Transitions / Total Valid Transitions) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 95% valid transitions pass
  - Normalisation Formula (0–100): Score = Valid Transition Pass Rate %
  - Execution Frequency: Every Commit / PR


### API Uptime %

- **Metric:** API Uptime %
- **Metric Category:** L1 Black Box · L2 API Testing · L3 Interface Reliability · L4 API Availability Testing
- **Excel Definition:** % of time the API endpoint is reachable and returning valid HTTP 2xx responses during test window
- **Required Evidence:** Successful health-check responses
- **Project Component Producing Evidence:** `GET /api/health`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#
- **Calculation/Derivation:**
  - Raw Measurement Formula: Uptime % = (Successful Health-Check Pings / Total Pings) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 99.9% uptime in test/staging environment
  - Normalisation Formula (0–100): Score = Uptime % [gate: >= 99%]
  - Execution Frequency: Daily


### Payload Accuracy Score

- **Metric:** Payload Accuracy Score
- **Metric Category:** L1 Black Box · L2 API Testing · L3 Response Integrity Testing · L4 Payload Accuracy Validation
- **Excel Definition:** % of API responses whose payload structure and data types exactly match the expected schema definition
- **Required Evidence:** OpenAPI document vs observed responses
- **Project Component Producing Evidence:** Swashbuckle OpenAPI, `HealthAndDocumentationTests`, frontend `src/api/types.ts`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#
- **Calculation/Derivation:**
  - Raw Measurement Formula: Payload Accuracy = (Schema-Conformant Responses / Total Responses) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 99% payload accuracy
  - Normalisation Formula (0–100): Score = Payload Accuracy %
  - Execution Frequency: Every Commit / PR


### Contract Conformance Rate

- **Metric:** Contract Conformance Rate
- **Metric Category:** L1 Black Box · L2 API Testing · L3 Contract Reliability · L4 OpenAPI Contract Testing
- **Excel Definition:** % of API responses that fully comply with the OpenAPI 3.x / Swagger specification contract
- **Required Evidence:** OpenAPI document vs observed responses
- **Project Component Producing Evidence:** Swashbuckle OpenAPI, `HealthAndDocumentationTests`, frontend `src/api/types.ts`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#
- **Calculation/Derivation:**
  - Raw Measurement Formula: Conformance Rate = (Conformant API Calls / Total Tested Calls) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% contract conformance — zero drift tolerated
  - Normalisation Formula (0–100): Score = Conformance Rate %
  - Execution Frequency: Every Commit / PR


### Consumer Contract Pass Rate

- **Metric:** Consumer Contract Pass Rate
- **Metric Category:** L1 Black Box · L2 API Testing · L3 Consumer-Driven Contract Testing · L4 Pact Contract Verification
- **Excel Definition:** % of provider responses satisfying all consumer-side contract expectations (Pact consumer-driven style)
- **Required Evidence:** OpenAPI document vs observed responses
- **Project Component Producing Evidence:** Swashbuckle OpenAPI, `HealthAndDocumentationTests`, frontend `src/api/types.ts`
- **Source of Evidence:** Primary Tool: Pact (multi-language OSS); Secondary Tool: PactFlow (free for 5 integrations); Languages Supported: Python, Node.js (JS, TS), Java, C#, Ruby, PHP
- **Calculation/Derivation:**
  - Raw Measurement Formula: Consumer Pass Rate = (Satisfied Consumer Contracts / Total Consumer Contracts) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% consumer contracts satisfied per build
  - Normalisation Formula (0–100): Score = Consumer Pass Rate %
  - Execution Frequency: Every Commit / PR


### Schema Drift Frequency

- **Metric:** Schema Drift Frequency
- **Metric Category:** L1 Black Box · L2 API Testing · L3 Schema Drift Detection · L4 Breaking Change Detection
- **Excel Definition:** Number of unannounced schema-breaking changes (field removal, type change) detected between consecutive builds
- **Required Evidence:** OpenAPI document vs observed responses
- **Project Component Producing Evidence:** Swashbuckle OpenAPI, `HealthAndDocumentationTests`, frontend `src/api/types.ts`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Language-agnostic (HTTP-based)
- **Calculation/Derivation:**
  - Raw Measurement Formula: Drift Count = Number of breaking diff events per build cycle
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 unannounced schema-breaking changes per sprint
  - Normalisation Formula (0–100): MAX(0, 100 – (Drift_Count × 20)) [each drift event = –20 pts]
  - Execution Frequency: Every Commit / PR


### Browser Compatibility Pass Rate

- **Metric:** Browser Compatibility Pass Rate
- **Metric Category:** L1 Black Box · L2 Compatibility Testing · L3 Experience Stability · L4 Cross-Browser Testing
- **Excel Definition:** % of test scenarios passing across all targeted browsers (Chrome, Firefox, Safari, Edge) at defined viewport sizes
- **Required Evidence:** Rendered UI across viewports and interactions
- **Project Component Producing Evidence:** Responsive CSS breakpoints, Vitest + Testing Library interaction tests
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: Python, Node.js (JS, TS), Java, C#
- **Calculation/Derivation:**
  - Raw Measurement Formula: Pass Rate = (Passing Browser × Scenario Combos / Total Combos) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 98% browser compatibility pass rate
  - Normalisation Formula (0–100): Score = Pass Rate %
  - Execution Frequency: Weekly


### Breakpoint Pass Rate

- **Metric:** Breakpoint Pass Rate
- **Metric Category:** L1 Black Box · L2 Compatibility Testing · L3 Cross-Device Layout Validation · L4 Breakpoint Validation
- **Excel Definition:** % of UI breakpoints (mobile, tablet, desktop) rendering correctly without layout regression
- **Required Evidence:** Rendered UI across viewports and interactions
- **Project Component Producing Evidence:** Responsive CSS breakpoints, Vitest + Testing Library interaction tests
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: JS, TS, JSX, TSX, Vue
- **Calculation/Derivation:**
  - Raw Measurement Formula: Breakpoint Pass Rate = (Passing Breakpoint Tests / Total Breakpoint Tests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% breakpoint pass rate
  - Normalisation Formula (0–100): Score = Breakpoint Pass Rate %
  - Execution Frequency: Daily


### Visual Regression Failure Rate

- **Metric:** Visual Regression Failure Rate
- **Metric Category:** L1 Black Box · L2 Frontend Testing · L3 Visual Regression Testing · L4 Screenshot Comparison
- **Excel Definition:** % pixel difference between baseline and current screenshot at defined viewports; detects unintended UI changes
- **Required Evidence:** Rendered UI across viewports and interactions
- **Project Component Producing Evidence:** Responsive CSS breakpoints, Vitest + Testing Library interaction tests
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: JS, TS, JSX, TSX, Vue
- **Calculation/Derivation:**
  - Raw Measurement Formula: Pixel Diff % = (Changed Pixels / Total Pixels) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: <= 1% pixel difference threshold
  - Normalisation Formula (0–100): MAX(0, 100 – Pixel_Diff% × 50)
  - Execution Frequency: Daily


### Interaction Success Rate

- **Metric:** Interaction Success Rate
- **Metric Category:** L1 Black Box · L2 Frontend Testing · L3 DOM & Interaction Validation · L4 Element Interaction Testing
- **Excel Definition:** % of interactive UI elements (buttons, forms, modals, dropdowns) responding correctly to user interactions
- **Required Evidence:** Rendered UI across viewports and interactions
- **Project Component Producing Evidence:** Responsive CSS breakpoints, Vitest + Testing Library interaction tests
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: JS, TS, JSX, TSX, Vue
- **Calculation/Derivation:**
  - Raw Measurement Formula: Interaction Pass Rate = (Passing Interaction Tests / Total Interaction Tests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% interactive element pass rate
  - Normalisation Formula (0–100): Score = Interaction Pass Rate %
  - Execution Frequency: Every Commit / PR


### WCAG Compliance Score

- **Metric:** WCAG Compliance Score
- **Metric Category:** L1 Black Box · L2 Compatibility Testing · L3 Accessibility Validation · L4 WCAG Compliance Testing
- **Excel Definition:** Count of WCAG 2.1 Level A and AA violations detected by automated accessibility scanning
- **Required Evidence:** Accessible markup and keyboard-operable controls
- **Project Component Producing Evidence:** `Layout` skip link, labelled fields, `aria-invalid`, focus styles in `styles.css`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: JS, TS, JSX, TSX, Vue, HTML
- **Calculation/Derivation:**
  - Raw Measurement Formula: WCAG Score = Crit×20 + Serious×10 + Moderate×3 + Minor×1
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 Critical WCAG violations; 0 Level A failures
  - Normalisation Formula (0–100): MAX(0, 100 – WCAG_Score)
  - Execution Frequency: Every Commit / PR


### Keyboard Navigation Coverage

- **Metric:** Keyboard Navigation Coverage
- **Metric Category:** L1 Black Box · L2 Compatibility Testing · L3 Keyboard Navigation Testing · L4 Keyboard Accessibility Coverage
- **Excel Definition:** % of interactive elements reachable and operable by keyboard alone (Tab, Enter, Space, Arrow keys)
- **Required Evidence:** Accessible markup and keyboard-operable controls
- **Project Component Producing Evidence:** `Layout` skip link, labelled fields, `aria-invalid`, focus styles in `styles.css`
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Selenium; Languages Supported: JS, TS, JSX, TSX, Vue
- **Calculation/Derivation:**
  - Raw Measurement Formula: Keyboard Coverage = (Keyboard-Accessible Elements / Total Interactive Elements) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 100% keyboard navigable critical elements
  - Normalisation Formula (0–100): Score = Keyboard Coverage %
  - Execution Frequency: Every Commit / PR


## Security Code (Repository)

### Secrets Exposed in Code Count

- **Metric:** Secrets Exposed in Code Count
- **Metric Category:** L1 Security Testing · L2 Secret Detection · L3 Secret Scanning · L4 Hardcoded Secret Detection
- **Excel Definition:** Scans git history and all source files for committed API keys, tokens, passwords, private keys, and certificates.
- **Required Evidence:** Repository contents and git history with no committed credentials
- **Project Component Producing Evidence:** `.gitignore`, empty `ConnectionStrings`/`SigningKey` in committed config, `dotnet user-secrets`, frontend `.env.example`
- **Source of Evidence:** Primary Tool: Gitleaks; Validation Type: Static Repo Scan; Requires Live App?: No; File / Artifact Scanned: Git history, all source files (.env, configs, code)
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(findings where entropy > threshold OR matches secret_regex_patterns)
  - Raw Measurement Formula: Secrets Count = count(distinct secret patterns detected across all file types)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 hardcoded secrets in source code or git history
  - Normalisation Formula (0–100): MAX(0, 100 – (Secrets_Count × 50)) [any = BLOCK]
  - Execution Frequency: Every Commit / PR


### Blocked Secret Commit Count

- **Metric:** Blocked Secret Commit Count
- **Metric Category:** L1 Security Testing · L2 Secret Detection · L3 Secret Scanning · L4 Pre-Commit Secret Prevention
- **Excel Definition:** Counts secret commit attempts blocked by pre-commit hooks before reaching the remote repository — measures enforcement effectiveness.
- **Required Evidence:** Repository contents and git history with no committed credentials
- **Project Component Producing Evidence:** `.gitignore`, empty `ConnectionStrings`/`SigningKey` in committed config, `dotnet user-secrets`, frontend `.env.example`
- **Source of Evidence:** Primary Tool: detect-secrets; Validation Type: Static Pre-Commit Hook; Requires Live App?: No; File / Artifact Scanned: Staged git files
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(commit_attempts blocked by pre-commit hook in period)
  - Raw Measurement Formula: Block Rate = (Blocked Commits / (Blocked + Slipped Through)) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of secret patterns blocked before push; 0 slip-throughs
  - Normalisation Formula (0–100): Score = Block Rate % [gate at 100%]
  - Execution Frequency: Every Commit / PR


### Historical Secret Exposure Count

- **Metric:** Historical Secret Exposure Count
- **Metric Category:** L1 Security Testing · L2 Secret Detection · L3 Secret Scanning · L4 Secrets in Git History
- **Excel Definition:** Full git history scan for secrets committed and later deleted — still accessible in git log. Detects compliance violations requiring forced-push remediation.
- **Required Evidence:** Repository contents and git history with no committed credentials
- **Project Component Producing Evidence:** `.gitignore`, empty `ConnectionStrings`/`SigningKey` in committed config, `dotnet user-secrets`, frontend `.env.example`
- **Source of Evidence:** Primary Tool: Trufflehog; Validation Type: Static Git History Scan; Requires Live App?: No; File / Artifact Scanned: Full git commit history
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(commits in full history where secret_pattern matched AND NOT in HEAD)
  - Raw Measurement Formula: Historical Exposure Score = Count(Historical Secrets) × 40
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 secrets exposed anywhere in git commit history
  - Normalisation Formula (0–100): MAX(0, 100 – Historical_Exposure_Score)
  - Execution Frequency: Weekly


### Open Firewall Rule Count

- **Metric:** Open Firewall Rule Count
- **Metric Category:** L1 Security Testing · L2 IaC Security · L3 IaC Scanning · L4 Open Security Group Rule Detection
- **Excel Definition:** Detects Terraform/CloudFormation resources defining security groups open to 0.0.0.0/0 on sensitive ports (non-80/443).
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov / tfsec; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: .tf, .yaml, .json IaC files
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(resources where ingress_cidr == '0.0.0.0/0' AND port NOT IN [80,443])
  - Raw Measurement Formula: Firewall Risk = Count(Open Ports)×30 + Count(Wildcard CIDRs)×15
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 security groups open to 0.0.0.0/0 on non-web ports
  - Normalisation Formula (0–100): MAX(0, 100 – Firewall_Risk)
  - Execution Frequency: Every Commit / PR


### Unencrypted Storage Count

- **Metric:** Unencrypted Storage Count
- **Metric Category:** L1 Security Testing · L2 IaC Security · L3 IaC Scanning · L4 Unencrypted Storage Definition
- **Excel Definition:** Identifies RDS, S3, EBS, GCS resources in IaC without encryption-at-rest enabled — non-compliant with PCI-DSS and SOC 2.
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov / tfsec; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: .tf, CloudFormation templates
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(resources where encryption_enabled == false OR storage_encrypted == false)
  - Raw Measurement Formula: Encryption Deficit Score = Count(Unencrypted Resources)×25
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 storage resources without encryption-at-rest defined
  - Normalisation Formula (0–100): MAX(0, 100 – Encryption_Deficit_Score)
  - Execution Frequency: Every Commit / PR


### Public Storage Bucket Count

- **Metric:** Public Storage Bucket Count
- **Metric Category:** L1 Security Testing · L2 IaC Security · L3 IaC Scanning · L4 Publicly Exposed Resource Detection
- **Excel Definition:** Detects S3/GCS buckets in IaC with public access enabled — measures blast radius of potential data leakage from misconfigured cloud storage.
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov / kics; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: .tf, CloudFormation templates
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(aws_s3_bucket where acl == 'public-read' OR block_public_access == false)
  - Raw Measurement Formula: Public Exposure Score = Count(Public Buckets)×40
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 publicly accessible storage buckets in IaC definitions
  - Normalisation Formula (0–100): MAX(0, 100 – Public_Exposure_Score)
  - Execution Frequency: Every Commit / PR


### CIS Benchmark Violation Count

- **Metric:** CIS Benchmark Violation Count
- **Metric Category:** L1 Security Testing · L2 IaC Security · L3 IaC Scanning · L4 CIS Benchmark Compliance
- **Excel Definition:** Measures percentage of IaC configurations meeting CIS Benchmark baseline controls for AWS/GCP/Azure cloud infrastructure.
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: All IaC files in repository
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(checks_failed against CIS benchmark ruleset) / total_checks × 100
  - Raw Measurement Formula: CIS Compliance % = (Passing CIS Checks / Total CIS Checks) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 95% CIS Benchmark compliance across IaC files
  - Normalisation Formula (0–100): Score = CIS Compliance % [gate at 95%]
  - Execution Frequency: Every Commit / PR


### Unreviewed Change Count

- **Metric:** Unreviewed Change Count
- **Metric Category:** L1 Security Testing · L2 SOC 2 Compliance · L3 Change Management Testing · L4 Change Control Verification
- **Excel Definition:** Count of code changes merged without required review approvals, bypassing change control gates — direct SOC 2 CC8.1 evidence.
- **Required Evidence:** Pull-request and merge history on the hosting provider
- **Project Component Producing Evidence:** Git branch `Scholarship-CMGroups-positive` (PR history is produced after review on the host)
- **Source of Evidence:** Primary Tool: GitHub Branch Protection API; Validation Type: Static Git Metadata Query; Requires Live App?: No; File / Artifact Scanned: GitHub PR / audit log
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(merged PRs where required_reviewers == 0 OR review_bypassed == true)
  - Raw Measurement Formula: Bypass Rate = (Unreviewed Merges / Total Merges) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 merges bypassing required review gates
  - Normalisation Formula (0–100): MAX(0, 100 – (Bypass_Rate × 20))
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - Requires pull-request history on the hosting provider.
  - Requires merge history on the hosting provider.


### Overprivileged Account Count

- **Metric:** Overprivileged Account Count
- **Metric Category:** L1 Security Testing · L2 SOC 2 Compliance · L3 Access Control · L4 Privileged Access Audit
- **Excel Definition:** Identifies repository collaborators or CI/CD service accounts with admin/write permissions exceeding their operational need — principle of least privilege.
- **Required Evidence:** Account privilege assignments
- **Project Component Producing Evidence:** `UserRole` (Applicant / Administrator) and `[Authorize(Roles = ...)]` on controllers
- **Source of Evidence:** Primary Tool: GitHub API / GitLab API; Validation Type: Static API Metadata Query; Requires Live App?: No; File / Artifact Scanned: Repository collaborator / permission configuration
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(collaborators where permission == 'admin' AND role != 'owner')
  - Raw Measurement Formula: Privilege Score = Count(Overprivileged Accounts)×20
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 service accounts with admin access beyond operational need
  - Normalisation Formula (0–100): MAX(0, 100 – Privilege_Score)
  - Execution Frequency: Weekly


## Security URL (API Service)

### OWASP High/Critical Finding Count

- **Metric:** OWASP High/Critical Finding Count
- **Metric Category:** L1 Security Testing · L2 OWASP Testing · L3 DAST — Dynamic Application Security Testing · L4 OWASP Top 10 Vulnerability Scan
- **Excel Definition:** Dynamic scan of a running application to identify OWASP Top 10 vulnerabilities: injection, broken auth, SSRF, insecure design, etc.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: OWASP ZAP; Secondary Tool: Nikto; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(risk == 'High' OR risk == 'Critical')
  - Raw Measurement Formula: DAST Score = Count(Critical)×25 + Count(High)×10 + Count(Medium)×3
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 High/Critical OWASP findings; < 5 Medium per release
  - Normalisation Formula (0–100): MAX(0, 100 – DAST_Score) [any critical = BLOCK]
  - Execution Frequency: Per Sprint / Pre-Release


### SQLi Vulnerability Count

- **Metric:** SQLi Vulnerability Count
- **Metric Category:** L1 Security Testing · L2 OWASP Testing · L3 DAST — Dynamic Application Security Testing · L4 SQL Injection Testing
- **Excel Definition:** Targeted dynamic testing for SQL injection vulnerabilities in all user-input paths and API parameters against the running application.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: OWASP ZAP; Secondary Tool: Nikto; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: CWE-89/CWE-564 — count(cweid in (89,564) OR alert contains SQLi keywords)
  - Raw Measurement Formula: SQLi Count = Confirmed SQLi findings from dynamic scan
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 SQLi vulnerabilities
  - Normalisation Formula (0–100): MAX(0, 100 – (SQLi_Count × 30))
  - Execution Frequency: Per Sprint / Pre-Release


### XSS Vulnerability Count

- **Metric:** XSS Vulnerability Count
- **Metric Category:** L1 Security Testing · L2 OWASP Testing · L3 DAST — Dynamic Application Security Testing · L4 XSS Testing
- **Excel Definition:** Dynamic testing for Cross-Site Scripting vulnerabilities in all reflected, stored, and DOM-based injection points of the live application.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: OWASP ZAP; Secondary Tool: Nikto; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: CWE-79/CWE-83 — count(cweid in (79,83) OR alert contains 'xss','reflected xss','stored xss','dom xss')
  - Raw Measurement Formula: XSS Count = Confirmed XSS findings from dynamic scan
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 XSS vulnerabilities (High/Critical)
  - Normalisation Formula (0–100): MAX(0, 100 – (XSS_Count × 25))
  - Execution Frequency: Per Sprint / Pre-Release


### Unauthorized Access Count

- **Metric:** Unauthorized Access Count
- **Metric Category:** L1 Security Testing · L2 OWASP Testing · L3 Authentication & Authorization Testing · L4 Broken Access Control Testing
- **Excel Definition:** Tests for broken access control — horizontal/vertical privilege escalation, IDOR, missing function-level access control in the live API.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: OWASP ZAP; Secondary Tool: Semgrep OSS; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: CWE-284/285/639/862 — count(cweid in (284,285,639,862) OR alert contains 'idor','authorization bypass','privilege escalation')
  - Raw Measurement Formula: Auth Failure Count = Unauthorized endpoints successfully accessed during scan
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 unauthorized access paths; 0 IDOR vulnerabilities
  - Normalisation Formula (0–100): MAX(0, 100 – (Auth_Failure_Count × 20))
  - Execution Frequency: Per Sprint / Pre-Release


### SSRF Vulnerability Count

- **Metric:** SSRF Vulnerability Count
- **Metric Category:** L1 Security Testing · L2 OWASP Testing · L3 DAST — Dynamic Application Security Testing · L4 Server-Side Request Forgery (SSRF) Testing
- **Excel Definition:** Tests live API endpoints for SSRF vulnerabilities where the server can be tricked into making requests to internal or external resources.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: OWASP ZAP; Secondary Tool: Burp Suite Community; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: CWE-918 — count(findings where alert contains 'ssrf' OR 'server-side request forgery')
  - Raw Measurement Formula: SSRF Count = Confirmed SSRF findings from dynamic scan
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 SSRF vulnerabilities
  - Normalisation Formula (0–100): MAX(0, 100 – (SSRF_Count × 35))
  - Execution Frequency: Per Sprint / Pre-Release


### Unauthenticated API Endpoint Count

- **Metric:** Unauthenticated API Endpoint Count
- **Metric Category:** L1 Security Testing · L2 API Security · L3 API Endpoint Validation · L4 Unauthenticated Endpoint Testing
- **Excel Definition:** Identifies API endpoints accessible without a valid authentication token — returns 200 without Authorization header present.
- **Required Evidence:** Authenticated endpoints with object-level checks
- **Project Component Producing Evidence:** `ApplicationsController`, `ApplicantsController`, `AuthorizationApiTests`
- **Source of Evidence:** Primary Tool: OWASP ZAP / 42Crunch; Secondary Tool: Postman / Newman; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(endpoints returning 2xx without valid Authorization header)
  - Raw Measurement Formula: Unauth Count = Count(Endpoints responding 200 without auth token)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 authenticated endpoints accessible without valid token
  - Normalisation Formula (0–100): MAX(0, 100 – (Unauth_Count × 25))
  - Execution Frequency: Per Sprint / Pre-Release


### BOLA Finding Count

- **Metric:** BOLA Finding Count
- **Metric Category:** L1 Security Testing · L2 API Security · L3 Authorization Testing · L4 BOLA / IDOR Testing
- **Excel Definition:** Tests whether authenticated User A can access User B's resources via manipulated object references in live API calls.
- **Required Evidence:** Authenticated endpoints with object-level checks
- **Project Component Producing Evidence:** `ApplicationsController`, `ApplicantsController`, `AuthorizationApiTests`
- **Source of Evidence:** Primary Tool: Postman / ZAP; Secondary Tool: ZAP Active Scan; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(requests where user_a accesses user_b_resource AND response_code == 200)
  - Raw Measurement Formula: BOLA Risk = Count(Cross-user Resource Access Violations)×25
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 broken object-level authorization findings
  - Normalisation Formula (0–100): MAX(0, 100 – BOLA_Risk)
  - Execution Frequency: Per Sprint / Pre-Release


### APIs Without Rate Limiting Count

- **Metric:** APIs Without Rate Limiting Count
- **Metric Category:** L1 Security Testing · L2 API Security · L3 Rate Limiting Compliance · L4 Rate Limit Enforcement Testing
- **Excel Definition:** Verifies all public-facing API endpoints enforce request rate limiting — tests that 429 responses are returned after threshold is exceeded.
- **Required Evidence:** HTTP 429 once a request budget is exceeded
- **Project Component Producing Evidence:** `AddApiRateLimiting` + `RateLimitingApiTests` (permit/window values are provisional — see docs/clarifications.md C-05)
- **Source of Evidence:** Primary Tool: Postman / Newman; Secondary Tool: k6 / Locust; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(endpoints where 429 NOT returned after threshold_requests within time_window)
  - Raw Measurement Formula: Rate Limit Coverage % = (Endpoints with enforced rate limit / Total Public Endpoints) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of public API endpoints enforce rate limiting
  - Normalisation Formula (0–100): Score = Rate_Limit_Coverage % [gate at 100%]
  - Execution Frequency: Per Sprint / Pre-Release
- **Requirement clarification needed:**
  - The rate-limit request count and time window are not stated in the workbook.


### Missing Security Header Count

- **Metric:** Missing Security Header Count
- **Metric Category:** L1 Security Testing · L2 API Security · L3 Transport Security · L4 Security Header Validation
- **Excel Definition:** Checks that all API responses include required security headers: HSTS, X-Frame-Options, Content-Security-Policy, X-Content-Type-Options.
- **Required Evidence:** Security headers on API responses; TLS at the host
- **Project Component Producing Evidence:** `SecurityHeadersMiddleware`, `UseHttpsRedirection`, `UseHsts`
- **Source of Evidence:** Primary Tool: OWASP ZAP; Secondary Tool: SecurityHeaders.com API; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(responses missing [Strict-Transport-Security, X-Frame-Options, Content-Security-Policy, X-Content-Type-Options])
  - Raw Measurement Formula: Header Gap Score = Count(Missing Required Headers per endpoint)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 responses missing required security headers
  - Normalisation Formula (0–100): MAX(0, 100 – (Header_Gap_Score × 10))
  - Execution Frequency: Every Commit / PR


### Weak TLS Config Count

- **Metric:** Weak TLS Config Count
- **Metric Category:** L1 Security Testing · L2 API Security · L3 Transport Security · L4 TLS Configuration Testing
- **Excel Definition:** Validates that all API endpoints use TLS 1.2+ and strong cipher suites — tests for deprecated SSL/TLS versions and weak ciphers.
- **Required Evidence:** Security headers on API responses; TLS at the host
- **Project Component Producing Evidence:** `SecurityHeadersMiddleware`, `UseHttpsRedirection`, `UseHsts`
- **Source of Evidence:** Primary Tool: testssl.sh (OSS); Secondary Tool: OWASP ZAP; Validation Type: Dynamic TLS Scan; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(findings where protocol in [SSLv3,TLS1.0,TLS1.1] OR cipher_strength == 'weak')
  - Raw Measurement Formula: TLS Risk = Count(Weak Protocols)×20 + Count(Weak Ciphers)×10
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 weak TLS configurations; TLS 1.2+ minimum enforced
  - Normalisation Formula (0–100): MAX(0, 100 – TLS_Risk)
  - Execution Frequency: Per Sprint / Pre-Release


### Session Timeout Compliance Rate

- **Metric:** Session Timeout Compliance Rate
- **Metric Category:** L1 Security Testing · L2 Auth & Session · L3 Session Management Testing · L4 Session Timeout Compliance
- **Excel Definition:** Validates that sessions expire after the policy-defined idle period and tokens cannot be reused after expiry — tests live session management endpoints.
- **Required Evidence:** Expired tokens rejected
- **Project Component Producing Evidence:** `JwtBearer` `ClockSkew = TimeSpan.Zero` and `AccessTokenLifetimeMinutes` (provisional — C-04)
- **Source of Evidence:** Primary Tool: Postman / Playwright; Secondary Tool: Newman; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (sessions_expired_within_policy / total_sessions_tested) × 100
  - Raw Measurement Formula: Compliance Rate = (Sessions expiring within SLA / Total Sessions Tested) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% of sessions expire within policy-defined idle timeout
  - Normalisation Formula (0–100): Score = Compliance Rate % [gate at 100%]
  - Execution Frequency: Per Sprint / Pre-Release
- **Requirement clarification needed:**
  - The policy value (e.g. idle timeout) is not stated in the workbook.


### CHD Exposure Finding Count

- **Metric:** CHD Exposure Finding Count
- **Metric Category:** L1 Security Testing · L2 PCI-DSS Compliance · L3 Payment Data Security · L4 Cardholder Data Exposure Scan
- **Excel Definition:** Detects unmasked or improperly transmitted cardholder data (PAN, track data) in live HTTP/API responses and network traffic.
- **Required Evidence:** API payloads limited to fields the application stores
- **Project Component Producing Evidence:** Response contracts (`ApplicantResponse`, `AuthResponse`) — no payment-card fields exist to expose
- **Source of Evidence:** Primary Tool: OWASP ZAP; Secondary Tool: mitmproxy + Presidio; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(alerts where response_body matches PAN_regex OR track_data_regex)
  - Raw Measurement Formula: CHD Score = Count(Exposed PAN Instances)×40 + Count(Partial Exposure)×10
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 CHD exposure findings in any API response
  - Normalisation Formula (0–100): MAX(0, 100 – CHD_Score) [any = BLOCK]
  - Execution Frequency: Per Sprint / Pre-Release


### PII in API Response Count

- **Metric:** PII in API Response Count
- **Metric Category:** L1 Security Testing · L2 GDPR Compliance · L3 PII in API Responses · L4 PII Exposure in Live Responses
- **Excel Definition:** Intercepts live API responses and detects PII (SSN, email, student ID) returned unnecessarily in payloads — runtime privacy validation.
- **Required Evidence:** API payloads limited to fields the application stores
- **Project Component Producing Evidence:** Response contracts (`ApplicantResponse`, `AuthResponse`) — no payment-card fields exist to expose
- **Source of Evidence:** Primary Tool: Presidio + mitmproxy; Secondary Tool: OWASP ZAP; Validation Type: Dynamic Proxy Scan; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(responses where Presidio detects entity_type in [US_SSN,EMAIL_ADDRESS,PERSON,STUDENT_ID])
  - Raw Measurement Formula: PII Leak Score = Count(PII Fields Exposed Unnecessarily)×30
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 PII items exposed in API responses beyond minimum necessary
  - Normalisation Formula (0–100): MAX(0, 100 – PII_Leak_Score) [any = BLOCK]
  - Execution Frequency: Per Sprint / Pre-Release


### Data Deletion Verification Rate

- **Metric:** Data Deletion Verification Rate
- **Metric Category:** L1 Security Testing · L2 GDPR Compliance · L3 Data Subject Rights Testing · L4 Right-to-Erasure Testing
- **Excel Definition:** Validates that user data deletion requests (GDPR Art. 17) are fully executed across all storage layers via live API calls.
- **Required Evidence:** Confirmed erasure of applicant records and dependents
- **Project Component Producing Evidence:** `ApplicantsController.Erase`, `ApplicantService.EraseAsync`, Profile page confirmation
- **Source of Evidence:** Primary Tool: Postman; Secondary Tool: REST Assured; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (deletion_requests_fully_executed / total_deletion_requests) × 100
  - Raw Measurement Formula: Deletion Rate = (Storage Layers with Confirmed Deletion / Total Storage Layers) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% data deleted across all storage layers on erasure request
  - Normalisation Formula (0–100): Score = Deletion Rate % [gate at 100%]
  - Execution Frequency: Per Sprint / Pre-Release


### Age-Gate Bypass Count

- **Metric:** Age-Gate Bypass Count
- **Metric Category:** L1 Security Testing · L2 GDPR Compliance · L3 Consent & Age-Gate Compliance · L4 COPPA Age Verification Testing
- **Excel Definition:** Tests for COPPA age-gate bypass vulnerabilities in the live app — ensures under-13 users cannot access restricted content without parental consent.
- **Required Evidence:** Age-restricted access control
- **Project Component Producing Evidence:** Requirement clarification needed — the workbook does not define an age gate for this application
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Postman; Validation Type: Dynamic E2E Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(test_cases where underage_user_access == allowed)
  - Raw Measurement Formula: Bypass Rate = (Bypassed Age Gates / Total Age Gate Tests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 age-gate bypass vulnerabilities
  - Normalisation Formula (0–100): MAX(0, 100 – (Bypass_Rate × 30))
  - Execution Frequency: Per Sprint / Pre-Release


### Evidence Collection Rate %

- **Metric:** Evidence Collection Rate %
- **Metric Category:** L1 Security Testing · L2 SOC 2 Compliance · L3 Audit Evidence Completeness · L4 SOC 2 Evidence Collection Rate
- **Excel Definition:** % of SOC 2 Type II control evidence items collected and uploaded for audit period — access logs, change logs, test results, incident reports.
- **Required Evidence:** Audit artefacts from tests and logs
- **Project Component Producing Evidence:** xUnit + Vitest output, Swagger document, structured logging of surrogate keys
- **Source of Evidence:** Primary Tool: Drata; Secondary Tool: Vanta; Validation Type: Platform / SaaS; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (evidence_items_collected / total_required_control_evidence_items) × 100
  - Raw Measurement Formula: Evidence Rate = (Evidence Collected / Total Required) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 100% of required SOC 2 control evidence collected per audit period
  - Normalisation Formula (0–100): Score = Evidence Rate % [gate at 100%]
  - Execution Frequency: Per Sprint / Pre-Release


### Unreviewed Change Count

- **Metric:** Unreviewed Change Count
- **Metric Category:** L1 Security Testing · L2 SOC 2 Compliance · L3 Change Management Testing · L4 Change Control Verification
- **Excel Definition:** Count of code changes merged without required review approvals, bypassing change control gates — validated via live GitHub API.
- **Required Evidence:** Pull-request and merge history on the hosting provider
- **Project Component Producing Evidence:** Git branch `Scholarship-CMGroups-positive` (PR history is produced after review on the host)
- **Source of Evidence:** Primary Tool: GitHub API (branch protection); Secondary Tool: GitLab API; Validation Type: Dynamic API Query; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(merged PRs where required_reviewers == 0 OR review_bypassed == true)
  - Raw Measurement Formula: Bypass Rate = (Unreviewed Merges / Total Merges) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 merges bypassing required review gates
  - Normalisation Formula (0–100): MAX(0, 100 – (Bypass_Rate × 20))
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - Requires pull-request history on the hosting provider.
  - Requires merge history on the hosting provider.


## Compliance

### PII Exposure Finding Count

- **Metric:** PII Exposure Finding Count
- **Metric Category:** L1 Security & Compliance · L2 FERPA/COPPA Compliance · L3 Student PII Exposure Scan · L4 PII Data Exposure Detection
- **Excel Definition:** Scans codebase and logs for student PII exposure — SSN, student ID, email, grade data — in violation of FERPA
- **Required Evidence:** API payloads limited to fields the application stores
- **Project Component Producing Evidence:** Response contracts (`ApplicantResponse`, `AuthResponse`) — no payment-card fields exist to expose
- **Source of Evidence:** Primary Tool: Microsoft Presidio; Secondary Tool: Gitleaks (custom regex); Languages Supported: Language-agnostic (Git / CI level)
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivations: count(findings where entity_type in [US_SSN, EMAIL_ADDRESS, PERSON, STUDENT_ID] AND source in [codebase, logs, API_response])
  - Raw Measurement Formula: PII Exposure Count = Instances of PII patterns (SSN, student ID, email, DOB) found in code/logs
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 PII exposure findings in code, logs, or test fixtures
  - Normalisation Formula (0–100): MAX(0, 100 – (PII_Count × 40)) [any exposure = BLOCK]
  - Execution Frequency: Every Commit / PR


### Age-Gate Bypass Count

- **Metric:** Age-Gate Bypass Count
- **Metric Category:** L1 Security & Compliance · L2 FERPA/COPPA Compliance · L3 Consent & Age-Gate Compliance · L4 COPPA Age Verification Testing
- **Excel Definition:** Tests for COPPA age-gate bypass vulnerabilities — ensures under-13 users cannot access restricted content without parental consent
- **Required Evidence:** Age-restricted access control
- **Project Component Producing Evidence:** Requirement clarification needed — the workbook does not define an age gate for this application
- **Source of Evidence:** Primary Tool: Playwright; Secondary Tool: Postman; Languages Supported: JS, TS, Python, Java
- **Calculation/Derivation:**
  - Direct Metric: No (Test case for underage is written)
  - Derivations: Age_Gate_Bypass_Count = count( test_cases where underage_user_access == allowed )
  - Raw Measurement Formula: Bypass Count = Number of age-gate bypass scenarios succeeding in test suite
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 age-gate bypass vulnerabilities
  - Normalisation Formula (0–100): MAX(0, 100 – (Bypass_Count × 30))
  - Execution Frequency: Per Sprint / Pre-Release


### Data Deletion Verification Rate

- **Metric:** Data Deletion Verification Rate
- **Metric Category:** L1 Security & Compliance · L2 GDPR Compliance · L3 Data Subject Rights Testing · L4 Right-to-Erasure Testing
- **Excel Definition:** Validates that user data deletion requests (GDPR Art. 17 Right to Erasure) are fully executed across all storage layers
- **Required Evidence:** Confirmed erasure of applicant records and dependents
- **Project Component Producing Evidence:** `ApplicantsController.Erase`, `ApplicantService.EraseAsync`, Profile page confirmation
- **Source of Evidence:** Primary Tool: Postman; Secondary Tool: REST Assured; Languages Supported: JS, TS, Python, Java, C#
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivations: (deletion requests fully executed across all storage layers / total deletion requests tested) × 100
  - Raw Measurement Formula: Deletion Rate = (Storage Layers with Confirmed Deletion / Total Storage Layers) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: 100% data deleted across all storage layers on erasure request
  - Normalisation Formula (0–100): Score = Deletion Rate % [gate: 100%]
  - Execution Frequency: Per Sprint / Pre-Release


### Non-Consented Tracker Count

- **Metric:** Non-Consented Tracker Count
- **Metric Category:** L1 Security & Compliance · L2 GDPR Compliance · L3 Cookie & Consent Testing · L4 Cookie Consent Compliance
- **Excel Definition:** Tests that no tracking cookies or analytics scripts load before explicit user consent is obtained
- **Required Evidence:** Absence of third-party tracking scripts
- **Project Component Producing Evidence:** Frontend `index.html` and `package.json` — no analytics SDKs
- **Source of Evidence:** Primary Tool: OneTrust Cookie Consent; Secondary Tool: Cookiebot; Languages Supported: JS, TS, JSX, TSX, Vue, HTML
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivations: count(trackers detected before consent event fired)
  - Raw Measurement Formula: Non-Consented Count = Number of analytics/tracking scripts loading before consent signal
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 non-consented trackers firing before consent
  - Normalisation Formula (0–100): MAX(0, 100 – (Non_Consented_Count × 20))
  - Execution Frequency: Per Sprint / Pre-Release


## Compliance URL (API Service)

### OWASP High/Critical Finding Count

- **Metric:** OWASP High/Critical Finding Count
- **Metric Category:** L1 Compliance Testing · L2 OWASP Testing · L3 DAST — Dynamic Application Security Testing · L4 OWASP Top 10 Vulnerability Scan
- **Excel Definition:** Dynamic scan of running application to identify OWASP Top 10 vulnerabilities: injection, broken auth, SSRF, insecure design, etc.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: ZAP Attack Proxy (OWASP); Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(risk == 'High')
- **Expected Positive Condition:** (blank in Excel)


### SQLi Vulnerability Count

- **Metric:** SQLi Vulnerability Count
- **Metric Category:** L1 Compliance Testing · L2 OWASP Testing · L3 DAST — Dynamic Application Security Testing · L4 SQL Injection Testing
- **Excel Definition:** Targeted dynamic testing for SQL injection vulnerabilities in all user-input paths and API parameters.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: ZAP Attack Proxy (OWASP); Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: CWE-89 / CWE-564 — count(cweid in (89,564) OR alert contains SQLi keywords)
- **Expected Positive Condition:** (blank in Excel)


### XSS Vulnerability Count

- **Metric:** XSS Vulnerability Count
- **Metric Category:** L1 Compliance Testing · L2 OWASP Testing · L3 DAST — Dynamic Application Security Testing · L4 XSS Testing
- **Excel Definition:** Dynamic testing for Cross-Site Scripting vulnerabilities in all reflected, stored, and DOM-based injection points.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: ZAP Attack Proxy (OWASP); Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: CWE-79 / CWE-83 — count(cweid in (79,83) OR alert contains 'xss','cross site scripting','reflected xss','stored xss')
- **Expected Positive Condition:** (blank in Excel)


### Unauthorized Access Count

- **Metric:** Unauthorized Access Count
- **Metric Category:** L1 Compliance Testing · L2 OWASP Testing · L3 Authentication & Authorization Testing · L4 Broken Access Control Testing
- **Excel Definition:** Tests for broken access control vulnerabilities — horizontal/vertical privilege escalation, IDOR, missing function-level access control.
- **Required Evidence:** Live API plus parameterized data access
- **Project Component Producing Evidence:** EF Core LINQ repositories, JWT auth, `SecurityHeadersMiddleware`, API tests
- **Source of Evidence:** Primary Tool: ZAP Attack Proxy (OWASP); Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: CWE-284/285/639/862 — count(cweid in (284,285,639,862) OR alert contains 'idor','authorization bypass','privilege escalation')
- **Expected Positive Condition:** (blank in Excel)


### CHD Exposure Finding Count

- **Metric:** CHD Exposure Finding Count
- **Metric Category:** L1 Compliance Testing · L2 PCI-DSS Compliance · L3 Payment Data Security · L4 Cardholder Data Exposure Scan
- **Excel Definition:** Detects unmasked or improperly transmitted cardholder data (PAN, track data, sensitive authentication data) in runtime HTTP/network traffic, logs, or API payloads.
- **Required Evidence:** API payloads limited to fields the application stores
- **Project Component Producing Evidence:** Response contracts (`ApplicantResponse`, `AuthResponse`) — no payment-card fields exist to expose
- **Source of Evidence:** Primary Tool: OWASP ZAP; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(alerts where response_body matches PAN_regex)
- **Expected Positive Condition:** (blank in Excel)


### Weak TLS Config Count

- **Metric:** Weak TLS Config Count
- **Metric Category:** L1 Compliance Testing · L2 PCI-DSS Compliance · L3 TLS/Encryption Compliance · L4 TLS Configuration Testing
- **Excel Definition:** Validates that all payment-related endpoints use TLS 1.2+ and strong cipher suites; no deprecated SSL/TLS versions.
- **Required Evidence:** Security headers on API responses; TLS at the host
- **Project Component Producing Evidence:** `SecurityHeadersMiddleware`, `UseHttpsRedirection`, `UseHsts`
- **Source of Evidence:** Primary Tool: testssl.sh (OSS); Validation Type: Dynamic TLS Scan; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(findings where protocol in [SSLv3, TLS1.0, TLS1.1] OR cipher_strength = 'weak')
- **Expected Positive Condition:** (blank in Excel)


### Age-Gate Bypass Count

- **Metric:** Age-Gate Bypass Count
- **Metric Category:** L1 Compliance Testing · L2 GDPR Compliance · L3 Consent & Age-Gate Compliance · L4 COPPA Age Verification Testing
- **Excel Definition:** Tests for COPPA age-gate bypass vulnerabilities — ensures under-13 users cannot access restricted content without parental consent.
- **Required Evidence:** Age-restricted access control
- **Project Component Producing Evidence:** Requirement clarification needed — the workbook does not define an age gate for this application
- **Source of Evidence:** Primary Tool: Playwright; Validation Type: Dynamic E2E Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: Age_Gate_Bypass_Count = count(test_cases where underage_user_access == allowed)
- **Expected Positive Condition:** (blank in Excel)


### Data Deletion Verification Rate

- **Metric:** Data Deletion Verification Rate
- **Metric Category:** L1 Compliance Testing · L2 GDPR Compliance · L3 Data Subject Rights Testing · L4 Right-to-Erasure Testing
- **Excel Definition:** Validates that user data deletion requests (GDPR Art. 17 Right to Erasure) are fully executed across all storage layers.
- **Required Evidence:** Confirmed erasure of applicant records and dependents
- **Project Component Producing Evidence:** `ApplicantsController.Erase`, `ApplicantService.EraseAsync`, Profile page confirmation
- **Source of Evidence:** Primary Tool: Postman; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (deletion requests fully executed across all storage layers / total deletion requests) × 100
- **Expected Positive Condition:** (blank in Excel)


### Non-Consented Tracker Count

- **Metric:** Non-Consented Tracker Count
- **Metric Category:** L1 Compliance Testing · L2 GDPR Compliance · L3 Cookie & Consent Testing · L4 Cookie Consent Compliance
- **Excel Definition:** Tests that no tracking cookies or analytics scripts load before explicit user consent is obtained.
- **Required Evidence:** Absence of third-party tracking scripts
- **Project Component Producing Evidence:** Frontend `index.html` and `package.json` — no analytics SDKs
- **Source of Evidence:** Primary Tool: OneTrust Cookie Consent; Validation Type: Dynamic Browser Scan; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(trackers detected before consent_event_fired)
- **Expected Positive Condition:** (blank in Excel)


### Unauthenticated API Endpoint Count

- **Metric:** Unauthenticated API Endpoint Count
- **Metric Category:** L1 Compliance Testing · L2 API Security · L3 API Endpoint Validation · L4 Unauthenticated Endpoint Testing
- **Excel Definition:** Identifies API endpoints accessible without a valid authentication token.
- **Required Evidence:** Authenticated endpoints with object-level checks
- **Project Component Producing Evidence:** `ApplicationsController`, `ApplicantsController`, `AuthorizationApiTests`
- **Source of Evidence:** Primary Tool: OWASP ZAP / 42Crunch; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(endpoints returning 200 without Authorization header)
- **Expected Positive Condition:** (blank in Excel)


### BOLA Finding Count

- **Metric:** BOLA Finding Count
- **Metric Category:** L1 Compliance Testing · L2 API Security · L3 Authorization Testing · L4 BOLA / IDOR Testing
- **Excel Definition:** Tests whether User A can access User B's resources via manipulated object references in API calls.
- **Required Evidence:** Authenticated endpoints with object-level checks
- **Project Component Producing Evidence:** `ApplicationsController`, `ApplicantsController`, `AuthorizationApiTests`
- **Source of Evidence:** Primary Tool: Postman / ZAP; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(requests where user_a accesses user_b_resource AND response == 200)
- **Expected Positive Condition:** (blank in Excel)


### APIs Without Rate Limiting Count

- **Metric:** APIs Without Rate Limiting Count
- **Metric Category:** L1 Compliance Testing · L2 API Security · L3 Rate Limiting Compliance · L4 Rate Limit Enforcement Testing
- **Excel Definition:** Verifies all public-facing API endpoints enforce request rate limiting to prevent abuse and DDoS.
- **Required Evidence:** HTTP 429 once a request budget is exceeded
- **Project Component Producing Evidence:** `AddApiRateLimiting` + `RateLimitingApiTests` (permit/window values are provisional — see docs/clarifications.md C-05)
- **Source of Evidence:** Primary Tool: Postman / Newman; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(endpoints where 429 not returned after threshold_requests)
- **Expected Positive Condition:** (blank in Excel)
- **Requirement clarification needed:**
  - The rate-limit request count and time window are not stated in the workbook.


### Missing Security Header Count

- **Metric:** Missing Security Header Count
- **Metric Category:** L1 Compliance Testing · L2 API Security · L3 Transport Security · L4 Security Header Validation
- **Excel Definition:** Checks that all API responses include required security headers: HSTS, X-Frame-Options, CSP, X-Content-Type.
- **Required Evidence:** Security headers on API responses; TLS at the host
- **Project Component Producing Evidence:** `SecurityHeadersMiddleware`, `UseHttpsRedirection`, `UseHsts`
- **Source of Evidence:** Primary Tool: OWASP ZAP; Validation Type: Dynamic DAST; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(responses missing [Strict-Transport-Security, X-Frame-Options, Content-Security-Policy])
- **Expected Positive Condition:** (blank in Excel)


### PII in API Response Count

- **Metric:** PII in API Response Count
- **Metric Category:** L1 Compliance Testing · L2 API Security · L3 PII in API Responses · L4 PII Exposure in Live Responses
- **Excel Definition:** Intercepts live API responses and detects PII (SSN, email, student ID) returned unnecessarily in payloads.
- **Required Evidence:** API payloads limited to fields the application stores
- **Project Component Producing Evidence:** Response contracts (`ApplicantResponse`, `AuthResponse`) — no payment-card fields exist to expose
- **Source of Evidence:** Primary Tool: Presidio + mitmproxy; Validation Type: Dynamic Proxy Scan; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(responses where Presidio detects entity_type in [US_SSN, EMAIL_ADDRESS, PERSON, STUDENT_ID])
- **Expected Positive Condition:** (blank in Excel)


### Session Timeout Compliance Rate

- **Metric:** Session Timeout Compliance Rate
- **Metric Category:** L1 Compliance Testing · L2 API Security · L3 Session Management Testing · L4 Session Timeout Compliance
- **Excel Definition:** Validates that sessions expire after the policy-defined idle period and tokens cannot be reused post-expiry.
- **Required Evidence:** Expired tokens rejected
- **Project Component Producing Evidence:** `JwtBearer` `ClockSkew = TimeSpan.Zero` and `AccessTokenLifetimeMinutes` (provisional — C-04)
- **Source of Evidence:** Primary Tool: Postman / Playwright; Validation Type: Dynamic API Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (sessions expired within policy window / total sessions tested) × 100
- **Expected Positive Condition:** (blank in Excel)


### Evidence Collection Rate %

- **Metric:** Evidence Collection Rate %
- **Metric Category:** L1 Compliance Testing · L2 SOC 2 Compliance · L3 Audit Evidence Completeness · L4 SOC 2 Evidence Collection Rate
- **Excel Definition:** % of SOC 2 Type II control evidence items collected and uploaded for audit period (access logs, change logs, test results, incident reports).
- **Required Evidence:** Audit artefacts from tests and logs
- **Project Component Producing Evidence:** xUnit + Vitest output, Swagger document, structured logging of surrogate keys
- **Source of Evidence:** Primary Tool: Drata; Validation Type: Platform / SaaS; Requires Live App?: Yes — integrations
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (evidence items collected / total required control evidence items) × 100
- **Expected Positive Condition:** (blank in Excel)


### Unreviewed Change Count

- **Metric:** Unreviewed Change Count
- **Metric Category:** L1 Compliance Testing · L2 SOC 2 Compliance · L3 Change Management Testing · L4 Change Control Verification
- **Excel Definition:** Count of code changes merged without required review approvals, bypassing change control gates.
- **Required Evidence:** Pull-request and merge history on the hosting provider
- **Project Component Producing Evidence:** Git branch `Scholarship-CMGroups-positive` (PR history is produced after review on the host)
- **Source of Evidence:** Primary Tool: GitHub (branch protection API); Validation Type: Dynamic API Query; Requires Live App?: Yes — GitHub API
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(merged PRs where required_reviewers == 0 OR review_bypassed == true)
- **Expected Positive Condition:** (blank in Excel)
- **Requirement clarification needed:**
  - Requires pull-request history on the hosting provider.


## Performance Code (Repository)

### Cyclomatic Complexity (Performance Hotspots)

- **Metric:** Cyclomatic Complexity (Performance Hotspots)
- **Metric Category:** L1 Performance Testing · L2 Static Analysis · L3 Algorithmic Complexity · L4 Complexity-Based Performance Risk
- **Excel Definition:** Flags functions with cyclomatic complexity > 15 that are likely to become performance bottlenecks — high complexity correlates with slow execution paths.
- **Required Evidence:** Source of performance-sensitive modules
- **Project Component Producing Evidence:** C# services/repositories and TypeScript client; EF Core queries; ESLint unused checks
- **Source of Evidence:** Validation Type: Static Code Scan; Requires Live App?: No; File / Artifact Scanned: Source code files (.py, .js, .ts, .java, .cs, .php, .rb, .c, .cpp)
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: count(functions where cyclomatic_complexity > 15)
  - Python :: Direct Metric: No
  - Python :: Derivation: count(functions where cyclomatic_complexity > 15)
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: count(functions where cyclomatic_complexity > 15)
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: count(functions where cyclomatic_complexity > 15)
  - C# :: Direct Metric: No
  - C# :: Derivation: count(functions where cyclomatic_complexity > 15)
  - Raw Measurement Formula: Hotspot Count = Count(Functions with CC > 15)
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 performance-critical functions with CC > 15
  - Normalisation Formula (0–100): MAX(0, 100 – (Hotspot_Count × 10))
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - The workbook does not state which modules count as performance-critical.


### Nested Loop Depth Count

- **Metric:** Nested Loop Depth Count
- **Metric Category:** L1 Performance Testing · L2 Static Analysis · L3 Algorithmic Complexity · L4 Big-O Complexity Review
- **Excel Definition:** Detects nested loops deeper than 3 levels in source code — O(n³) or worse complexity patterns that cause exponential performance degradation at scale.
- **Required Evidence:** Source of performance-sensitive modules
- **Project Component Producing Evidence:** C# services/repositories and TypeScript client; EF Core queries; ESLint unused checks
- **Source of Evidence:** Validation Type: Static Code Scan; Requires Live App?: No; File / Artifact Scanned: Source code files
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: count(functions where max_nesting_depth_of_loops >= 3)
  - Python :: Direct Metric: No
  - Python :: Derivation: count(functions where max_nested_depth >= 3)
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: count(functions where nested_loop_depth >= 3)
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: count(functions where nested_loop_depth >= 3)
  - C# :: Direct Metric: No
  - C# :: Derivation: count(methods where nested_loop_depth >= 3)
  - Raw Measurement Formula: Nested Loop Risk = Count(Functions with loop nesting >= 3)×15
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 functions with nested loop depth >= 3 in hot code paths
  - Normalisation Formula (0–100): MAX(0, 100 – Nested_Loop_Risk)
  - Execution Frequency: Every Commit / PR


### N+1 Query Anti-Pattern Count

- **Metric:** N+1 Query Anti-Pattern Count
- **Metric Category:** L1 Performance Testing · L2 Static Analysis · L3 Database Query Analysis · L4 N+1 Query Pattern Detection
- **Excel Definition:** Scans ORM code patterns for N+1 query anti-patterns — loops issuing individual DB queries instead of batch fetches, causing exponential DB load.
- **Required Evidence:** Source of performance-sensitive modules
- **Project Component Producing Evidence:** C# services/repositories and TypeScript client; EF Core queries; ESLint unused checks
- **Source of Evidence:** Validation Type: Static SAST Scan; Requires Live App?: No; File / Artifact Scanned: Source code files using ORM frameworks (SQLAlchemy, Hibernate, ActiveRecord, EF Core)
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: count(DB_queries_inside_loops_without_batch_fetch)
  - Python :: Direct Metric: No
  - Python :: Derivation: count(ORM_queries_inside_loops_without_prefetch)
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: count(query_patterns_inside_async_loops)
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: count(TypeORM_queries_inside_loops)
  - C# :: Direct Metric: No
  - C# :: Derivation: count(DB_query_patterns_inside_loops_without_batch_fetch)
  - Raw Measurement Formula: N+1 Risk Score = Count(N+1 Patterns)×20
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 N+1 query patterns in production code paths
  - Normalisation Formula (0–100): MAX(0, 100 – N1_Risk_Score)
  - Execution Frequency: Every Commit / PR


### Large Allocation in Loop Count

- **Metric:** Large Allocation in Loop Count
- **Metric Category:** L1 Performance Testing · L2 Static Analysis · L3 Memory Management · L4 Memory Allocation Pattern Analysis
- **Excel Definition:** Identifies large object allocations or memory-intensive operations inside loops — patterns likely to cause GC pressure and heap exhaustion at scale.
- **Required Evidence:** Source of performance-sensitive modules
- **Project Component Producing Evidence:** C# services/repositories and TypeScript client; EF Core queries; ESLint unused checks
- **Source of Evidence:** Validation Type: Static Code Scan; Requires Live App?: No; File / Artifact Scanned: Source code files
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: count(object_creation_inside_loops)
  - Python :: Direct Metric: No
  - Python :: Derivation: count(large_alloc_or_list_concat_inside_loop)
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: count(array_or_object_allocations_inside_loops)
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: count(memory_intensive_operations_inside_loops)
  - C# :: Direct Metric: No
  - C# :: Derivation: count(memory_intensive_allocations_inside_loops)
  - Raw Measurement Formula: Memory Pressure Score = Count(Large Allocations in Loops)×15
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 large memory allocations inside unbounded loops
  - Normalisation Formula (0–100): MAX(0, 100 – Memory_Pressure_Score)
  - Execution Frequency: Every Commit / PR


### Race Condition Risk Count

- **Metric:** Race Condition Risk Count
- **Metric Category:** L1 Performance Testing · L2 Static Analysis · L3 Concurrency Analysis · L4 Thread-Safety Pattern Detection
- **Excel Definition:** Scans shared mutable state accessed from multiple threads without proper synchronization — detects potential race conditions before runtime.
- **Required Evidence:** Source of performance-sensitive modules
- **Project Component Producing Evidence:** C# services/repositories and TypeScript client; EF Core queries; ESLint unused checks
- **Source of Evidence:** Validation Type: Static SAST Scan; Requires Live App?: No; File / Artifact Scanned: Source code files
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: count(shared_mutable_state_without_synchronization)
  - Python :: Direct Metric: No
  - Python :: Derivation: count(shared_state_access_without_lock)
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: count(async_shared_state_mutations)
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: count(unsafe_parallel_async_updates)
  - C# :: Direct Metric: No
  - C# :: Derivation: count(thread_unsafe_shared_resource_access)
  - Raw Measurement Formula: Race Risk Score = Count(Unguarded Shared State)×20
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 unguarded shared mutable state in concurrent code
  - Normalisation Formula (0–100): MAX(0, 100 – Race_Risk_Score)
  - Execution Frequency: Every Commit / PR


### Unused Import Count

- **Metric:** Unused Import Count
- **Metric Category:** L1 Performance Testing · L2 Dependency Analysis · L3 Bundle Size Analysis · L4 Unused Dependency Detection
- **Excel Definition:** Detects imported libraries never used in the codebase — dead imports inflate bundle size and slow application startup unnecessarily.
- **Required Evidence:** Source of performance-sensitive modules
- **Project Component Producing Evidence:** C# services/repositories and TypeScript client; EF Core queries; ESLint unused checks
- **Source of Evidence:** Validation Type: Static Code Scan; Requires Live App?: No; File / Artifact Scanned: Source code files, package.json
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: count(unused_import_statements)
  - Python :: Direct Metric: No
  - Python :: Derivation: count(unused_imports)
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: count(unused_dependencies)
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: count(unused_imports_or_exports)
  - C# :: Direct Metric: No
  - C# :: Derivation: count(unused_namespaces)
  - Raw Measurement Formula: Dead Import % = (Unused Imports / Total Imports) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 2% unused imports across codebase
  - Normalisation Formula (0–100): MAX(0, 100 – (Dead_Import% × 25))
  - Execution Frequency: Every Commit / PR


### Build Duration (seconds)

- **Metric:** Build Duration (seconds)
- **Metric Category:** L1 Performance Testing · L2 Dependency Analysis · L3 Build Performance · L4 Build Time Regression
- **Excel Definition:** Measures total CI build time — slow builds increase feedback loop latency, reducing developer velocity and deployment frequency.
- **Required Evidence:** Timed `dotnet build` / `npm run build` runs
- **Project Component Producing Evidence:** `ScholarshipCMGroups.sln` and Vite `package.json` scripts
- **Source of Evidence:** Validation Type: Static CI Metadata; Requires Live App?: No; File / Artifact Scanned: CI pipeline logs / build manifests
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: build_end_timestamp - build_start_timestamp
  - Python :: Direct Metric: No
  - Python :: Derivation: build_end_timestamp - build_start_timestamp
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: build_end_timestamp - build_start_timestamp
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: build_end_timestamp - build_start_timestamp
  - C# :: Direct Metric: No
  - C# :: Derivation: build_end_timestamp - build_start_timestamp
  - Raw Measurement Formula: Build Regression = ((Build_Current – Build_Baseline) / Build_Baseline) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 5% build time regression from baseline; absolute < 10 minutes
  - Normalisation Formula (0–100): MAX(0, 100 – (Build_Regression% × 5))
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires a previous baseline measurement, which a new repository does not have.


### Circular Dependency Count

- **Metric:** Circular Dependency Count
- **Metric Category:** L1 Performance Testing · L2 Dependency Analysis · L3 Dependency Graph Analysis · L4 Circular Dependency Detection
- **Excel Definition:** Identifies circular import/dependency chains in the module graph — circular deps slow module resolution and cause unpredictable initialization order.
- **Required Evidence:** Source of performance-sensitive modules
- **Project Component Producing Evidence:** C# services/repositories and TypeScript client; EF Core queries; ESLint unused checks
- **Source of Evidence:** Validation Type: Static Code Scan; Requires Live App?: No; File / Artifact Scanned: Source code files, module imports
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: count(circular_package_dependencies)
  - Python :: Direct Metric: No
  - Python :: Derivation: count(cyclic_module_imports)
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: count(circular_module_dependencies)
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: count(circular_dependency_cycles)
  - C# :: Direct Metric: No
  - C# :: Derivation: count(cyclic_assembly_dependencies)
  - Raw Measurement Formula: Circular Dep Score = Count(Circular Dependency Cycles)×15
- **Expected Positive Condition:**
  - Expected Value / Threshold: 0 circular dependency cycles in module graph
  - Normalisation Formula (0–100): MAX(0, 100 – Circular_Dep_Score)
  - Execution Frequency: Daily


### Churn Score (Performance Modules)

- **Metric:** Churn Score (Performance Modules)
- **Metric Category:** L1 Performance Testing · L2 Code Quality · L3 Technical Debt · L4 Code Churn in Performance-Critical Paths
- **Excel Definition:** Measures rate of change in performance-critical modules — high churn in hot paths increases regression risk for latency and throughput.
- **Required Evidence:** Git history over the window named in Excel
- **Project Component Producing Evidence:** Meaningful commits on `Scholarship-CMGroups-positive`
- **Source of Evidence:** Validation Type: Static Git Analysis; Requires Live App?: No; File / Artifact Scanned: Git history for files tagged as performance-critical
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: (lines_added + lines_deleted) / total_LOC over rolling_window
  - Python :: Direct Metric: No
  - Python :: Derivation: (lines_added + lines_deleted) / total_LOC over rolling_window
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: (lines_added + lines_deleted) / total_LOC over rolling_window
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: (lines_added + lines_deleted) / total_LOC over rolling_window
  - C# :: Direct Metric: No
  - C# :: Derivation: (lines_added + lines_deleted) / total_LOC over rolling_window
  - Raw Measurement Formula: Perf Churn Score = Churn% × Performance_Module_Weight
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 20% churn rate per sprint in performance-critical modules
  - Normalisation Formula (0–100): MAX(0, 100 – (Perf_Churn% × 3))
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - The weight and the list of performance-critical modules are not stated in the workbook.
  - The workbook does not state which modules count as performance-critical.


### Performance Test Coverage %

- **Metric:** Performance Test Coverage %
- **Metric Category:** L1 Performance Testing · L2 Code Quality · L3 Test Coverage · L4 Performance Test Code Coverage
- **Excel Definition:** % of performance-critical code paths covered by dedicated performance/load test scripts — ensures regressions are detectable.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Validation Type: Static Coverage Report; Requires Live App?: No; File / Artifact Scanned: Coverage report artifacts (.json, .xml)
- **Calculation/Derivation:**
  - Java :: Direct Metric: No
  - Java :: Derivation: (performance_covered_lines / total_performance_critical_lines) * 100
  - Python :: Direct Metric: No
  - Python :: Derivation: (performance_covered_lines / total_performance_critical_lines) * 100
  - JavaScript :: Direct Metric: No
  - JavaScript :: Derivation: (performance_covered_lines / total_performance_critical_lines) * 100
  - TypeScript :: Direct Metric: No
  - TypeScript :: Derivation: (performance_covered_lines / total_performance_critical_lines) * 100
  - C# :: Direct Metric: No
  - C# :: Derivation: (performance_covered_lines / total_performance_critical_lines) * 100
  - Raw Measurement Formula: Perf Coverage % = (Perf-Critical Lines Covered / Total Perf-Critical Lines) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 70% of performance-critical paths covered by load tests
  - Normalisation Formula (0–100): Score = Perf_Coverage % [gate at 70%]
  - Execution Frequency: Daily
- **Requirement clarification needed:**
  - The workbook does not state which modules count as performance-critical.


## Performance URL (API Service)

### Throughput Under Load (RPS)

- **Metric:** Throughput Under Load (RPS)
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Load Signal Delta · L4 Load Testing
- **Excel Definition:** Requests per second achieved under defined load profile vs target SLA throughput — validates system capacity at expected peak load.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter (Apache); Optional Tool: Gatling (OSS) / Locust (Python); Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: http_reqs
  - Raw Measurement Formula: Throughput = Total Successful Requests / Test Duration (seconds)
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= target RPS with p95 latency within SLA threshold
  - Normalisation Formula (0–100): Score = MIN(100, (Achieved_RPS / Target_RPS) × 100)
  - Execution Frequency: Weekly
- **Requirement clarification needed:**
  - Target RPS is not stated in the workbook.


### Peak Load Degradation %

- **Metric:** Peak Load Degradation %
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Load Signal Delta · L4 Stress Testing
- **Excel Definition:** The load level at which system performance degrades below acceptable thresholds — identifies breaking point and measures response time increase under stress.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Locust (step load) / Gatling (open model); Validation Type: Dynamic Stress Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: ((p95_stress – p95_baseline) / p95_baseline) × 100
  - Raw Measurement Formula: Degradation % = ((Latency_Under_Stress – Latency_Baseline) / Latency_Baseline) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: Degradation < 20% response time increase at 2× expected peak load
  - Normalisation Formula (0–100): MAX(0, 100 – Degradation%) [each % = –1 pt]
  - Execution Frequency: Per Sprint / Pre-Release
- **Requirement clarification needed:**
  - Requires a previous baseline measurement, which a new repository does not have.


### Performance Regression Delta %

- **Metric:** Performance Regression Delta %
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Load Signal Delta · L4 Baseline Comparison Testing
- **Excel Definition:** % change in p95 response time between current build and established performance baseline — detects regressions introduced by code changes.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Gatling reports / Locust HTML report with history; Validation Type: Dynamic Baseline Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: ((p95_current – p95_baseline) / p95_baseline) × 100
  - Raw Measurement Formula: Regression Delta = ((p95_Current – p95_Baseline) / p95_Baseline) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 10% regression from baseline (delta <= 10%)
  - Normalisation Formula (0–100): MAX(0, 100 – (Regression_Delta × 5)) [each 1% = –5 pts]
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - Requires a previous baseline measurement, which a new repository does not have.


### Max Concurrent Users (VU)

- **Metric:** Max Concurrent Users (VU)
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Load Signal Delta · L4 Concurrency Testing
- **Excel Definition:** Maximum number of virtual users the system can handle simultaneously while maintaining response times within SLA — measures horizontal scalability.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Gatling; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: vus_max
  - Raw Measurement Formula: Max VU = Peak concurrent users at which p95 latency breaches SLA threshold
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= defined VU target (domain-specific SLA e.g. 1000 VU for web API)
  - Normalisation Formula (0–100): Score = MIN(100, (Achieved_VU / Target_VU) × 100)
  - Execution Frequency: Per Sprint / Pre-Release
- **Requirement clarification needed:**
  - Target virtual-user count is not stated in the workbook.
  - The workbook gives an example, not a value for this project.


### 4xx/5xx Error Rate %

- **Metric:** 4xx/5xx Error Rate %
- **Metric Category:** L1 Performance Testing · L2 Reliability Testing · L3 Error State Stability · L4 Error Rate Monitoring
- **Excel Definition:** % of HTTP requests returning 4xx or 5xx status codes during test run — measures system stability under load.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Locust / Prometheus + Grafana / Newman; Validation Type: Dynamic Reliability Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: http_req_failed
  - Raw Measurement Formula: Error Rate = (4xx + 5xx Responses / Total Responses) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 1% error rate per build; 0 SLA breaches per sprint
  - Normalisation Formula (0–100): MAX(0, 100 – (Error_Rate × 20)) [5% = 0 pts]
  - Execution Frequency: Every Commit / PR


### p95 Response Time (ms)

- **Metric:** p95 Response Time (ms)
- **Metric Category:** L1 Performance Testing · L2 Reliability Testing · L3 Latency Consistency · L4 p95 Latency Testing
- **Excel Definition:** 95th percentile response time during load test — captures tail latency invisible in averages; critical for real user experience.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Gatling / Locust / Apache JMeter; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: p(95)
  - Raw Measurement Formula: p95 = 95th percentile of response time distribution from load test
- **Expected Positive Condition:**
  - Expected Value / Threshold: p95 <= defined SLA (< 500ms web APIs; < 200ms search/browse)
  - Normalisation Formula (0–100): MAX(0, 100 – ((p95_ms – SLA_ms) / SLA_ms × 100))
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - The latency SLA per endpoint category is not stated in the workbook.


### p99 Response Time (ms)

- **Metric:** p99 Response Time (ms)
- **Metric Category:** L1 Performance Testing · L2 Reliability Testing · L3 Latency Consistency · L4 p99 Latency Testing
- **Excel Definition:** 99th percentile response time — extreme tail latency; identifies outlier request handling issues affecting the worst-off 1% of users.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Gatling / Locust / Jaeger; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: p(99)
  - Raw Measurement Formula: p99 = 99th percentile of response time distribution from load test
- **Expected Positive Condition:**
  - Expected Value / Threshold: p99 <= 2× p95 SLA (signals no extreme outlier spikes)
  - Normalisation Formula (0–100): MAX(0, 100 – ((p99_ms – 2×SLA_ms) / SLA_ms × 100))
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - The latency SLA per endpoint category is not stated in the workbook.


### Mean Response Time (ms)

- **Metric:** Mean Response Time (ms)
- **Metric Category:** L1 Performance Testing · L2 Reliability Testing · L3 Latency Consistency · L4 Mean Response Time Monitoring
- **Excel Definition:** Average response time across all requests during load test — baseline latency metric for tracking improvement trends over time.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Gatling; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: http_req_duration{avg}
  - Raw Measurement Formula: Mean RT = sum(response_times) / count(requests)
- **Expected Positive Condition:**
  - Expected Value / Threshold: Mean RT <= 50% of p95 SLA (healthy distribution)
  - Normalisation Formula (0–100): MAX(0, 100 – ((Mean_ms – (SLA_ms × 0.5)) / (SLA_ms × 0.5) × 100))
  - Execution Frequency: Every Commit / PR
- **Requirement clarification needed:**
  - The latency SLA per endpoint category is not stated in the workbook.


### Successful Request Rate %

- **Metric:** Successful Request Rate %
- **Metric Category:** L1 Performance Testing · L2 Reliability Testing · L3 Throughput Consistency · L4 Request Success Rate
- **Excel Definition:** % of all requests completing successfully (2xx responses) under the defined load profile — direct measure of system reliability under pressure.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Gatling / Locust; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: http_req_failed (inverted)
  - Raw Measurement Formula: Success Rate = (2xx Responses / Total Requests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 99% successful request rate under target load profile
  - Normalisation Formula (0–100): Score = Success_Rate % [gate at 99%]
  - Execution Frequency: Every Commit / PR


### Memory Leak Score

- **Metric:** Memory Leak Score
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Endurance Testing · L4 Soak Testing
- **Excel Definition:** Monitors memory usage during extended load run (2–8 hrs) to detect memory leaks and gradual resource degradation under sustained traffic.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Locust / Gatling / JVM heap profiling / memory_profiler (Python); Validation Type: Dynamic Soak Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: memory_leak_proxy_score
  - Raw Measurement Formula: Memory Growth % = ((Mem_End – Mem_Start) / Mem_Start) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 10% memory growth over soak test duration; no OOM events
  - Normalisation Formula (0–100): MAX(0, 100 – (Memory_Growth% × 5)) [20% = 0 pts]
  - Execution Frequency: Per Sprint / Pre-Release


### Average CPU Utilisation % (Soak)

- **Metric:** Average CPU Utilisation % (Soak)
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Soak Testing · L4 CPU Utilisation Monitoring
- **Excel Definition:** Average CPU utilisation across all application instances during extended soak test — detects CPU leaks and runaway thread consumption over time.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6 + Prometheus; Secondary Tool: JMeter + Grafana; Optional Tool: Locust + cAdvisor; Validation Type: Dynamic Soak Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: avg(cpu_usage_percent) over soak_duration
  - Raw Measurement Formula: CPU Utilisation = avg(cpu_percent) over test_window
- **Expected Positive Condition:**
  - Expected Value / Threshold: Average CPU < 70% sustained; no spikes > 90% during soak
  - Normalisation Formula (0–100): MAX(0, 100 – ((avg_cpu – 70) × 3))
  - Execution Frequency: Per Sprint / Pre-Release


### Spike Recovery Time (s)

- **Metric:** Spike Recovery Time (s)
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Spike Testing · L4 Traffic Surge Handling
- **Excel Definition:** Time taken for the system to recover to normal response times after a sudden traffic spike — measures autoscaling effectiveness and resilience.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Locust (step load) / Gatling (closed model); Validation Type: Dynamic Spike Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: spike_recovery_time_s
  - Raw Measurement Formula: Recovery Time = Time from spike end to when p95 returns to baseline ± 10%
- **Expected Positive Condition:**
  - Expected Value / Threshold: Recovery within 30 seconds after spike subsides; 0 errors during spike
  - Normalisation Formula (0–100): MAX(0, 100 – (Recovery_Seconds × 2)) [50s = 0 pts]
  - Execution Frequency: Per Sprint / Pre-Release
- **Requirement clarification needed:**
  - Requires a previous baseline measurement, which a new repository does not have.


### Spike Error Rate %

- **Metric:** Spike Error Rate %
- **Metric Category:** L1 Performance Testing · L2 Performance Testing · L3 Spike Testing · L4 Error Rate During Spike
- **Excel Definition:** % of requests failing (4xx/5xx) during the spike phase — measures graceful degradation capability when the system is overwhelmed.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Locust; Validation Type: Dynamic Spike Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: http_req_failed (spike phase)
  - Raw Measurement Formula: Spike Error Rate = (Failed Requests During Spike / Total Spike Requests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 5% error rate during spike; recovers to < 1% within 30s
  - Normalisation Formula (0–100): MAX(0, 100 – (Spike_Error_Rate × 10))
  - Execution Frequency: Per Sprint / Pre-Release


### Top-N Slowest Endpoints (p95 ms)

- **Metric:** Top-N Slowest Endpoints (p95 ms)
- **Metric Category:** L1 Performance Testing · L2 API Performance · L3 Endpoint Latency Profiling · L4 Slowest Endpoint Detection
- **Excel Definition:** Identifies the slowest API endpoints by p95 latency — pinpoints where optimisation effort delivers the highest user experience improvement.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6; Secondary Tool: JMeter; Optional Tool: Jaeger / OpenTelemetry; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: Yes
  - Derivation: http_req_duration grouped by URL
  - Raw Measurement Formula: Endpoint Latency = p95(response_time) per unique endpoint
- **Expected Positive Condition:**
  - Expected Value / Threshold: No endpoint > 2× average p95 latency; 0 endpoints breaching SLA
  - Normalisation Formula (0–100): MAX(0, 100 – (Count(SLA_Breaching_Endpoints) × 15))
  - Execution Frequency: Weekly


### Connection Pool Saturation %

- **Metric:** Connection Pool Saturation %
- **Metric Category:** L1 Performance Testing · L2 API Performance · L3 Connection Management · L4 Connection Pool Exhaustion Testing
- **Excel Definition:** Measures database and HTTP connection pool utilisation under load — pool exhaustion causes queuing, timeout errors, and cascading failures.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6 + DB metrics; Secondary Tool: JMeter + Prometheus; Optional Tool: Grafana; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (active_connections / pool_max_connections) × 100
  - Raw Measurement Formula: Saturation % = (Peak Active Connections / Max Pool Size) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: < 80% connection pool saturation at target load; 0 pool exhaustion events
  - Normalisation Formula (0–100): MAX(0, 100 – ((Saturation – 80) × 5))
  - Execution Frequency: Weekly


### Cache Hit Rate %

- **Metric:** Cache Hit Rate %
- **Metric Category:** L1 Performance Testing · L2 API Performance · L3 Caching Effectiveness · L4 Cache Hit Rate Testing
- **Excel Definition:** % of requests served from cache vs origin — low cache hit rates increase backend load, latency, and infrastructure cost under production traffic.
- **Required Evidence:** Live API under load against a stated SLA/target
- **Project Component Producing Evidence:** Runnable API (`ScholarshipCMGroups.Api`) — load targets themselves are not in the workbook (clarifications C-08)
- **Source of Evidence:** Primary Tool: K6 + Redis metrics; Secondary Tool: JMeter + Prometheus; Optional Tool: Grafana + cAdvisor; Validation Type: Dynamic Load Test; Requires Live App?: Yes
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: (cache_hits / (cache_hits + cache_misses)) × 100
  - Raw Measurement Formula: Cache Hit Rate = (Cache Hits / Total Requests) × 100
- **Expected Positive Condition:**
  - Expected Value / Threshold: >= 80% cache hit rate for cacheable endpoint categories
  - Normalisation Formula (0–100): Score = Cache_Hit_Rate % [gate at 80%]
  - Execution Frequency: Weekly
- **Requirement clarification needed:**
  - The workbook does not state which endpoints are cacheable.


## Compliance Code (Repository)

### Secrets Exposed in Code Count

- **Metric:** Secrets Exposed in Code Count
- **Metric Category:** L1 Compliance Testing · L2 Static Code Analysis · L3 Secret Detection · L4 Hardcoded Secret Scan
- **Excel Definition:** Scans git history and source files for API keys, tokens, passwords, private keys, and certificates committed to the repository.
- **Required Evidence:** Repository contents and git history with no committed credentials
- **Project Component Producing Evidence:** `.gitignore`, empty `ConnectionStrings`/`SigningKey` in committed config, `dotnet user-secrets`, frontend `.env.example`
- **Source of Evidence:** Primary Tool: Gitleaks / Trufflehog; Validation Type: Static Repo Scan; Requires Live App?: No; File / Artifact Scanned: Git history, source files
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(findings where entropy > threshold OR matches secret_regex)
- **Expected Positive Condition:** (blank in Excel)


### Blocked Secret Commit Count

- **Metric:** Blocked Secret Commit Count
- **Metric Category:** L1 Compliance Testing · L2 Static Code Analysis · L3 Secret Detection · L4 Pre-Commit Secret Prevention
- **Excel Definition:** Counts number of secret commits blocked by pre-commit hook before reaching remote repository.
- **Required Evidence:** Repository contents and git history with no committed credentials
- **Project Component Producing Evidence:** `.gitignore`, empty `ConnectionStrings`/`SigningKey` in committed config, `dotnet user-secrets`, frontend `.env.example`
- **Source of Evidence:** Primary Tool: detect-secrets / Gitleaks; Validation Type: Static Pre-Commit Hook; Requires Live App?: No; File / Artifact Scanned: Staged git files
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(commit_attempts blocked by pre-commit hook in period)
- **Expected Positive Condition:** (blank in Excel)


### PII in Codebase Count

- **Metric:** PII in Codebase Count
- **Metric Category:** L1 Compliance Testing · L2 FERPA/COPPA Compliance · L3 Student PII Exposure Scan · L4 PII in Codebase Detection
- **Excel Definition:** Scans source files and test fixtures for hardcoded PII — SSN, student IDs, email addresses, grade data — in violation of FERPA.
- **Required Evidence:** Source and logs without personal data literals
- **Project Component Producing Evidence:** `LogMessages.cs` (IDs only), `database/seed/001-reference-scholarships.sql` (catalogue only)
- **Source of Evidence:** Primary Tool: Microsoft Presidio (fed files); Validation Type: Static PII Scan; Requires Live App?: No; File / Artifact Scanned: Source files, test data, fixtures
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(findings where entity_type in [US_SSN, EMAIL_ADDRESS, PERSON, STUDENT_ID] AND source in [codebase, test_fixtures])
- **Expected Positive Condition:** (blank in Excel)


### PII Log Statement Count

- **Metric:** PII Log Statement Count
- **Metric Category:** L1 Compliance Testing · L2 FERPA/COPPA Compliance · L3 PII Logging Detection · L4 PII in Log Statements
- **Excel Definition:** Detects log statements that print sensitive fields (SSN, email, PAN) — risks PII ending up in log files.
- **Required Evidence:** Source and logs without personal data literals
- **Project Component Producing Evidence:** `LogMessages.cs` (IDs only), `database/seed/001-reference-scholarships.sql` (catalogue only)
- **Source of Evidence:** Primary Tool: Semgrep (custom rules); Validation Type: Static SAST; Requires Live App?: No; File / Artifact Scanned: Source code files
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(log_statements where argument matches PII_field_names [ssn, email, pan, password, dob])
- **Expected Positive Condition:** (blank in Excel)


### Open Firewall Rule Count (IaC)

- **Metric:** Open Firewall Rule Count (IaC)
- **Metric Category:** L1 Compliance Testing · L2 Static Code Analysis · L3 IaC Security Scanning · L4 Open Security Group Rule Detection
- **Excel Definition:** Detects Terraform/CloudFormation resources defining security groups open to 0.0.0.0/0 on sensitive ports.
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov / tfsec; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: .tf, .yaml, .json IaC files
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(resources where ingress_cidr == '0.0.0.0/0' AND port not in [80, 443])
- **Expected Positive Condition:** (blank in Excel)


### Unencrypted Storage Count (IaC)

- **Metric:** Unencrypted Storage Count (IaC)
- **Metric Category:** L1 Compliance Testing · L2 Static Code Analysis · L3 IaC Security Scanning · L4 Unencrypted Storage Definition
- **Excel Definition:** Identifies RDS, S3, EBS, or GCS resources defined in IaC without encryption-at-rest enabled.
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov / tfsec; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: .tf, CloudFormation templates
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(resources where encryption_enabled == false OR storage_encrypted == false)
- **Expected Positive Condition:** (blank in Excel)


### Public Storage Bucket Count (IaC)

- **Metric:** Public Storage Bucket Count (IaC)
- **Metric Category:** L1 Compliance Testing · L2 Static Code Analysis · L3 IaC Security Scanning · L4 Publicly Exposed Resource Detection
- **Excel Definition:** Detects S3 buckets or GCS buckets defined in IaC with public access enabled.
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov / kics; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: .tf, CloudFormation templates
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(aws_s3_bucket where acl == 'public-read' OR block_public_access == false)
- **Expected Positive Condition:** (blank in Excel)


### CIS Benchmark Violation Count (IaC)

- **Metric:** CIS Benchmark Violation Count (IaC)
- **Metric Category:** L1 Compliance Testing · L2 Static Code Analysis · L3 IaC Security Scanning · L4 CIS Benchmark Compliance
- **Excel Definition:** Measures percentage of IaC configurations meeting CIS Benchmark baseline controls for AWS/GCP/Azure.
- **Required Evidence:** Infrastructure-as-code definitions
- **Project Component Producing Evidence:** This application does not ship cloud IaC; SQL Server is an external dependency configured outside the repo
- **Source of Evidence:** Primary Tool: Checkov; Validation Type: Static IaC Scan; Requires Live App?: No; File / Artifact Scanned: All IaC files in repo
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(checks failed against CIS benchmark ruleset) / total_checks × 100
- **Expected Positive Condition:** (blank in Excel)


### Unreviewed Change Count

- **Metric:** Unreviewed Change Count
- **Metric Category:** L1 Compliance Testing · L2 SOC 2 Compliance · L3 Change Management Testing · L4 Change Control Verification
- **Excel Definition:** Count of code changes merged without required review approvals, bypassing change control gates.
- **Required Evidence:** Pull-request and merge history on the hosting provider
- **Project Component Producing Evidence:** Git branch `Scholarship-CMGroups-positive` (PR history is produced after review on the host)
- **Source of Evidence:** Primary Tool: GitHub Branch Protection API; Validation Type: Static Git Metadata Query; Requires Live App?: No; File / Artifact Scanned: GitHub PR / audit log
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(merged PRs where required_reviewers == 0 OR review_bypassed == true)
- **Expected Positive Condition:** (blank in Excel)
- **Requirement clarification needed:**
  - Requires pull-request history on the hosting provider.


### Historical Secret Exposure Count

- **Metric:** Historical Secret Exposure Count
- **Metric Category:** L1 Compliance Testing · L2 SOC 2 Compliance · L3 Secret Management · L4 Secrets in Repo History
- **Excel Definition:** Full git history scan for secrets that may have been committed and later deleted — still present in git history.
- **Required Evidence:** Repository contents and git history with no committed credentials
- **Project Component Producing Evidence:** `.gitignore`, empty `ConnectionStrings`/`SigningKey` in committed config, `dotnet user-secrets`, frontend `.env.example`
- **Source of Evidence:** Primary Tool: Trufflehog; Validation Type: Static Git History Scan; Requires Live App?: No; File / Artifact Scanned: Full git commit history
- **Calculation/Derivation:**
  - Direct Metric: No
  - Derivation: count(commits in full history where secret_pattern matched AND NOT in HEAD)
- **Expected Positive Condition:** (blank in Excel)

