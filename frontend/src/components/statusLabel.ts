import type { ApplicationStatus } from '../api/types';

const labels: Readonly<Record<ApplicationStatus, string>> = {
  Draft: 'Draft',
  Submitted: 'Submitted',
  UnderReview: 'Under review',
  Approved: 'Approved',
  Rejected: 'Rejected',
  Withdrawn: 'Withdrawn',
};

export function statusLabel(status: ApplicationStatus): string {
  return labels[status];
}
