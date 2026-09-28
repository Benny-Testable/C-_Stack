// Scholarship CMGroups — ESLint 8.47.0 configuration, PRIMARY tool only
// (Testable_Strategy_Metrics_Mapping_v0.2, Lint / Rule Violations, JS primary "eslint";
// Whitebox_Tools_Registry JavaScript #3 "eslint" 8.47.0). Core rules only, no plugins.
// Blocking rules are "error": the Lint / Rule Violations threshold is "0 blocking errors;
// < 10 warnings per KLOC" (White Box row 27).
module.exports = {
  root: true,
  env: { browser: true, es2022: true, node: true },
  parserOptions: { ecmaVersion: 2022, sourceType: 'module', ecmaFeatures: { jsx: true } },
  extends: ['eslint:recommended'],
  rules: {
    // Core ESLint cannot see identifiers used only inside JSX, so PascalCase names
    // (React components) are ignored to avoid false positives in .jsx files.
    'no-unused-vars': ['error', { args: 'after-used', ignoreRestSiblings: true, varsIgnorePattern: '^[A-Z]' }],
    'no-var': 'error',
    'prefer-const': 'error',
    eqeqeq: ['error', 'always'],
    camelcase: ['error', { properties: 'never' }],
    'no-console': 'warn',
    'max-depth': ['error', 3],
    'max-params': ['error', 5],
    'no-else-return': 'error'
  }
}
