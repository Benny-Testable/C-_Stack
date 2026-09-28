import { ApiError, toApiError } from './ApiError';

/**
 * The single place the application talks to the network.
 *
 * Every request funnels through {@link request}, so the bearer token, the JSON headers, the
 * cancellation signal, and the error translation are applied uniformly. Components never call
 * `fetch`, which is what keeps the duplication the workbook measures out of the UI layer.
 */

/** Supplies the current bearer token, or null when nobody is signed in. */
export type TokenProvider = () => string | null;

/** Invoked when the API reports 401, so the session can be cleared exactly once per failure. */
export type UnauthorizedHandler = () => void;

export interface HttpClientOptions {
  readonly baseUrl: string;
  readonly getToken?: TokenProvider;
  readonly onUnauthorized?: UnauthorizedHandler;
}

export interface RequestOptions {
  readonly method?: 'GET' | 'POST' | 'PUT' | 'DELETE';
  readonly body?: unknown;
  readonly query?: Readonly<Record<string, string | number | boolean | undefined>>;
  readonly signal?: AbortSignal;
}

export class HttpClient {
  private readonly baseUrl: string;
  private readonly getToken: TokenProvider;
  private readonly onUnauthorized: UnauthorizedHandler;

  public constructor(options: HttpClientOptions) {
    this.baseUrl = options.baseUrl.replace(/\/+$/u, '');
    this.getToken = options.getToken ?? (() => null);
    this.onUnauthorized = options.onUnauthorized ?? (() => undefined);
  }

  public async get<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
    return this.request<TResponse>(path, { ...options, method: 'GET' });
  }

  public async post<TResponse>(path: string, body?: unknown, options: RequestOptions = {}): Promise<TResponse> {
    return this.request<TResponse>(path, { ...options, method: 'POST', body });
  }

  public async put<TResponse>(path: string, body?: unknown, options: RequestOptions = {}): Promise<TResponse> {
    return this.request<TResponse>(path, { ...options, method: 'PUT', body });
  }

  public async delete<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
    return this.request<TResponse>(path, { ...options, method: 'DELETE' });
  }

  private async request<TResponse>(path: string, options: RequestOptions): Promise<TResponse> {
    const response = await fetch(this.buildUrl(path, options.query), {
      method: options.method ?? 'GET',
      headers: this.buildHeaders(options.body !== undefined),
      body: options.body === undefined ? null : JSON.stringify(options.body),
      ...(options.signal ? { signal: options.signal } : {}),
    });

    if (!response.ok) {
      const error = await toApiError(response);
      if (error.isUnauthorized) {
        this.onUnauthorized();
      }
      throw error;
    }

    return (await readJson<TResponse>(response)) as TResponse;
  }

  private buildUrl(path: string, query: RequestOptions['query']): string {
    const url = `${this.baseUrl}${path.startsWith('/') ? path : `/${path}`}`;

    if (query === undefined) {
      return url;
    }

    const search = new URLSearchParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined && value !== '') {
        search.append(key, String(value));
      }
    }

    const serialised = search.toString();

    return serialised.length > 0 ? `${url}?${serialised}` : url;
  }

  private buildHeaders(hasBody: boolean): HeadersInit {
    const headers: Record<string, string> = { Accept: 'application/json' };

    if (hasBody) {
      headers['Content-Type'] = 'application/json';
    }

    const token = this.getToken();
    if (token !== null && token !== '') {
      headers.Authorization = `Bearer ${token}`;
    }

    return headers;
  }
}

/**
 * Reads a JSON body, treating 204 and an empty body as `undefined`.
 *
 * `DELETE /api/scholarships/{id}` answers 204 with no body, so calling `response.json()`
 * unconditionally would throw on a successful request.
 */
async function readJson<TResponse>(response: Response): Promise<TResponse | undefined> {
  if (response.status === 204) {
    return undefined;
  }

  const text = await response.text();
  if (text.length === 0) {
    return undefined;
  }

  return JSON.parse(text) as TResponse;
}

export { ApiError };
