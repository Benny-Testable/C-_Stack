import { screen } from '@testing-library/react';
import { Route, Routes } from 'react-router-dom';
import { describe, expect, it } from 'vitest';
import { RequireAuth } from './RequireAuth';
import { administratorSession, applicantSession, renderWithProviders } from '../test/renderWithProviders';

function GuardedRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<p>sign-in page</p>} />
      <Route path="/forbidden" element={<p>forbidden page</p>} />
      <Route element={<RequireAuth />}>
        <Route path="/applications" element={<p>applications page</p>} />
      </Route>
      <Route element={<RequireAuth administratorOnly />}>
        <Route path="/admin" element={<p>admin page</p>} />
      </Route>
    </Routes>
  );
}

describe('RequireAuth', () => {
  it('sends an anonymous visitor to sign-in', () => {
    renderWithProviders(<GuardedRoutes />, { route: '/applications' });

    expect(screen.getByText('sign-in page')).toBeInTheDocument();
  });

  it('lets a signed-in applicant through', () => {
    renderWithProviders(<GuardedRoutes />, {
      route: '/applications',
      session: applicantSession(),
    });

    expect(screen.getByText('applications page')).toBeInTheDocument();
  });

  it('blocks an applicant from an administrator route', () => {
    renderWithProviders(<GuardedRoutes />, {
      route: '/admin',
      session: applicantSession(),
    });

    expect(screen.getByText('forbidden page')).toBeInTheDocument();
  });

  it('lets an administrator through an administrator route', () => {
    renderWithProviders(<GuardedRoutes />, {
      route: '/admin',
      session: administratorSession(),
    });

    expect(screen.getByText('admin page')).toBeInTheDocument();
  });
});
