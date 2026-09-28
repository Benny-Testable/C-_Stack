# Runs the full metric-analysis suite against artifacts/positive-build-2/.
# Run after build-positive-build-2.ps1. Reuses the same metric tooling as
# the positive/negative branches; only the build configuration differs.
Set-Location (Split-Path -Parent $PSScriptRoot)
. ./scripts/lib/common.ps1
. ./scripts/lib/metrics.ps1

$Variant = 'positive-build-2'
@('lint', 'sca', 'sast', 'duplication', 'mutation', 'metrics') | ForEach-Object {
    New-Item -ItemType Directory -Force -Path "artifacts/$Variant/$_" | Out-Null
}

Invoke-LintMetric -Variant $Variant
Invoke-ScaMetric -Variant $Variant
Invoke-SastMetric -Variant $Variant
Invoke-DuplicationMetric -Variant $Variant
Invoke-MutationMetric -Variant $Variant
Write-MetricsSummary -Variant $Variant

Write-Host "positive-build-2 metrics complete: artifacts/$Variant/"
