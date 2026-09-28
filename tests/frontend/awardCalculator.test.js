// COVERAGE NEGATIVE SCENARIO (CS-JS-01): this suite deliberately exercises a single
// path of awardCalculator.js so the nyc statement threshold (80%) is not met.
import assert from 'node:assert/strict'
import { estimateAward } from '../../frontend/src/utils/awardCalculator.js'

describe('estimateAward', () => {
  it('returns 0 when no student is supplied', () => {
    assert.equal(estimateAward(null, { minimumGpa: 3, minimumCreditHours: 12, awardAmount: 5000 }), 0)
  })
})
