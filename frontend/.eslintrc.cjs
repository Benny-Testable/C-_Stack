// Scholarship CMGroups — ESLint 8.47.0 configuration.
// Registry tools: eslint (core), eslint-sonarjs (cognitive complexity), eslint-security (SAST).
// Blocking rules are "error": the Lint / Rule Violations threshold is "0 blocking errors;
// < 10 warnings per KLOC" (Testable_Strategy_Metrics_Mapping_v0.2, White Box row 27).
module.exports = {
  root: true,
  env: { browser: true, es2022: true, node: true },
  parserOptions: { ecmaVersion: 2022, sourceType: 'module', ecmaFeatures: { jsx: true } },
  settings: { react: { version: '18.3' } },
  plugins: ['react', 'sonarjs', 'security'],
  extends: ['eslint:recommended', 'plugin:react/recommended', 'plugin:react/jsx-runtime'],
  rules: {
    'no-unused-vars': ['error', { args: 'after-used', ignoreRestSiblings: true }],
    'no-var': 'error',
    'prefer-const': 'error',
    eqeqeq: ['error', 'always'],
    camelcase: ['error', { properties: 'never' }],
    'no-console': 'warn',
    'max-depth': ['error', 3],
    'max-params': ['error', 5],
    'no-else-return': 'error',
    'react/prop-types': 'off',
    'sonarjs/cognitive-complexity': ['error', 15],
    'sonarjs/no-duplicate-string': ['error', { threshold: 3 }],
    'sonarjs/no-identical-functions': 'error',
    'sonarjs/no-collapsible-if': 'error',
    'security/detect-non-literal-regexp': 'error',
    'security/detect-unsafe-regex': 'error'
  }
}
