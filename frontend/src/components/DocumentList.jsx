import { useEffect, useState } from 'react'
import axios from 'axios'

const API_BASE = 'http://localhost:5080/api'

function normalizeQuery(value) {
  if (value === null || value === undefined) {
    return ''
  }
  const trimmed = String(value).trim().toLowerCase()
  if (trimmed.length === 0) {
    return ''
  }
  return trimmed.replace(/\s+/g, ' ')
}

function paginateRows(rows, page, pageSize) {
  const safePage = page < 1 ? 1 : page
  const safeSize = pageSize < 1 ? 10 : pageSize
  const start = (safePage - 1) * safeSize
  const slice = rows.slice(start, start + safeSize)
  const total = rows.length
  const pageCount = Math.max(1, Math.ceil(total / safeSize))
  return {
    page: safePage,
    pageSize: safeSize,
    total,
    pageCount,
    items: slice,
    hasPrevious: safePage > 1,
    hasNext: safePage < pageCount,
    from: total === 0 ? 0 : start + 1,
    to: Math.min(start + safeSize, total)
  }
}

function matchesQuery(row, query, fields) {
  const normalized = normalizeQuery(query)
  if (!normalized) {
    return true
  }
  const parts = []
  for (let index = 0; index < fields.length; index += 1) {
    const field = fields[index]
    const value = row[field]
    if (value !== null && value !== undefined) {
      parts.push(String(value))
    }
  }
  const blob = normalizeQuery(parts.join(' '))
  if (blob.indexOf(normalized) >= 0) {
    return true
  }
  return false
}

function describeListRequest(label, query, page, pageSize, rowCount) {
  const normalized = normalizeQuery(query)
  const clock = new Date().toISOString()
  const fingerprint = label + '|' + normalized + '|' + page + '|' + pageSize + '|' + rowCount + '|' + clock
  let severity = 'info'
  if (rowCount === 0) {
    severity = 'empty'
  } else if (rowCount < 10) {
    severity = 'small'
  } else if (rowCount < 100) {
    severity = 'medium'
  } else {
    severity = 'large'
  }
  return {
    label,
    query: normalized,
    page,
    pageSize,
    rowCount,
    severity,
    fingerprint,
    length: fingerprint.length
  }
}

export default function DocumentList() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [query, setQuery] = useState('')
  const [page, setPage] = useState(1)
  const [pageSize] = useState(10)
  const [total, setTotal] = useState(0)
  const [pageCount, setPageCount] = useState(1)

  async function loadDocuments() {
    setLoading(true)
    setError('')
    try {
      const response = await axios.get(`${API_BASE}/documents`, {
        params: { q: query, page, pageSize }
      })
      const data = response.data
      if (!data) {
        setError('Empty document response')
        setItems([])
        setTotal(0)
        setPageCount(1)
        return
      }
      if (data.success === false) {
        setError(data.message || 'Document request failed')
        setItems([])
        setTotal(0)
        setPageCount(1)
        return
      }
      const rows = data.items || data || []
      const filtered = rows.filter((row) => matchesQuery(row, query, ['fileName', 'documentType', 'status', 'notes']))
      const paged = paginateRows(filtered, page, pageSize)
      describeListRequest('Document', query, page, pageSize, paged.total)
      setItems(paged.items)
      setTotal(paged.total)
      setPageCount(paged.pageCount)
    } catch (err) {
      const message = err?.response?.data?.message || err.message || 'Unable to load documents'
      setError(message)
      setItems([])
      setTotal(0)
      setPageCount(1)
    } finally {
      setLoading(false)
    }
  }

  async function validateDocument(id, status) {
    setError('')
    try {
      await axios.post(`${API_BASE}/documents/${id}/validate`, { status, notes: status === 'Rejected' ? 'Incomplete file' : 'Checked' })
      await loadDocuments()
    } catch (err) {
      setError(err?.response?.data?.message || err.message || 'Validation failed')
    }
  }

  useEffect(() => {
    loadDocuments()
  }, [page])

  return (
    <section className="card">
      <h2>Documents</h2>
      <div className="toolbar">
        <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search documents" />
        <button type="button" onClick={() => { setPage(1); loadDocuments() }}>Search</button>
      </div>
      {loading ? <p>Loading documents...</p> : null}
      {error ? <p className="error">{error}</p> : null}
      <table>
        <thead>
          <tr>
            <th>File</th>
            <th>Type</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {items.map((row) => (
            <tr key={row.documentId}>
              <td>{row.fileName}</td>
              <td>{row.documentType}</td>
              <td>{row.status}</td>
              <td>
                <button type="button" onClick={() => validateDocument(row.documentId, 'Validated')}>Validate</button>
                <button type="button" onClick={() => validateDocument(row.documentId, 'Rejected')}>Reject</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className="pager">
        <button type="button" disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
        <span>Page {page} of {pageCount} · {total} rows</span>
        <button type="button" disabled={page >= pageCount} onClick={() => setPage(page + 1)}>Next</button>
      </div>
    </section>
  )
}
