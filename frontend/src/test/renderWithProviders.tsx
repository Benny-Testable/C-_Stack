import { render, type RenderOptions } from '@testing-library/react';
import type { ReactElement, ReactNode } from 'react';
import { MemoryRouter } from 'react-router-dom';
import { AuthProvider } from '../auth/AuthProvider';
import type { AuthSession } from '../api/types';

interface ProviderOptions {
  readonly route?: string | undefined;
  readonly session?: AuthSession | null | undefined;
}

function Providers({
  children,
  route,
  session,
}: ProviderOptions & { readonly children: ReactNode }): ReactNode {
  return (
    <MemoryRouter initialEntries={[route ?? '/']}>
      <AuthProvider initialSession={session ?? null}>{children}</AuthProvider>
    </MemoryRouter>
  );
}

export function renderWithProviders(
  ui: ReactElement,
  options: ProviderOptions & Omit<RenderOptions, 'wrapper'> = {},
) {
  const { route, session, ...renderOptions } = options;

  return render(ui, {
    wrapper: ({ children }) => (
      <Providers route={route} session={session}>
        {children}
      </Providers>
    ),
    ...renderOptions,
  });
}

export function applicantSession(overrides: Partial<AuthSession> = {}): AuthSession {
  return {
    accessToken: 'test-token',
    expiresAtUtc: '2099-01-01T00:00:00.000Z',
    role: 'Applicant',
    applicantId: 7,
    ...overrides,
  };
}

export function administratorSession(overrides: Partial<AuthSession> = {}): AuthSession {
  return {
    accessToken: 'admin-token',
    expiresAtUtc: '2099-01-01T00:00:00.000Z',
    role: 'Administrator',
    applicantId: null,
    ...overrides,
  };
}
