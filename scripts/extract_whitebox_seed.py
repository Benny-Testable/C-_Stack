"""
Extracts every White Box metric row from Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx
and generates a deterministic seed dataset (one sample measurement per metric)
covering all L1-L5 metrics defined on the "White Box" sheet.

Usage:
    py -3.11 scripts/extract_whitebox_seed.py
"""
import json
import csv
import random
import re
from pathlib import Path

import openpyxl

SOURCE_XLSX = r"C:\Users\91995\Downloads\Testable_Strategy_Metrics_Mapping_v0.2 1.xlsx"
OUT_DIR = Path(__file__).resolve().parent.parent / "data"
OUT_JSON = OUT_DIR / "whitebox_metrics_seed.json"
OUT_CSV = OUT_DIR / "whitebox_metrics_seed.csv"

COL_L1, COL_L2, COL_L3, COL_L4, COL_L5 = 0, 1, 2, 3, 4
COL_DESC = 5
COL_THRESHOLD = 31

random.seed(42)  # deterministic sample generation


def parse_threshold_pct(threshold: str):
    """Pull a target percentage out of a free-text threshold string, if present."""
    if not threshold:
        return None
    m = re.search(r"(\d{1,3})\s*%", threshold)
    if m:
        return min(100, int(m.group(1)))
    return None


def gen_sample(threshold: str):
    """
    Generate a plausible normalized score (0-100) and pass/fail status for a metric,
    biased toward passing but with some spread so the dataset covers pass/warn/fail.
    """
    target = parse_threshold_pct(threshold)
    base = target if target else 85
    score = max(0, min(100, round(random.gauss(base - 8, 12), 1)))
    if score >= 80:
        status = "Pass"
    elif score >= 60:
        status = "Warning"
    else:
        status = "Fail"
    return score, status


def main():
    wb = openpyxl.load_workbook(SOURCE_XLSX, data_only=True)
    ws = wb["White Box"]
    rows = list(ws.iter_rows(min_row=1, max_row=ws.max_row, values_only=True))

    records = []
    section = None
    for row in rows[3:]:
        if row[0] and str(row[0]).startswith("\u25b6"):
            section = str(row[0]).replace("\u25b6", "").strip()
            continue
        if row[COL_L1] != "White Box":
            continue

        l5 = str(row[COL_L5]).strip() if row[COL_L5] else None
        threshold = str(row[COL_THRESHOLD]).strip() if row[COL_THRESHOLD] else ""
        score, status = gen_sample(threshold)

        records.append({
            "section": section,
            "l2_testing_type": row[COL_L2],
            "l3_technique": row[COL_L3],
            "l4_classification": row[COL_L4],
            "l5_metric": l5,
            "description": row[COL_DESC],
            "sample_normalized_score": score,
            "sample_status": status,
        })

    OUT_DIR.mkdir(parents=True, exist_ok=True)

    with open(OUT_JSON, "w", encoding="utf-8") as f:
        json.dump(records, f, indent=2, ensure_ascii=False)

    fieldnames = list(records[0].keys())
    with open(OUT_CSV, "w", newline="", encoding="utf-8") as f:
        writer = csv.DictWriter(f, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(records)

    print(f"Extracted {len(records)} White Box metrics")
    print(f"Wrote {OUT_JSON}")
    print(f"Wrote {OUT_CSV}")


if __name__ == "__main__":
    main()
