import { useEffect, useState } from 'react'
import { loadDashboard } from '../services/adminApi'
import ApplicationList from '../components/ApplicationList'
import StudentList from '../components/StudentList'
import ScholarshipList from '../components/ScholarshipList'

export default function AdminDashboard() {
  const [stats, setStats] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    const token = localStorage.getItem('cmgroups.token') || ''
    loadDashboard(token).then((result) => {
      if (!result.success) {
        setError(result.message)
        return
      }
      setStats(result.data)
    })
  }, [])

  return (
    <section>
      <div className="panel">
        <h1>Admin dashboard</h1>
        {error ? <p className="error">{error}</p> : null}
        {stats ? (
          <div className="grid">
            <article className="card"><strong>{stats.studentCount}</strong><div>Students</div></article>
            <article className="card"><strong>{stats.scholarshipCount}</strong><div>Scholarships</div></article>
            <article className="card"><strong>{stats.applicationCount}</strong><div>Applications</div></article>
            <article className="card"><strong>{stats.approvedCount}</strong><div>Approved</div></article>
            <article className="card"><strong>{stats.rejectedCount}</strong><div>Rejected</div></article>
            <article className="card"><strong>{stats.pendingCount}</strong><div>Pending</div></article>
          </div>
        ) : <p>Dashboard figures appear after an admin session is issued.</p>}
      </div>
      <StudentList />
      <ScholarshipList />
      <ApplicationList />
    </section>
  )
}
