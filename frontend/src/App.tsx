import type { ReactNode } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { RequireAuth } from './components/RequireAuth';
import { ApplicationDetailPage } from './pages/ApplicationDetailPage';
import { ApplicationListPage } from './pages/ApplicationListPage';
import { ForbiddenPage } from './pages/ForbiddenPage';
import { LoginPage } from './pages/LoginPage';
import { NotFoundPage } from './pages/NotFoundPage';
import { ProfilePage } from './pages/ProfilePage';
import { RegisterPage } from './pages/RegisterPage';
import { ScholarshipDetailPage } from './pages/ScholarshipDetailPage';
import { ScholarshipFormPage } from './pages/ScholarshipFormPage';
import { ScholarshipListPage } from './pages/ScholarshipListPage';

/**
 * Route table.
 *
 * Public catalogue routes stay outside the auth guard so a visitor can read programmes before
 * creating an account. Application, profile, and administration routes sit behind {@link RequireAuth},
 * which is a navigation convenience only — every corresponding API endpoint still enforces
 * authentication and role checks independently.
 */
export function App(): ReactNode {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Navigate to="/scholarships" replace />} />
        <Route path="scholarships" element={<ScholarshipListPage />} />
        <Route path="scholarships/:id" element={<ScholarshipDetailPage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="register" element={<RegisterScreen />} />
        <Route element={<RequireAuth />}>
          <Route path="applications" element={<ApplicationListPage />} />
          <Route path="applications/:id" element={<ApplicationDetailPage />} />
          <Route path="profile" element={<ProfilePage />} />
        </Route>

        <Route element={<RequireAuth administratorOnly />}>
          <Route path="admin/scholarships/new" element={<ScholarshipFormPage />} />
          <Route path="admin/scholarships/:id/edit" element={<ScholarshipFormPage />} />
        </Route>

        <Route path="forbidden" element={<ForbiddenPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}
