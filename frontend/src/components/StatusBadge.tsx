import type { ReactNode } from 'react';
import type { ApplicationStatus } from '../api/types';
import { statusLabel } from './statusLabel';

/**
 * Renders an application status.
 *
 * Colour never stands in for the label: the text is always present so the state is readable without
 * colour vision.
 */
export function StatusBadge({ status }: { readonly status: ApplicationStatus }): ReactNode {
  return (
    <span className={`badge badge--${status.toLowerCase()}`} data-testid="status-badge">
      {statusLabel(status)}
    </span>
  );
}
