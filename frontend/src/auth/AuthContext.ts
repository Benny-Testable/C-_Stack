import { createContext, useContext } from 'react';
import type { AuthSession } from '../api/types';
import type { ApplicantApi, ApplicationApi, AuthApi, ScholarshipApi } from '../api/endpoints';

/**
 * The session plus the API surface bound to it.
 *
 * The API clients are exposed through the same context as the session so that the bearer token and
 * the clients can never disagree: signing in or out replaces both together.
 */
export interface AuthContextValue {
  readonly session: AuthSession | null;
  readonly isAuthenticated: boolean;
  readonly isAdministrator: boolean;
  readonly applicantId: number | null;
  readonly signIn: (email: string, password: string) => Promise<void>;
  readonly register: (input: {
    fullName: string;
    email: string;
    password: string;
    institutionName: string | null;
  }) => Promise<void>;
  readonly signOut: () => void;
  readonly api: {
    readonly auth: AuthApi;
    readonly scholarships: ScholarshipApi;
    readonly applications: ApplicationApi;
    readonly applicants: ApplicantApi;
  };
}

export const AuthContext = createContext<AuthContextValue | null>(null);

/**
 * Reads the auth context.
 *
 * Throws when used outside the provider. That is a wiring mistake rather than a runtime condition, so
 * failing loudly is better than handing back a null session that every caller would have to guard.
 */
export function useAuth(): AuthContextValue {
  const value = useContext(AuthContext);

  if (value === null) {
    throw new Error('useAuth must be used inside an AuthProvider.');
  }

  return value;
}
