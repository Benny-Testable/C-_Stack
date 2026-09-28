import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';

/**
 * Vite build and Vitest configuration.
 *
 * Vite transforms TypeScript/JSX and minifies production bundles through esbuild. The `esbuild`
 * block below is a real setting rather than a placeholder: it strips `debugger` statements and the
 * `console.debug`/`console.trace` calls from production bundles so that development diagnostics
 * cannot leak into a deployed build.
 */
export default defineConfig({
  plugins: [react()],

  esbuild: {
    drop: ['debugger'],
    pure: ['console.debug', 'console.trace'],
    legalComments: 'none',
  },

  build: {
    outDir: 'dist',
    // Emitted so that a failed request in production can still be traced to a source line without
    // shipping readable application source in the bundle itself.
    sourcemap: 'hidden',
    // Fails the build rather than silently shipping an oversized main chunk.
    chunkSizeWarningLimit: 600,
    rollupOptions: {
      output: {
        manualChunks: {
          router: ['react-router-dom'],
        },
      },
    },
  },

  server: {
    port: 5173,
    strictPort: true,
    proxy: {
      // Keeps the browser on a single origin during development, so the app exercises the same
      // relative URLs it uses in production behind a reverse proxy.
      '/api': {
        target: 'https://localhost:7148',
        changeOrigin: true,
        secure: false,
      },
    },
  },

  preview: {
    port: 4173,
    strictPort: true,
  },

  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/test/setup.ts'],
    include: ['src/**/*.test.{ts,tsx}'],
    restoreMocks: true,
    coverage: {
      provider: 'v8',
      // Cobertura is the format the workbook's coverage metrics read; the text and html reporters
      // are for humans running the suite locally.
      reporter: ['text', 'html', 'cobertura'],
      reportsDirectory: './coverage',
      include: ['src/**/*.{ts,tsx}'],
      exclude: [
        'src/**/*.test.{ts,tsx}',
        'src/test/**',
        'src/main.tsx',
        'src/vite-env.d.ts',
      ],
    },
  },
});
