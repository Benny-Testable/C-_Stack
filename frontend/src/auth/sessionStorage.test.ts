import { afterEach, describe, expect, it } from '@jest/globals';
import { clearSession, hasExpired, loadSession, saveSession } from './sessionStorage';
import type { AuthSession } from '../api/types';

const valid: AuthSession = {
  accessToken: 'token',
  expiresAtUtc: '2099-01-01T00:00:00.000Z',
  role: 'Applicant',
  applicantId: 3,
};

afterEach(() => {
  sessionStorage.clear();
});

describe('hasExpired', () => {
  it('treats a missing expiry as expired', () => {
    expect(hasExpired({ ...valid, expiresAtUtc: 'not-a-date' })).toBe(true);
  });

  it('treats a future expiry as current', () => {
    expect(hasExpired(valid, new Date('2026-01-01T00:00:00.000Z'))).toBe(false);
  });
});

describe('session persistence', () => {
  it('round-trips a valid session', () => {
    saveSession(valid);

    expect(loadSession()).toEqual(valid);
  });

  it('discards an expired session', () => {
    saveSession({ ...valid, expiresAtUtc: '2000-01-01T00:00:00.000Z' });

    expect(loadSession()).toBeNull();
    expect(sessionStorage.getItem('scholarship-cmgroups.session')).toBeNull();
  });

  it('discards a malformed payload instead of throwing', () => {
    sessionStorage.setItem('scholarship-cmgroups.session', '{not-json');

    expect(loadSession()).toBeNull();
  });

  it('clears the stored session', () => {
    saveSession(valid);
    clearSession();

    expect(loadSession()).toBeNull();
  });
});
