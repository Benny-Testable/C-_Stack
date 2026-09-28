import { afterEach, describe, expect, it, vi } from 'vitest';
import { ApiError } from './ApiError';
import { HttpClient } from './httpClient';

afterEach(() => {
  vi.unstubAllGlobals();
});

describe('HttpClient', () => {
  it('sends JSON and the bearer token', async () => {
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ id: 1 }), {
        status: 200,
        headers: { 'Content-Type': 'application/json' },
      }),
    );
    vi.stubGlobal('fetch', fetchMock);

    const client = new HttpClient({
      baseUrl: 'https://api.example.test/',
      getToken: () => 'secret-token',
    });

    await expect(client.post<{ id: number }>('/api/scholarships', { name: 'Merit' })).resolves.toEqual({
      id: 1,
    });

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0] as [string, RequestInit];
    expect(url).toBe('https://api.example.test/api/scholarships');
    expect(init.method).toBe('POST');
    expect(init.body).toBe(JSON.stringify({ name: 'Merit' }));
    const headers = new Headers(init.headers);
    expect(headers.get('Authorization')).toBe('Bearer secret-token');
    expect(headers.get('Content-Type')).toBe('application/json');
  });

  it('omits undefined query values', async () => {
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ items: [] }), { status: 200 }),
    );
    vi.stubGlobal('fetch', fetchMock);

    const client = new HttpClient({ baseUrl: 'https://api.example.test' });
    await client.get('/api/scholarships', { query: { search: 'merit', page: undefined } });

    const [url] = fetchMock.mock.calls[0] as [string];
    expect(url).toBe('https://api.example.test/api/scholarships?search=merit');
  });

  it('treats a 204 as undefined', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(null, { status: 204 })));
    const client = new HttpClient({ baseUrl: 'https://api.example.test' });

    await expect(client.delete('/api/scholarships/1')).resolves.toBeUndefined();
  });

  it('notifies the session when the API returns 401', async () => {
    const onUnauthorized = vi.fn();
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(null, { status: 401 })));

    const client = new HttpClient({
      baseUrl: 'https://api.example.test',
      onUnauthorized,
    });

    await expect(client.get('/api/applications')).rejects.toBeInstanceOf(ApiError);
    expect(onUnauthorized).toHaveBeenCalledTimes(1);
  });
});
