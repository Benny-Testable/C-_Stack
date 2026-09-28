declare namespace NodeJS {
  interface ProcessEnv {
    /** Public API origin. Empty means same origin as the page. Set at Webpack build time. */
    readonly API_BASE_URL?: string;
  }
}
