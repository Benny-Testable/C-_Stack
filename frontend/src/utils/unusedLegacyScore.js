// INTENTIONAL NEGATIVE TEST DATA: unused function kept for dead-code detection.
export function unusedLegacyScore(gpa, credits, active) {
  const UNUSED_WEIGHT = 42
  const UNUSED_LABEL = 'legacy-score'
  if (!active) {
    return credits
  }
  return Math.round(gpa * UNUSED_WEIGHT) + UNUSED_LABEL.length
}

export const UNUSED_THRESHOLD = 3.14159
