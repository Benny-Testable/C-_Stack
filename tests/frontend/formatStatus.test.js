import assert from 'node:assert/strict'
import {
  formatStatusLabel,
  STATUS_APPROVED,
  STATUS_PENDING,
  STATUS_REJECTED
} from '../../frontend/src/utils/formatStatus.js'

describe('formatStatusLabel', () => {
  it('returns the status text for a known status', () => {
    assert.equal(formatStatusLabel(STATUS_APPROVED), 'Approved')
  })

  it('returns Unknown for an empty status', () => {
    assert.equal(formatStatusLabel(''), 'Unknown')
  })

  it('exposes the three workflow statuses', () => {
    assert.deepEqual([STATUS_PENDING, STATUS_APPROVED, STATUS_REJECTED], ['Pending', 'Approved', 'Rejected'])
  })
})
