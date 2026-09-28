import type { ReactNode } from 'react';

/**
 * Page navigation for a `PagedResult`.
 *
 * Renders nothing for a single page rather than showing disabled controls, so a short list is not
 * cluttered by navigation that cannot do anything.
 */
interface PaginationProps {
  readonly page: number;
  readonly totalPages: number;
  readonly onPageChange: (page: number) => void;
}

export function Pagination({ page, totalPages, onPageChange }: PaginationProps): ReactNode {
  if (totalPages <= 1) {
    return null;
  }

  return (
    <nav className="pagination" aria-label="Pagination">
      <button
        type="button"
        className="button button--secondary"
        onClick={() => {
          onPageChange(page - 1);
        }}
        disabled={page <= 1}
      >
        Previous
      </button>

      <span className="pagination__status" aria-live="polite">
        {`Page ${String(page)} of ${String(totalPages)}`}
      </span>

      <button
        type="button"
        className="button button--secondary"
        onClick={() => {
          onPageChange(page + 1);
        }}
        disabled={page >= totalPages}
      >
        Next
      </button>
    </nav>
  );
}
