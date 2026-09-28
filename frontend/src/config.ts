/**
 * Runtime configuration, inlined at Webpack build time through `DefinePlugin`.
 *
 * `API_BASE_URL` defaults to an empty string, which makes every request relative to the page
 * origin — the correct behaviour behind a reverse proxy and what the Webpack dev-server proxy
 * expects.
 *
 * Only non-secret values belong here. Values are public in the bundle, so API keys, connection
 * strings, and signing keys must never be configured this way. See `.env.example`.
 */
export interface AppConfig {
  readonly apiBaseUrl: string;
}

export const appConfig: AppConfig = {
  apiBaseUrl: process.env.API_BASE_URL ?? '',
};
