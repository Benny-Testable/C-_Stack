import { useCallback, useRef, useState } from 'react';
import { ApiError } from '../api/ApiError';
import { describeError } from './useAsyncData';

/**
 * Runs a mutating request, tracking in-flight state and validation errors.
 *
 * Guards against double submission: while a request is in flight further calls return immediately.
 * Without that, a double-clicked "Submit" button produces two applications, one of which the API then
 * rejects with a 409.
 *
 * Field-level messages from an {@link ApiError} are exposed separately from the summary message so a
 * form can put each message next to the input it belongs to.
 */
export interface SubmitState {
  readonly isSubmitting: boolean;
  readonly error: string | null;
  readonly fieldErrors: Readonly<Record<string, readonly string[]>>;
}

export interface SubmitResult<TArgs extends readonly unknown[], TResponse> {
  readonly state: SubmitState;
  readonly submit: (...args: TArgs) => Promise<TResponse | null>;
  readonly reset: () => void;
}

const idle: SubmitState = { isSubmitting: false, error: null, fieldErrors: {} };

export function useSubmit<TArgs extends readonly unknown[], TResponse>(
  action: (...args: TArgs) => Promise<TResponse>,
): SubmitResult<TArgs, TResponse> {
  const [state, setState] = useState<SubmitState>(idle);

  // The guard lives in a ref, not in state: a state updater may run more than once per commit, and
  // the check has to be immediate rather than waiting for the next render.
  const inFlight = useRef(false);

  const reset = useCallback((): void => {
    setState(idle);
  }, []);

  const submit = useCallback(
    async (...args: TArgs): Promise<TResponse | null> => {
      if (inFlight.current) {
        return null;
      }

      inFlight.current = true;
      setState({ isSubmitting: true, error: null, fieldErrors: {} });

      try {
        const response = await action(...args);
        setState(idle);

        return response;
      } catch (error: unknown) {
        setState({
          isSubmitting: false,
          error: describeError(error),
          fieldErrors: error instanceof ApiError ? error.fieldErrors : {},
        });

        return null;
      } finally {
        inFlight.current = false;
      }
    },
    [action],
  );

  return { state, submit, reset };
}
