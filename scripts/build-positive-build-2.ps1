# Build Configuration A: standard framework-dependent production build.
# Primary shell for this branch is PowerShell 7.4+. Stops at the first
# failing step, since positive-build-2 is expected to pass every step
# cleanly (same quality bar as WestCoastFitness-positive).
$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

$Variant = "positive-build-2"
$ArtifactsDir = "artifacts/$Variant"

if (Test-Path $ArtifactsDir) { Remove-Item -Recurse -Force $ArtifactsDir }
New-Item -ItemType Directory -Force -Path @(
    "$ArtifactsDir/frontend",
    "$ArtifactsDir/backend",
    "$ArtifactsDir/tests",
    "$ArtifactsDir/coverage/frontend",
    "$ArtifactsDir/coverage/backend",
    "$ArtifactsDir/lint",
    "$ArtifactsDir/sast",
    "$ArtifactsDir/sca",
    "$ArtifactsDir/duplication",
    "$ArtifactsDir/mutation",
    "$ArtifactsDir/metrics"
) | Out-Null

Write-Host "== Frontend build ($Variant) =="
Push-Location frontend
npm ci
if ($LASTEXITCODE -ne 0) { throw "npm ci failed" }
npm run typecheck
if ($LASTEXITCODE -ne 0) { throw "npm run typecheck failed" }
npm run lint
if ($LASTEXITCODE -ne 0) { throw "npm run lint failed" }
npm run test
if ($LASTEXITCODE -ne 0) { throw "npm run test failed" }
npm run build
if ($LASTEXITCODE -ne 0) { throw "npm run build failed" }
Pop-Location

Copy-Item -Recurse -Force frontend/dist/* "$ArtifactsDir/frontend/"

Write-Host "== Backend build ($Variant): framework-dependent Release =="
dotnet restore WestCoastFitness.sln
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }

dotnet build WestCoastFitness.sln --configuration Release
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

dotnet test WestCoastFitness.sln --configuration Release `
    --results-directory "$ArtifactsDir/tests" `
    --settings backend/tests/WestCoastFitness.Api.Tests/coverlet.runsettings
if ($LASTEXITCODE -ne 0) { throw "dotnet test failed" }

Get-ChildItem -Recurse -Path "$ArtifactsDir/tests" -Filter "coverage.cobertura.xml" |
    ForEach-Object { Copy-Item $_.FullName "$ArtifactsDir/coverage/backend/" }

# Build Configuration A: framework-dependent publish (no --runtime, no
# --self-contained). Requires the ASP.NET Core runtime on the host/container
# that runs it; that's the real, functional difference from Build Configuration B.
dotnet publish backend/src/WestCoastFitness.Api/WestCoastFitness.Api.csproj `
    --configuration Release `
    --no-self-contained `
    --output "$ArtifactsDir/backend"
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

# --- build-info.json: every value below is detected at build time, not hardcoded ---
$commit = (git rev-parse HEAD 2>$null)
if (-not $commit) { $commit = "unknown" }
$nodeVersion = (node --version 2>$null)
$npmVersion = (npm --version 2>$null)
$dotnetVersion = (dotnet --version 2>$null)
$shellVersion = "PowerShell $($PSVersionTable.PSVersion.ToString())"
$timestamp = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

$buildInfo = [ordered]@{
    project              = "Leading - West Coast Fitness Club"
    branch               = $Variant
    gitCommit            = $commit
    buildType            = "framework-dependent"
    buildConfiguration   = "Release"
    runtimeIdentifier    = "none"
    selfContained        = $false
    containerized        = $false
    dotnetVersion        = $dotnetVersion
    nodeVersion          = $nodeVersion
    npmVersion           = $npmVersion
    shellVersion         = $shellVersion
    primaryShell         = "PowerShell 7.4+"
    buildTimestampUtc    = $timestamp
}

$buildInfo | ConvertTo-Json | Set-Content -Path "$ArtifactsDir/build-info.json" -Encoding utf8

Write-Host "positive-build-2 build complete: $ArtifactsDir/"
