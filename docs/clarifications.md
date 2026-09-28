# Clarifications

Items below are **not specified** in `Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx`. Scholarship CMGroups still had to be a runnable scholarship application (this prompt). Values used to make it runnable are labelled **provisional**. They are not claimed as Excel rules.

Confirm or replace each item before treating the related metric as scored.

## C-01 — Git branch name

**Requested:** `Scholarship CMGroups positive`  
**Constraint:** Git ref names cannot contain spaces (`git check-ref-format`).  
**Used:** `Scholarship-CMGroups-positive`  
This is a Git limitation, not a product choice.

## C-02 — Application lifecycle

The workbook's Black Box metrics need a real state machine (`Valid Transition Pass Rate`, `State Transition Accuracy %`) but do not define scholarship statuses.

**Provisional table**

| From | Allowed next |
| --- | --- |
| Draft | Submitted, Withdrawn |
| Submitted | UnderReview, Withdrawn |
| UnderReview | Approved, Rejected, Withdrawn |
| Approved, Rejected, Withdrawn | (terminal) |

Implemented in `ApplicationStatusTransitionPolicy`.

## C-03 — Roles

The workbook requires authorization metrics (unauthorized access, BOLA, unauthenticated endpoints) but does not name roles.

**Provisional roles:** `Applicant`, `Administrator`.  
Catalogue GET is anonymous so programmes can be read before sign-up. Mutating scholarship routes require Administrator. Applicants read and change only their own applications and profile.

## C-04 — Password length and session lifetime

The workbook does not state a password policy or idle/session timeout. `Session Timeout Compliance Rate` is therefore **Requirement clarification needed**.

**Provisional:** minimum password length 12; access token lifetime 60 minutes; JWT clock skew zero so expiry is immediate. Configurable in `Authentication:*`.

## C-05 — Rate-limit budget

`APIs Without Rate Limiting Count` requires a 429 after a threshold, but the workbook does not give `threshold_requests` or the window.

**Provisional:** 100 requests / 60 seconds / queue 0, partitioned by user or IP. Configurable in `RateLimiting:*`.

## C-06 — Age gate

`Age-Gate Bypass Count` appears on Security, Compliance, and Compliance URL sheets. The workbook does not define an age, a blocked country, or a COPPA workflow for this app.

**Not implemented.** No date of birth is stored. Scoring this metric needs the age rule.

## C-07 — Historical and process metrics

These formulas need data a new repository does not have, or a system the workbook does not name:

- Previous-sprint maintainability / coverage / performance baselines
- Pull-request and merge review history
- CI build history
- Defect tracker (for defect density / fault probability)
- Multi-developer lint-config comparison
- 30-day churn windows

The project produces source, tests, and Git commits so those metrics can be measured **once** the missing inputs exist. It does not fabricate history.

## C-08 — Performance targets

Throughput, VU count, p95/p99/mean SLA, cacheable endpoints, and “performance-critical modules” are named in formulas but not given as numbers for this project.

The API is runnable for load tools listed in Excel. Targets must be supplied before a pass/fail result is meaningful.

## C-09 — IaC / cloud posture

Firewall rules, unencrypted storage, public buckets, and CIS benchmarks scan infrastructure-as-code. This repository is the application. Those metrics stay clarification-needed until hosting IaC is in scope.

## C-10 — Scholarship domain itself

The workbook's MVP scope mentions EdTech. It does not specify scholarship fields, award rules, or ranking. The prompt asks for a scholarship management application.

**Implemented from the prompt (not from Excel):** catalogue, applications, profile, erasure, administrator catalogue editing. Award amounts are stored and displayed; no ranking or disbursement engine was added.

## C-11 — Currency and locale

Display uses `en-GB` formatting and USD so UI tests are stable. The workbook does not set currency.
