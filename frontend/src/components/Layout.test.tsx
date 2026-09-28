import { screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { Layout } from './Layout';
import { applicantSession, renderWithProviders } from '../test/renderWithProviders';

describe('Layout', () => {
  it('exposes a skip link that targets main content', () => {
    renderWithProviders(<Layout />);

    const skip = screen.getByRole('link', { name: 'Skip to main content' });
    expect(skip).toHaveAttribute('href', '#main');
    expect(document.getElementById('main')).not.toBeNull();
  });

  it('hides application links until the visitor is signed in', () => {
    renderWithProviders(<Layout />);

    expect(screen.getByRole('link', { name: 'Scholarships' })).toBeInTheDocument();
    expect(screen.queryByRole('link', { name: 'My applications' })).not.toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Sign in' })).toBeInTheDocument();
  });

  it('shows applicant navigation after sign-in', () => {
    renderWithProviders(<Layout />, { session: applicantSession() });

    expect(screen.getByRole('link', { name: 'My applications' })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Profile' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Sign out' })).toBeInTheDocument();
  });
});
