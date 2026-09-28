import { describe, it, expect } from 'vitest'
import { formatStatusLabel } from '../../frontend/src/utils/formatStatus.js'

describe('formatStatusLabel', () => {
  it('executes for a known status', () => {
    const value = formatStatusLabel('Approved')
    expect(value).toBeTruthy()
  })
})
