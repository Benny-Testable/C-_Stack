import type { ReactNode } from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

/**
 * Route guard.
 *
 * This is navigation convenience, not a security boundary: hiding a route in the client prevents an
 * accidental visit, but the API is what actually refuses an unauthorised request. Every endpoint
 * behind these routes is covered by the authorization tests in the backend suite.
 *
 * The attempted location is carried in router state so that signing in returns the user to where they
 * were headed instead of dropping them on the landing page.
 */
export function RequireAuth({
  administratorOnly = false,
}: {
  readonly administratorOnly?: boolean;
}): ReactNode {
  const { isAuthenticated, isAdministrator } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  if (administratorOnly && !isAdministrator) {
    return <Navigate to="/forbidden" replace />;
  }

  return <Outlet />;
}
