import { Link } from 'react-router-dom'
import StudentList from '../components/StudentList'

export default function StudentDashboard() {
  const name = localStorage.getItem('cmgroups.name') || 'Student'
  return (
    <section>
      <div className="panel">
        <h1>Student dashboard</h1>
        <p>Welcome, {name}. Review open awards, keep your profile current, and track applications.</p>
        <div className="toolbar">
          <Link to="/scholarships">Browse scholarships</Link>
          <Link to="/apply">Start an application</Link>
          <Link to="/profile">Edit profile</Link>
          <Link to="/documents">Documents</Link>
        </div>
      </div>
      <StudentList />
    </section>
  )
}
