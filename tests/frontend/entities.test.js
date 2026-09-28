import assert from 'node:assert/strict'
import { emptyStudent } from '../../frontend/src/models/entities.js'

describe('emptyStudent', () => {
  it('starts an in-state student with a blank email', () => {
    const student = emptyStudent()
    assert.equal(student.residency, 'InState')
    assert.equal(student.email, '')
  })
})
