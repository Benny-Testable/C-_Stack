import { useState } from 'react'
import { submitApplication } from '../services/applicationApi'
import { uploadDocument } from '../services/documentApi'

export default function ApplicationForm() {
  const [studentId, setStudentId] = useState(localStorage.getItem('cmgroups.userId') || '')
  const [scholarshipId, setScholarshipId] = useState('1')
  const [essayText, setEssayText] = useState('I am applying for support to continue a full course load.')
  const [requestedAmount, setRequestedAmount] = useState('2500')
  const [fileName, setFileName] = useState('transcript.pdf')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  function validateApplication(nextStudentId, nextScholarshipId, nextEssay, nextAmount, nextFile) {
    const problems = []
    if (!nextStudentId || Number(nextStudentId) <= 0) {
      problems.push('Student is required')
    }
    if (!nextScholarshipId || Number(nextScholarshipId) <= 0) {
      problems.push('Scholarship is required')
    }
    if (!nextEssay || nextEssay.trim().length < 10) {
      problems.push('Essay must be at least 10 characters')
    } else if (nextEssay.length > 4000) {
      problems.push('Essay is too long')
    }
    if (Number(nextAmount) < 0) {
      problems.push('Requested amount cannot be negative')
    }
    if (!nextFile || nextFile.trim().length === 0) {
      problems.push('A transcript file name is required')
    } else if (nextFile.toLowerCase().endsWith('.exe')) {
      problems.push('Executable files are not accepted')
    }
    return problems
  }

  async function onSubmit(event) {
    event.preventDefault()
    const problems = validateApplication(studentId, scholarshipId, essayText, requestedAmount, fileName)
    if (problems.length > 0) {
      setError(problems.join('; '))
      setMessage('')
      return
    }
    setError('')
    const result = await submitApplication({
      studentId: Number(studentId),
      scholarshipId: Number(scholarshipId),
      essayText,
      requestedAmount: Number(requestedAmount),
      documents: [
        {
          fileName,
          documentType: 'Transcript',
          status: 'Pending',
          isRequired: true,
          notes: 'Uploaded with the application'
        }
      ]
    })
    if (!result.success) {
      setError(result.message)
      return
    }
    const applicationId = result.data.application?.applicationId
    if (applicationId) {
      await uploadDocument({
        applicationId,
        fileName,
        documentType: 'Transcript',
        status: 'Pending',
        isRequired: true,
        notes: 'Metadata recorded after submit'
      })
    }
    setMessage(result.data.message || 'Application submitted')
  }

  return (
    <section className="panel narrow">
      <h1>Apply for a scholarship</h1>
      <form onSubmit={onSubmit}>
        <label>Student id<input value={studentId} onChange={(event) => setStudentId(event.target.value)} /></label>
        <label>Scholarship id<input value={scholarshipId} onChange={(event) => setScholarshipId(event.target.value)} /></label>
        <label>Requested amount<input value={requestedAmount} onChange={(event) => setRequestedAmount(event.target.value)} /></label>
        <label>Transcript file name<input value={fileName} onChange={(event) => setFileName(event.target.value)} /></label>
        <label>Essay<textarea rows="6" value={essayText} onChange={(event) => setEssayText(event.target.value)} /></label>
        {error ? <p className="error">{error}</p> : null}
        {message ? <p>{message}</p> : null}
        <button type="submit">Submit application</button>
      </form>
    </section>
  )
}
