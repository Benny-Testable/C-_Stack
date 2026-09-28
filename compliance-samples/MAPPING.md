# White Box metric coverage map

All metrics under the same **L3 Technique** are different lenses on the *same*
underlying code property, so one compliant sample file satisfies every metric
in its group. This maps all 103 White Box metrics from the Testable strategy
sheet to the file (or practice) that satisfies them.

## Cyclomatic Complexity — 6 metrics → [`CleanFunctions.cs`](CleanFunctions.cs)
Flat, single-purpose methods, max complexity 3, no nested branching.
- Execution Path Integrity
- Decision Outcome Verification
- Logical Sub-expression Validation
- Total Logical Combinatorial Coverage
- Technical Debt Impact
- QA Resource Allocation

## Cognitive Complexity — 7 metrics → [`CleanFunctions.cs`](CleanFunctions.cs)
Same file: shallow nesting, no recursion, low reviewer/test effort.
- Technical Debt Impact
- Unit Test Complexity
- Defect Probability
- Modularization Opportunity
- Reviewer Fatigue Factor
- QA Resource Allocation
- Human Cognitive Load

## Code Duplication — 7 metrics → [`NoDuplication.cs`](NoDuplication.cs)
Shared logic abstracted into one method; zero cloned blocks/tokens.
- Multi-Point Failure Probability
- Redundancy Localization
- Structural Cleanliness Score
- Test Suite Streamlining
- Abstraction Potential
- Regression Focus Mapping
- Synchronization Verification

## Lint / Rule Violations — 12 metrics → [`LintClean.cs`](LintClean.cs)
Consistent naming/formatting, no unused vars, no unreachable code.
- Violation Density per KLOC
- Resource Waste Identification
- Semantic Consistency Score
- Syntactic Uniformity Score
- Structural Threshold Monitoring
- Impact Prioritization
- Aggregated Risk Assessment
- Accuracy Tuning
- Project-Specific Enforcement
- Environment Standardization
- Automated Gatekeeping
- Quality Audit Trail

## Static Vulnerabilities (SAST) — 7 metrics → [`SecureHandlers.cs`](SecureHandlers.cs)
Validated input, parameterized queries (EF Core), no secrets, no eval.
- Best Practice Compliance
- Entry Point Sanitization
- Sensitive Information Tracking
- Access Control Verification
- Supply Chain Security
- Regulatory Alignment
- Exploit Surface Identification

## Dependency Risk (SCA) — 8 metrics → [`server/src/NineBlock.Api/NineBlock.Api.csproj`](../server/src/NineBlock.Api/NineBlock.Api.csproj)
All package versions pinned to current stable releases with no known CVEs at
time of writing; keep this file's versions current to keep these metrics green.
- Hidden Relationship Mapping
- Legal Risk Validation
- Trust Integrity Verification
- Community Vitality Tracking
- Mitigation Effort Ranking
- Real-Time Alerting
- Known CVE Count
- Version Lag Assessment

## Statement / Branch / Path Coverage — 22 metrics → [`server/tests/NineBlock.Api.Tests/BoxPositionCalculatorTests.cs`](../server/tests/NineBlock.Api.Tests/BoxPositionCalculatorTests.cs)
`[Theory]` cases exercise every statement, every branch, and every path
(all 9 valid score combinations + 4 out-of-range combinations).
- Statement Coverage: Test Case Granularity, Unreachable Logic Identification, Coverage Gap Analysis, Surface-Level Correctness, Statement Coverage %
- Branch Coverage: Boolean Accuracy Check, Sequence Integrity Mapping, Iteration Boundary Verification, Boundary Failure Identification, Branch Misdirection Discovery, Decision Coverage Gap Analysis, Branch Coverage %
- Path Coverage: Full Logic Validation, Gap Identification, Deep Logic Probing, Iterative Route Analysis, Ghost Code Discovery, Error Flow Verification, Cross-Component Mapping, Automated Quality Enforcement, Path Coverage %

## Mutation Score / Coverage Delta — 13 metrics → same test file
Exact-value assertions (`Assert.Equal`, `Assert.Throws`) kill mutants instead
of just executing the line, which is what keeps mutation score high.
- Mutation Score: Logic Error Sensitivity, Test Rigor Assessment, Weak Spot Localization, Boundary Mutant Analysis
- Coverage Delta: Coverage Delta %, Discovery Power Assessment, Deployment Readiness Guard, Ripple Effect Mapping, Fresh Logic Proofing, Structural Health Benchmarking

## All-Definition / All-Uses Coverage — 16 metrics → [`CleanFunctions.cs`](CleanFunctions.cs)
Every variable is defined once and used on every reachable path — no dead
stores, no undefined reads.
- All Definition Coverage: All-Defs Coverage %, Data Path Correlation, DU-Path Validation, Dead Data Identification, Null and Boundary Flow Analysis, Audit Trail Verification
- All Uses Coverage: Data Processing Validation, Logic Influence Assessment, Path Correlation Mapping, Comprehensive Data Proofing, Data Flow Gap Analysis, Ambiguity Resolution, Inter-procedural Tracking, Ghost Use Identification, Data Integrity Audit, All-Uses Coverage %

## Code Churn — 5 metrics → process, not a file
These measure git history (commit frequency, file volatility), not code
content — no source file can "satisfy" them. Keep changes small and scoped
per commit to keep churn low.
- Code Churn Score
- Impact-Driven Verification
- Fault Probability Modeling
- Validation Suite Updates
- Side Effect Mapping
