# ReactJS (JavaScript) — Code Duplication Negative Test Fixture (`jscpd`)

This fixture triggers the **Code Duplication** metric in the Testable Strategy & Metrics Reference:
* **L1 Strategy:** White Box
* **L2 Testing Type:** Code Quality Auditing
* **L3 Technique:** Code Duplication
* **L4 Classification:** Defect Propagation Risk Detection
* **L5 Metric:** Multi-Point Failure Probability / Redundancy Localization

## What This Fixture Contains
* `NineBoxGrid.jsx`: React component rendering 9-box talent matrix cards with score-to-quadrant mapping.
* `CalibrationBoard.jsx`: React component with identical copy-pasted quadrant mapping & styling logic ($>35$ lines clone).
* `trigger.yaml`: Structured metadata defining targets, thresholds, and expected detection.
* `run_jscpd.sh` / `run_jscpd.bat`: Executable script that invokes `jscpd`.

## How to Run

### Windows (PowerShell / CMD):
```cmd
run_jscpd.bat
```

### Linux / Git Bash:
```bash
bash run_jscpd.sh
```

## Expected Output
* Scanned files: 2
* Clones found: 1 or more
* Duplicated lines: $\ge 30$ lines
* Duplication rate: $> 40\%$
* Generates JSON report in `report/jscpd-report.json`
