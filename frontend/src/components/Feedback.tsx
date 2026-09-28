import type { ReactNode } from 'react';

/**
 * Loading, error, and empty states.
 *
 * Collected in one module because every page needs all three, and because the accessibility details
 * (the live regions, the busy state) are easy to get subtly wrong when they are re-implemented per
 * page.
 */

export function LoadingIndicator({ label = 'Loading…' }: { readonly label?: string }): ReactNode {
  return (
    <p className="feedback feedback--loading" role="status" aria-live="polite" aria-busy="true">
      {label}
    </p>
  );
}

/**
 * `role="alert"` so a screen reader announces the failure immediately; a retry is offered only when
 * the caller can actually retry.
 */
export function ErrorMessage({
  message,
  onRetry,
}: {
  readonly message: string;
  readonly onRetry?: () => void;
}): ReactNode {
  return (
    <div className="feedback feedback--error" role="alert">
      <p>{message}</p>
      {onRetry !== undefined && (
        <button type="button" className="button button--secondary" onClick={onRetry}>
          Try again
        </button>
      )}
    </div>
  );
}

export function EmptyState({ message }: { readonly message: string }): ReactNode {
  return (
    <p className="feedback feedback--empty" role="status">
      {message}
    </p>
  );
}

/** Confirms that a mutation succeeded, announced politely so it does not interrupt. */
export function SuccessMessage({ message }: { readonly message: string }): ReactNode {
  return (
    <p className="feedback feedback--success" role="status" aria-live="polite">
      {message}
    </p>
  );
}
