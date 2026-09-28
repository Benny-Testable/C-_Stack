import { nodeResolve } from '@rollup/plugin-node-resolve'
import commonjs from '@rollup/plugin-commonjs'
import json from '@rollup/plugin-json'
import { babel } from '@rollup/plugin-babel'
import replace from '@rollup/plugin-replace'
import html from '@rollup/plugin-html'
import postcss from 'rollup-plugin-postcss'
import copy from 'rollup-plugin-copy'
import terser from '@rollup/plugin-terser'

const production = process.env.NODE_ENV !== 'development'

export default {
  input: 'src/main.tsx',
  output: {
    dir: 'dist',
    format: 'es',
    entryFileNames: 'assets/main.[hash].js',
    sourcemap: !production,
  },
  plugins: [
    replace({
      preventAssignment: true,
      'process.env.NODE_ENV': JSON.stringify(production ? 'production' : 'development'),
    }),
    nodeResolve({ extensions: ['.js', '.jsx', '.ts', '.tsx'] }),
    commonjs(),
    json(),
    babel({
      babelHelpers: 'bundled',
      extensions: ['.js', '.jsx', '.ts', '.tsx'],
      exclude: 'node_modules/**',
    }),
    postcss({ extract: 'assets/main.css' }),
    copy({
      targets: [{ src: 'public/favicon.svg', dest: 'dist' }],
    }),
    html({
      title: 'West Coast Fitness Club',
      publicPath: '/',
      template: async ({ attributes, files, meta, publicPath, title }) => {
        const scripts = (files.js || [])
          .map(({ fileName }) => `<script type="module" src="${publicPath}${fileName}"></script>`)
          .join('\n    ')
        const links = [`<link rel="stylesheet" href="${publicPath}assets/main.css">`].join('\n    ')
        return `<!doctype html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <link rel="icon" type="image/svg+xml" href="${publicPath}favicon.svg" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>${title}</title>
    ${links}
  </head>
  <body>
    <div id="root"></div>
    ${scripts}
  </body>
</html>
`
      },
    }),
    production && terser(),
  ].filter(Boolean),
}
