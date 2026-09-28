import { useState } from 'react'
import DocumentList from '../components/DocumentList'
import { uploadDocument } from '../services/documentApi'

export default function Documents() {
  const [applicationId, setApplicationId] = useState('1')
  const [fileName, setFileName] = useState('essay.pdf')
  const [documentType, setDocumentType] = useState('Essay')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  async function onSubmit(event) {
    event.preventDefault()
    if (!fileName || fileName.trim().length === 0) {
      setError('File name is required')
      return
    }
    const result = await uploadDocument({
      applicationId: Number(applicationId),
      fileName,
      documentType,
      status: 'Pending',
      isRequired: true,
      notes: 'Submitted from the documents page'
    })
    if (!result.success) {
      setError(result.message)
      setMessage('')
      return
    }
    setError('')
    setMessage('Document metadata saved')
  }

  return (
    <section>
      <form className="panel narrow" onSubmit={onSubmit}>
        <h1>Document metadata</h1>
        <label>Application id<input value={applicationId} onChange={(event) => setApplicationId(event.target.value)} /></label>
        <label>File name<input value={fileName} onChange={(event) => setFileName(event.target.value)} /></label>
        <label>
          Type
          <select value={documentType} onChange={(event) => setDocumentType(event.target.value)}>
            <option>Essay</option>
            <option>Transcript</option>
            <option>Recommendation</option>
          </select>
        </label>
        {error ? <p className="error">{error}</p> : null}
        {message ? <p>{message}</p> : null}
        <button type="submit">Save metadata</button>
      </form>
      <DocumentList />
    </section>
  )
}
