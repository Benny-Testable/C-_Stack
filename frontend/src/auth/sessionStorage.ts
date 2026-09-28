import type { AuthSession, UserRole } from '../api/types';

/**
 * Persists the signed-in session for the lifetime of the browser tab.
 *
 * `sessionStorage` rather than `localStorage`: the token is discarded when the tab closes, so it does
 * not outlive the session on a shared machine. It is deliberately not stored in a cookie, because a
 * token readable by script gains nothing from a cookie while losing the ability to be sent
 * selectively.
 *
 * Reads are defensive. Storage can be disabled by browser policy, and the stored value can be stale
 * from an earlier version of the app, so an unusable entry is discarded rather than thrown.
 */

const storageKey = 'scholarship-cmgroups.session';

const roles: readonly UserRole[] = ['Applicant', 'Administrator'];

function storage(): Storage | null {
  try {
    return window.sessionStorage;
  } catch {
    return null;
  }
}

function isSession(value: unknown): value is AuthSession {
  if (typeof value !== 'object' || value === null) {
    return false;
  }

  const candidate = value as Partial<AuthSession>;

  return (
    typeof candidate.accessToken === 'string' &&
    candidate.accessToken.length > 0 &&
    typeof candidate.expiresAtUtc === 'string' &&
    typeof candidate.role === 'string' &&
    roles.includes(candidate.role) &&
    (candidate.applicantId === null || typeof candidate.applicantId === 'number')
  );
}

/** True once the token's expiry has passed, so an expired session is never presented as signed in. */
export function hasExpired(session: AuthSession, now: Date = new Date()): boolean {
  const expiry = Date.parse(session.expiresAtUtc);

  return Number.isNaN(expiry) || expiry <= now.getTime();
}

export function loadSession(now: Date = new Date()): AuthSession | null {
  const raw = storage()?.getItem(storageKey);
  if (raw === null || raw === undefined) {
    return null;
  }

  try {
    const parsed: unknown = JSON.parse(raw);
    if (!isSession(parsed) || hasExpired(parsed, now)) {
      clearSession();

      return null;
    }

    return parsed;
  } catch {
    clearSession();

    return null;
  }
}

export function saveSession(session: AuthSession): void {
  storage()?.setItem(storageKey, JSON.stringify(session));
}

export function clearSession(): void {
  storage()?.removeItem(storageKey);
}
