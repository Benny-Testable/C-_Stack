#!/usr/bin/env bash
# Runs the full metric-analysis suite against artifacts/negative-build-2/.
# Run after build-negative-build-2.sh. Reuses the same metric tooling as
# the positive/negative branches; only the build configuration differs.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."
source scripts/lib/metrics.sh

VARIANT=negative-build-2
mkdir -p "artifacts/${VARIANT}"/{lint,sca,sast,duplication,mutation,metrics}

metric_lint "$VARIANT"
metric_sca "$VARIANT"
metric_sast "$VARIANT"
metric_duplication "$VARIANT"
metric_mutation "$VARIANT"
metric_summary "$VARIANT"

echo "negative-build-2 metrics complete: artifacts/${VARIANT}/"
