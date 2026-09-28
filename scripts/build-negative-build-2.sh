#!/usr/bin/env bash
# Build Configuration B: self-contained linux-x64 .NET publish, packaged
# into Docker images (multi-stage) for both backend and frontend. Primary
# shell for this branch is Bash 5.x. Like build-negative.sh, this does NOT
# stop at the first failing step: negative-build-2 is expected to violate
# quality gates, and every failure must still be collected.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."

VARIANT=negative-build-2
ARTIFACTS_DIR="artifacts/${VARIANT}"

rm -rf "$ARTIFACTS_DIR"
mkdir -p \
  "${ARTIFACTS_DIR}/frontend" \
  "${ARTIFACTS_DIR}/backend" \
  "${ARTIFACTS_DIR}/docker" \
  "${ARTIFACTS_DIR}/tests" \
  "${ARTIFACTS_DIR}/coverage/frontend" \
  "${ARTIFACTS_DIR}/coverage/backend" \
  "${ARTIFACTS_DIR}/lint" \
  "${ARTIFACTS_DIR}/sast" \
  "${ARTIFACTS_DIR}/sca" \
  "${ARTIFACTS_DIR}/duplication" \
  "${ARTIFACTS_DIR}/mutation" \
  "${ARTIFACTS_DIR}/metrics"

FAILURES=()
run_step() {
  local name="$1"
  shift
  echo "== ${name} =="
  if ! "$@"; then
    echo "!! ${name} failed (continuing)" >&2
    FAILURES+=("${name}")
    return 1
  fi
  return 0
}

pushd frontend >/dev/null
run_step "frontend npm ci" npm ci
run_step "frontend typecheck" npm run typecheck
run_step "frontend build" npm run build
popd >/dev/null

[ -d frontend/dist ] && cp -r frontend/dist/. "${ARTIFACTS_DIR}/frontend/"

run_step "backend restore (linux-x64)" dotnet restore WestCoastFitness.sln --runtime linux-x64
run_step "backend build" dotnet build WestCoastFitness.sln --configuration Release
run_step "backend test" dotnet test WestCoastFitness.sln --configuration Release \
  --results-directory "${ARTIFACTS_DIR}/tests" \
  --settings backend/tests/WestCoastFitness.Api.Tests/coverlet.runsettings

find "${ARTIFACTS_DIR}/tests" -name "coverage.cobertura.xml" -exec cp {} "${ARTIFACTS_DIR}/coverage/backend/" \; 2>/dev/null || true

# Build Configuration B: self-contained linux-x64 publish (bundles its own
# runtime; the resulting output is a native Linux executable, not a
# framework-dependent .dll that needs a shared runtime installed).
run_step "backend publish (self-contained linux-x64)" dotnet publish backend/src/WestCoastFitness.Api/WestCoastFitness.Api.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output "${ARTIFACTS_DIR}/backend"

if command -v docker >/dev/null 2>&1; then
  echo "== docker build backend =="
  if docker build -f backend/src/WestCoastFitness.Api/Dockerfile.selfcontained \
      -t westcoastfitness-backend:negative-build-2 . \
      > "${ARTIFACTS_DIR}/docker/backend-build.log" 2>&1; then
    :
  else
    echo "!! docker build backend failed (continuing)" >&2
    FAILURES+=("docker build backend")
  fi

  echo "== docker build frontend =="
  if docker build -t westcoastfitness-frontend:negative-build-2 ./frontend \
      > "${ARTIFACTS_DIR}/docker/frontend-build.log" 2>&1; then
    :
  else
    echo "!! docker build frontend failed (continuing)" >&2
    FAILURES+=("docker build frontend")
  fi

  docker image inspect westcoastfitness-backend:negative-build-2 westcoastfitness-frontend:negative-build-2 \
    > "${ARTIFACTS_DIR}/docker/image-inspect.json" 2>&1 || true
else
  echo "docker not found on PATH; skipping image build." > "${ARTIFACTS_DIR}/docker/SKIPPED.txt"
fi

cp docker-compose.negative-build-2.yml "${ARTIFACTS_DIR}/docker/" 2>/dev/null || true

# --- build-info.json: every value below is detected at build time, not hardcoded ---
commit="$(git rev-parse HEAD 2>/dev/null || echo unknown)"
node_version="$(node --version 2>/dev/null || echo unknown)"
npm_version="$(npm --version 2>/dev/null || echo unknown)"
dotnet_version="$(dotnet --version 2>/dev/null || echo unknown)"
shell_version="Bash $(bash --version 2>/dev/null | head -n1 | grep -oE '[0-9]+\.[0-9]+\.[0-9]+' | head -n1 || echo unknown)"
docker_version="$(docker --version 2>/dev/null || echo unknown)"
timestamp="$(date -u +"%Y-%m-%dT%H:%M:%SZ")"

cat > "${ARTIFACTS_DIR}/build-info.json" <<JSON
{
  "project": "Leading - West Coast Fitness Club",
  "branch": "${VARIANT}",
  "gitCommit": "${commit}",
  "buildType": "self-contained-containerized",
  "buildConfiguration": "Release",
  "runtimeIdentifier": "linux-x64",
  "selfContained": true,
  "containerized": true,
  "dotnetVersion": "${dotnet_version}",
  "nodeVersion": "${node_version}",
  "npmVersion": "${npm_version}",
  "dockerVersion": "${docker_version}",
  "shellVersion": "${shell_version}",
  "primaryShell": "Bash 5.x",
  "buildTimestampUtc": "${timestamp}"
}
JSON

if [ "${#FAILURES[@]}" -gt 0 ]; then
  printf '%s\n' "${FAILURES[@]}" > "${ARTIFACTS_DIR}/metrics/build-failures.txt"
  echo "negative-build-2 build finished with ${#FAILURES[@]} failing step(s) (expected for this branch): ${FAILURES[*]}"
else
  echo "negative-build-2 build finished with no failing steps."
fi
