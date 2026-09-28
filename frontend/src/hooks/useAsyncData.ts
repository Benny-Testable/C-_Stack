import { useCallback, useEffect, useState } from 'react';
import { ApiError } from '../api/ApiError';

/**
 * Loads data for a view, tracking the loading and error states the UI needs.
 *
 * Two behaviours matter here and are the reason this is a hook rather than inline `useEffect` calls:
 *
 * 1. Every request is cancelled when the component unmounts or the inputs change, so a slow response
 *    cannot resolve into an unmounted component or overwrite a newer result.
 * 2. `AbortError` is not reported. A cancelled request is not a failure, and surfacing it would flash
 *    an error banner on every navigation.
 */
export type AsyncState<T> =
  | { readonly status: 'loading'; readonly data: T | null; readonly error: null }
  | { readonly status: 'ready'; readonly data: T; readonly error: null }
  | { readonly status: 'error'; readonly data: null; readonly error: string };

export interface AsyncResult<T> {
  readonly state: AsyncState<T>;
  /** Re-runs the loader, for example after a mutation elsewhere on the page. */
  readonly reload: () => void;
}

function isAbort(error: unknown): boolean {
  return error instanceof DOMException && error.name === 'AbortError';
}

export function describeError(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'Something went wrong. Please try again.';
}

export function useAsyncData<T>(
  load: (signal: AbortSignal) => Promise<T>,
  dependencies: readonly unknown[],
): AsyncResult<T> {
  const [state, setState] = useState<AsyncState<T>>({ status: 'loading', data: null, error: null });
  const [reloadToken, setReloadToken] = useState(0);

  const reload = useCallback((): void => {
    setState({ status: 'loading', data: null, error: null });
    setReloadToken((token) => token + 1);
  }, []);

  useEffect(() => {
    const controller = new AbortController();

    // Loading is set from the async path's first tick via the initial state and from `reload`.
    // A synchronous setState here is flagged by react-hooks/set-state-in-effect; the fetch
    // callbacks below are the supported place to publish results.
    void load(controller.signal)
      .then((data) => {
        if (!controller.signal.aborted) {
          setState({ status: 'ready', data, error: null });
        }
      })
      .catch((error: unknown) => {
        if (controller.signal.aborted || isAbort(error)) {
          return;
        }
        setState({ status: 'error', data: null, error: describeError(error) });
      });

    return () => {
      controller.abort();
    };
    // `load` is intentionally excluded: callers pass an inline closure, so including it would
    // re-run the effect on every render. The explicit dependency list is what identifies a request.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...dependencies, reloadToken]);

  return { state, reload };
}
