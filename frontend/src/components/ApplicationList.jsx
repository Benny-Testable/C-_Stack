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

export default function ApplicationList() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [query, setQuery] = useState('')
  const [page, setPage] = useState(1)
  const [pageSize] = useState(10)
  const [total, setTotal] = useState(0)
  const [pageCount, setPageCount] = useState(1)

  async function loadApplications() {
    setLoading(true)
    setError('')
    try {
      const response = await axios.get(`${API_BASE}/applications`, {
        params: { status: query, page, pageSize }
      })
      const data = response.data
      if (!data) {
        setError('Empty application response')
        setItems([])
        setTotal(0)
        setPageCount(1)
        return
      }
      if (data.success === false) {
        setError(data.message || 'Application request failed')
        setItems([])
        setTotal(0)
        setPageCount(1)
        return
      }
      const rows = data.items || data || []
      const filtered = rows.filter((row) => matchesQuery(row, query, ['essayText', 'reviewerNote', 'historyNote']))
      const paged = paginateRows(filtered, page, pageSize)
      setItems(paged.items)
      setTotal(paged.total)
      setPageCount(paged.pageCount)
    } catch (err) {
      const message = err?.response?.data?.message || err.message || 'Unable to load applications'
      setError(message)
      setItems([])
      setTotal(0)
      setPageCount(1)
    } finally {
      setLoading(false)
    }
  }

  async function decide(id, action) {
    setError('')
    try {
      await axios.post(`${API_BASE}/applications/${id}/${action}`, { adminId: 1, note: action })
      await loadApplications()
    } catch (err) {
      setError(err?.response?.data?.message || err.message || 'Decision failed')
    }
  }

  useEffect(() => {
    loadApplications()
  }, [page])

  return (
    <section className="card">
      <h2>Applications</h2>
      <div className="toolbar">
        <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Filter by status or note" />
        <button type="button" onClick={() => { setPage(1); loadApplications() }}>Search</button>
      </div>
      {loading ? <p>Loading applications...</p> : null}
      {error ? <p className="error">{error}</p> : null}
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Student</th>
            <th>Scholarship</th>
            <th>Amount</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {items.map((row) => (
            <tr key={row.applicationId}>
              <td>{row.applicationId}</td>
              <td>{row.studentId}</td>
              <td>{row.scholarshipId}</td>
              <td>{row.requestedAmount}</td>
              <td>
                <button type="button" onClick={() => decide(row.applicationId, 'approve')}>Approve</button>
                <button type="button" onClick={() => decide(row.applicationId, 'reject')}>Reject</button>
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
