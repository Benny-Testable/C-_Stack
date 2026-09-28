import { TextDecoder, TextEncoder } from 'util';

Object.assign(globalThis, {
  TextEncoder,
  TextDecoder,
});

class TestHeaders {
  private readonly values = new Map<string, string>();

  constructor(init?: HeadersInit) {
    if (init instanceof TestHeaders) {
      init.forEach((value, key) => this.values.set(key.toLowerCase(), value));
      return;
    }

    if (Array.isArray(init)) {
      for (const [key, value] of init) {
        this.values.set(key.toLowerCase(), value);
      }
      return;
    }

    if (init && typeof init === 'object') {
      for (const [key, value] of Object.entries(init)) {
        this.values.set(key.toLowerCase(), String(value));
      }
    }
  }

  get(name: string): string | null {
    return this.values.get(name.toLowerCase()) ?? null;
  }

  forEach(callback: (value: string, key: string) => void): void {
    this.values.forEach((value, key) => callback(value, key));
  }
}

class TestResponse {
  readonly status: number;
  readonly ok: boolean;
  readonly headers: TestHeaders;
  private readonly body: string | null;

  constructor(body: BodyInit | null = null, init: ResponseInit = {}) {
    this.status = init.status ?? 200;
    this.ok = this.status >= 200 && this.status < 300;
    this.headers = new TestHeaders(init.headers);
    this.body = typeof body === 'string' ? body : null;
  }

  text(): Promise<string> {
    return Promise.resolve(this.body ?? '');
  }

  json(): Promise<unknown> {
    return Promise.resolve(JSON.parse(this.body ?? 'null') as unknown);
  }
}

Object.assign(globalThis, {
  Headers: TestHeaders,
  Response: TestResponse,
});
