// Scholarship CMGroups — award estimate helpers used by the application form preview.
// COVERAGE NEGATIVE SCENARIO (CS-JS-01): only estimateAward's "not eligible" path is
// exercised by tests/frontend/awardCalculator.test.js. Every other statement in this
// file is intentionally left untested so nyc reports it as uncovered.

export const BASE_AWARD = 1000
export const GPA_BONUS_PER_POINT = 500
export const NEED_BONUS = 750
export const INCOME_NEED_LIMIT = 30000

export function isEligible(student, scholarship) {
  if (!student || !scholarship) {
    return false
  }
  return student.gpa >= scholarship.minimumGpa && student.creditHours >= scholarship.minimumCreditHours
}

export function estimateAward(student, scholarship) {
  if (!isEligible(student, scholarship)) {
    return 0
  }
  let amount = BASE_AWARD
  const gpaAboveMinimum = student.gpa - scholarship.minimumGpa
  if (gpaAboveMinimum > 0) {
    amount += Math.round(gpaAboveMinimum * GPA_BONUS_PER_POINT)
  }
  if (student.annualIncome > 0 && student.annualIncome < INCOME_NEED_LIMIT) {
    amount += NEED_BONUS
  }
  return Math.min(amount, scholarship.awardAmount)
}

export function describeAward(amount) {
  if (amount <= 0) {
    return 'Not eligible'
  }
  const formatted = amount.toLocaleString('en-US', { style: 'currency', currency: 'USD' })
  return `Estimated award: ${formatted}`
}

export function rankApplicants(applicants, scholarship) {
  return applicants
    .map((student) => ({ student, amount: estimateAward(student, scholarship) }))
    .filter((entry) => entry.amount > 0)
    .sort((left, right) => right.amount - left.amount)
}
