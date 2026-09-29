import type { Session } from '../auth/session';

const base = import.meta.env.VITE_API_BASE ?? '';

export async function postJson<T>(path: string, body: unknown, token?: string): Promise<T> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' };
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }
  const response = await fetch(`${base}${path}`, {
    method: 'POST',
    headers,
    body: JSON.stringify(body),
  });
  if (!response.ok) {
    const problem: unknown = await response.json().catch(() => ({ error: response.statusText }));
    const message = problem && typeof problem === 'object' && 'error' in problem ? String(problem.error) : response.statusText;
    throw new Error(message);
  }
  return (await response.json()) as T;
}

export async function getJson<T>(path: string, token?: string): Promise<T> {
  const headers: Record<string, string> = {};
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }
  const response = await fetch(`${base}${path}`, { headers });
  if (!response.ok) {
    throw new Error(response.statusText);
  }
  return (await response.json()) as T;
}

export interface Plan {
  id: string;
  name: string;
  monthlyPrice: number;
  maxClassesPerWeek: number;
  window: string;
}

export type { Session };
