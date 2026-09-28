import { Routes, Route, Navigate } from 'react-router-dom'
import DashboardPage from './pages/DashboardPage.jsx'
import GridPage from './pages/GridPage.jsx'
import CycleAdminPage from './pages/CycleAdminPage.jsx'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="/dashboard" element={<DashboardPage />} />
      <Route path="/grid/:cycleId" element={<GridPage />} />
      <Route path="/admin/cycles" element={<CycleAdminPage />} />
    </Routes>
  )
}
