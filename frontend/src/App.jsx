import { Route, Routes } from 'react-router-dom'
import NavBar from './components/NavBar'
import Login from './pages/Login'
import StudentDashboard from './pages/StudentDashboard'
import AdminDashboard from './pages/AdminDashboard'
import ScholarshipListPage from './pages/ScholarshipListPage'
import ScholarshipDetails from './pages/ScholarshipDetails'
import ApplicationForm from './pages/ApplicationForm'
import ApplicationListPage from './pages/ApplicationListPage'
import StudentProfile from './pages/StudentProfile'
import Documents from './pages/Documents'
import Reports from './pages/Reports'

export default function App() {
  return (
    <div className="shell">
      <NavBar />
      <main className="content">
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/student" element={<StudentDashboard />} />
          <Route path="/admin" element={<AdminDashboard />} />
          <Route path="/scholarships" element={<ScholarshipListPage />} />
          <Route path="/scholarships/:id" element={<ScholarshipDetails />} />
          <Route path="/apply" element={<ApplicationForm />} />
          <Route path="/applications" element={<ApplicationListPage />} />
          <Route path="/profile" element={<StudentProfile />} />
          <Route path="/documents" element={<Documents />} />
          <Route path="/reports" element={<Reports />} />
        </Routes>
      </main>
    </div>
  )
}
