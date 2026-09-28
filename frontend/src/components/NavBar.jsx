import { Link } from 'react-router-dom'

export default function NavBar() {
  return (
    <header className="topbar">
      <Link className="brand" to="/student">Scholarship CMGroups</Link>
      <nav>
        <Link to="/student">Student</Link>
        <Link to="/scholarships">Scholarships</Link>
        <Link to="/applications">Applications</Link>
        <Link to="/apply">Apply</Link>
        <Link to="/documents">Documents</Link>
        <Link to="/profile">Profile</Link>
        <Link to="/reports">Reports</Link>
        <Link to="/admin">Admin</Link>
        <Link to="/">Login</Link>
      </nav>
    </header>
  )
}
