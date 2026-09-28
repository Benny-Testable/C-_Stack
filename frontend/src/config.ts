/**
 * Runtime configuration, read from Vite environment variables.
 *
 * `VITE_API_BASE_URL` is the only setting. It defaults to an empty string, which makes every request
 * relative to the page origin — the correct behaviour behind a reverse proxy and the configuration
 * the dev server proxy in `vite.config.ts` expects.
 *
 * Only non-secret values belong here. Anything placed in a `VITE_`-prefixed variable is inlined into
 * the bundle and is therefore public, so API keys, connection strings, and signing keys must never be
 * configured this way. See `.env.example`.
 */
export interface AppConfig {
  readonly apiBaseUrl: string;
}

export const appConfig: AppConfig = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? '',
};
