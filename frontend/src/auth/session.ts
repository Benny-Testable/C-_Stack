const storageKey = 'wcf.session';

export interface Session {
  token: string;
  email: string;
  fullName: string;
  role: string;
}

export function loadSession(): Session | null {
  const raw = sessionStorage.getItem(storageKey);
  if (!raw) {
    return null;
  }
  const parsed: unknown = JSON.parse(raw);
  if (!parsed || typeof parsed !== 'object' || !('token' in parsed)) {
    return null;
  }
  return parsed as Session;
}

export function saveSession(session: Session): void {
  sessionStorage.setItem(storageKey, JSON.stringify(session));
}

export function clearSession(): void {
  sessionStorage.removeItem(storageKey);
}
