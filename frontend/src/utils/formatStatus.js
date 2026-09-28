export function formatStatusLabel(status) {
  if (!status) {
    return 'Unknown'
  }
  return String(status)
}

export const STATUS_PENDING = 'Pending'
export const STATUS_APPROVED = 'Approved'
export const STATUS_REJECTED = 'Rejected'
