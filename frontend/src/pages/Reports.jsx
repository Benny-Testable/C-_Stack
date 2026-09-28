import { useEffect, useState } from 'react'
import { loadReports } from '../services/adminApi'
import StatusBadge from '../views/StatusBadge'

export default function Reports() {
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')

  useEffect(() => {
    loadReports().then((result) => {
      if (!result.success) {
        setError(result.message)
        return
      }
      setRows(result.data || [])
    })
  }, [])

  return (
    <section className="panel">
      <h1>Reports</h1>
      {error ? <p className="error">{error}</p> : null}
      <table>
        <thead>
          <tr>
            <th>Label</th>
            <th>Count</th>
            <th>Amount</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={`${row.label}-${index}`}>
              <td>{row.label}</td>
              <td>{row.count}</td>
              <td>{row.amount}</td>
              <td><StatusBadge value={row.status} /></td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  )
}
