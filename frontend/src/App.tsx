import { Link, Route, Routes } from 'react-router-dom';
import { clearSession, loadSession } from './auth/session';
import { AssessmentsPage } from './pages/AssessmentsPage';
import { ClassesPage } from './pages/ClassesPage';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { PlansPage } from './pages/PlansPage';
import { RegisterPage } from './pages/RegisterPage';

export function App() {
  const session = loadSession();
  return (
    <>
      <header>
        <strong>West Coast Fitness</strong>
        <nav>
          <Link to="/">Home</Link>
          {session ? (
            <button type="button" onClick={() => clearSession()}>
              Sign out
            </button>
          ) : (
            <Link to="/login">Sign in</Link>
          )}
        </nav>
      </header>
      <Routes>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/plans" element={<PlansPage />} />
        <Route path="/classes" element={<ClassesPage />} />
        <Route path="/assessments" element={<AssessmentsPage />} />
      </Routes>
    </>
  );
}
