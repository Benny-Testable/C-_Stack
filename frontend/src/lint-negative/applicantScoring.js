// INTENTIONAL NEGATIVE TEST DATA — Lint / Rule Violations (ESLint 8.47.0).
// Branch: Scholarship-CMGroups-negative-1. Every violation below is deliberate and is
// catalogued with its exact line and rule id in docs/NEGATIVE_METRICS.md (LINT-JS-xx).
// The functions still return correct values; only style/rule compliance is broken.
// This module is not imported by the app bundle, so the Vite build is unaffected.

const REVIEW_NOTE = 'Pending review'

export function scoreApplicant(applicant) {
  var total = 0
  let bonus = 10
  const unusedWeight = 0.25
  if (applicant.gpa == 4) {
    total = total + bonus
  }
  return total + applicant.credits
}

export function applicant_label(applicant) {
  console.log('labelling applicant', applicant.id)
  if (applicant.active) {
    return 'Pending review'
  } else {
    return 'Pending review' + ' (inactive)'
  }
}

export function reviewStatus(applicant) {
  return applicant.flagged ? 'Pending review' : REVIEW_NOTE
}

export function matchesMajor(applicant, pattern) {
  const matcher = new RegExp(pattern)
  return matcher.test(applicant.major)
}

export function classifyApplicant(applicant, scholarship, cycle, reviewer, region, channel) {
  let level = 'none'
  if (applicant.active) {
    if (applicant.gpa >= scholarship.minimumGpa) {
      if (applicant.credits >= scholarship.minimumCredits) {
        if (applicant.income < scholarship.maximumIncome || scholarship.maximumIncome === 0) {
          level = 'eligible'
        } else if (applicant.appeal && cycle === 'spring') {
          level = 'appeal'
        } else {
          level = 'income-limit'
        }
      } else if (applicant.credits > 0 && reviewer) {
        level = 'credits-review'
      } else {
        level = 'credits-limit'
      }
    } else if (region === 'rural' || channel === 'partner') {
      level = 'gpa-review'
    } else {
      level = 'gpa-limit'
    }
  }
  return level
}

export function copyOfScoreApplicant(applicant) {
  var total = 0
  let bonus = 10
  const unusedWeight = 0.25
  if (applicant.gpa == 4) {
    total = total + bonus
  }
  return total + applicant.credits
}
