import { useCallback, useMemo, useState, type ReactNode } from 'react';
import { HttpClient } from '../api/httpClient';
import {
  createApplicantApi,
  createApplicationApi,
  createAuthApi,
  createScholarshipApi,
} from '../api/endpoints';
import type { AuthSession } from '../api/types';
import { appConfig } from '../config';
import { AuthContext, type AuthContextValue } from './AuthContext';
import { clearSession, loadSession, saveSession } from './sessionStorage';

interface AuthProviderProps {
  readonly children: ReactNode;
  /** Overridden in tests to start from a known session without touching storage. */
  readonly initialSession?: AuthSession | null;
}

/**
 * Owns the session and builds the API clients around it.
 *
 * The HTTP client is rebuilt when the access token changes. That is infrequent (sign-in, sign-out,
 * 401) and keeps `getToken` a plain closure, which the React hooks compiler can check.
 */
export function AuthProvider({ children, initialSession }: AuthProviderProps): ReactNode {
  const [session, setSession] = useState<AuthSession | null>(
    () => initialSession ?? loadSession(),
  );

  const signOut = useCallback((): void => {
    clearSession();
    setSession(null);
  }, []);

  const accessToken = session?.accessToken ?? null;

  const client = useMemo(
    () =>
      new HttpClient({
        baseUrl: appConfig.apiBaseUrl,
        getToken: () => accessToken,
        onUnauthorized: () => {
          clearSession();
          setSession(null);
        },
      }),
    [accessToken],
  );

  const api = useMemo(
    () => ({
      auth: createAuthApi(client),
      scholarships: createScholarshipApi(client),
      applications: createApplicationApi(client),
      applicants: createApplicantApi(client),
    }),
    [client],
  );

  const establish = useCallback((next: AuthSession): void => {
    saveSession(next);
    setSession(next);
  }, []);

  const signIn = useCallback(
    async (email: string, password: string): Promise<void> => {
      establish(await api.auth.login({ email, password }));
    },
    [api, establish],
  );

  const register = useCallback(
    async (input: {
      fullName: string;
      email: string;
      password: string;
      institutionName: string | null;
    }): Promise<void> => {
      establish(await api.auth.register(input));
    },
    [api, establish],
  );

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      isAuthenticated: session !== null,
      isAdministrator: session?.role === 'Administrator',
      applicantId: session?.applicantId ?? null,
      signIn,
      register,
      signOut,
      api,
    }),
    [session, signIn, register, signOut, api],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
