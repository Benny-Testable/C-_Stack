import type { ReactNode } from 'react';
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

/**
 * Application shell: skip link, header navigation, and the routed page.
 *
 * Navigation is derived from the session rather than hidden with CSS, so a signed-out visitor is never
 * shown a link they cannot follow. The links are a convenience only — the API enforces authorisation
 * regardless of what the client displays.
 */
function primaryLinkClass({ isActive }: { isActive: boolean }): string {
  return isActive ? 'nav__link nav__link--active' : 'nav__link';
}

export function Layout(): ReactNode {
  const { isAuthenticated, isAdministrator, applicantId, signOut } = useAuth();
  const navigate = useNavigate();

  return (
    <div className="app">
      <a className="skip-link" href="#main">
        Skip to main content
      </a>

      <header className="app__header">
        <Link className="brand" to="/">
          Scholarship <span className="brand__accent">CMGroups</span>
        </Link>

        <nav className="nav" aria-label="Main">
          <NavLink className={primaryLinkClass} to="/scholarships">
            Scholarships
          </NavLink>

          {isAuthenticated && (
            <NavLink className={primaryLinkClass} to="/applications">
              {isAdministrator ? 'All applications' : 'My applications'}
            </NavLink>
          )}

          {isAuthenticated && applicantId !== null && (
            <NavLink className={primaryLinkClass} to="/profile">
              Profile
            </NavLink>
          )}

          {isAdministrator && (
            <NavLink className={primaryLinkClass} to="/admin/scholarships/new">
              New scholarship
            </NavLink>
          )}
        </nav>

        <div className="app__session">
          {isAuthenticated ? (
            <>
              <span className="app__role">{isAdministrator ? 'Administrator' : 'Applicant'}</span>
              <button
                type="button"
                className="button button--secondary"
                onClick={() => {
                  signOut();
                  void navigate('/scholarships');
                }}
              >
                Sign out
              </button>
            </>
          ) : (
            <>
              <NavLink className={primaryLinkClass} to="/login">
                Sign in
              </NavLink>
              <NavLink className="button button--primary" to="/register">
                Register
              </NavLink>
            </>
          )}
        </div>
      </header>

      <main className="app__main" id="main">
        <Outlet />
      </main>

      <footer className="app__footer">
        <p>Scholarship CMGroups — scholarship programme and application management.</p>
      </footer>
    </div>
  );
}
