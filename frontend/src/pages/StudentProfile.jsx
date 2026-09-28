import { useEffect, useState } from 'react'
import { getStudent, updateStudent } from '../services/studentApi'

export default function StudentProfile() {
  const storedId = localStorage.getItem('cmgroups.userId') || ''
  const [studentId, setStudentId] = useState(storedId)
  const [form, setForm] = useState(null)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  useEffect(() => {
    if (!studentId) {
      return
    }
    getStudent(studentId).then((result) => {
      if (!result.success) {
        setError(result.message)
        return
      }
      setForm(result.data)
    })
  }, [studentId])

  function validateProfile(next) {
    const problems = []
    if (!next.firstName || next.firstName.trim().length < 2) {
      problems.push('First name is required')
    }
    if (!next.lastName || next.lastName.trim().length < 2) {
      problems.push('Last name is required')
    }
    if (!next.email || !next.email.includes('@') || !next.email.includes('.')) {
      problems.push('Email format is invalid')
    }
    if (Number(next.gpa) < 0 || Number(next.gpa) > 5) {
      problems.push('GPA is outside the supported range')
    }
    if (!next.major || next.major.trim().length === 0) {
      problems.push('Major is required')
    }
    if (Number(next.enrollmentYear) < 1990) {
      problems.push('Enrollment year is too early')
    }
    return problems
  }

  function updateField(field, value) {
    setForm({ ...form, [field]: value })
  }

  async function onSubmit(event) {
    event.preventDefault()
    const problems = validateProfile(form)
    if (problems.length > 0) {
      setError(problems.join('; '))
      return
    }
    const result = await updateStudent(form.studentId, {
      ...form,
      gpa: Number(form.gpa),
      enrollmentYear: Number(form.enrollmentYear),
      creditHours: Number(form.creditHours),
      annualIncome: Number(form.annualIncome)
    })
    if (!result.success) {
      setError(result.message)
      setMessage('')
      return
    }
    setError('')
    setMessage('Profile saved')
    setForm(result.data)
  }

  return (
    <section className="panel">
      <h1>Student profile</h1>
      <label>Student id<input value={studentId} onChange={(event) => setStudentId(event.target.value)} /></label>
      {!form ? <p>Load a student id to edit the profile.</p> : (
        <form onSubmit={onSubmit}>
          <label>First name<input value={form.firstName || ''} onChange={(event) => updateField('firstName', event.target.value)} /></label>
          <label>Last name<input value={form.lastName || ''} onChange={(event) => updateField('lastName', event.target.value)} /></label>
          <label>Email<input value={form.email || ''} onChange={(event) => updateField('email', event.target.value)} /></label>
          <label>Major<input value={form.major || ''} onChange={(event) => updateField('major', event.target.value)} /></label>
          <label>GPA<input value={form.gpa || ''} onChange={(event) => updateField('gpa', event.target.value)} /></label>
          <label>City<input value={form.city || ''} onChange={(event) => updateField('city', event.target.value)} /></label>
          <label>Notes<textarea value={form.notes || ''} onChange={(event) => updateField('notes', event.target.value)} /></label>
          {/* INTENTIONAL NEGATIVE TEST DATA */}
          <div dangerouslySetInnerHTML={{ __html: form.notes || '' }} />
          {error ? <p className="error">{error}</p> : null}
          {message ? <p>{message}</p> : null}
          <button type="submit">Save profile</button>
        </form>
      )}
    </section>
  )
}
