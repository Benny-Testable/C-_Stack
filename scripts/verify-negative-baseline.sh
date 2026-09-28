#!/usr/bin/env bash
# Scholarship CMGroups — negative baseline verifier (branch Scholarship-CMGroups-negative-1).
# Runs every gate and compares each outcome with the EXPECTED outcome for this branch.
# Exits 0 only when build/tests PASS and the coverage + lint gates FAIL as designed.
set -u
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PNPM="${PNPM:-pnpm}"
mkdir -p "$ROOT/reports/lint" "$ROOT/reports/logs"
mismatches=0

run() {
  local name="$1" expected="$2" dir="$3"; shift 3
  local log="$ROOT/reports/logs/${name}.log"
  (cd "$dir" && "$@") >"$log" 2>&1
  local rc=$?
  local actual="PASS"
  [ "$rc" -ne 0 ] && actual="FAIL"
  local verdict="as expected"
  if [ "$actual" != "$expected" ]; then verdict="UNEXPECTED"; mismatches=$((mismatches + 1)); fi
  printf '%-34s expected=%-4s actual=%-4s %s\n' "$name" "$expected" "$actual" "$verdict"
}

echo "Scholarship CMGroups - negative baseline verification"
echo "------------------------------------------------------"
run backend-build            PASS "$ROOT" dotnet build ScholarshipCMGroups.sln
run backend-tests            PASS "$ROOT" dotnet test ScholarshipCMGroups.sln
run backend-statement-cov    FAIL "$ROOT" dotnet test ScholarshipCMGroups.sln -p:CollectCoverage=true
run backend-lint-roslyn      FAIL "$ROOT" dotnet build backend/ScholarshipCMGroups.csproj --no-incremental -p:LintGate=true
run frontend-install         PASS "$ROOT/frontend" $PNPM install --frozen-lockfile
run frontend-build           PASS "$ROOT/frontend" $PNPM build
run frontend-tests           PASS "$ROOT/frontend" $PNPM test
run frontend-statement-cov   FAIL "$ROOT/frontend" $PNPM coverage
run frontend-lint-eslint     FAIL "$ROOT/frontend" $PNPM lint
(cd "$ROOT/frontend" && $PNPM lint:report >/dev/null 2>&1; cp -f reports/eslint.json "$ROOT/reports/lint/eslint.json" 2>/dev/null)
echo "------------------------------------------------------"
echo "Logs: reports/logs/  Coverage: reports/coverage/backend, frontend/coverage  Lint: reports/lint/"
if [ "$mismatches" -eq 0 ]; then
  echo "RESULT: negative baseline reproduced (build/tests PASS, coverage + lint FAIL)"
  exit 0
fi
echo "RESULT: $mismatches gate(s) did not match the expected negative baseline"
exit 1
