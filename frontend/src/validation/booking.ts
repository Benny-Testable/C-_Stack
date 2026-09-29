export function cancellationError(hoursUntilClass: number, alreadyCancelled: boolean): string | null {
  if (alreadyCancelled) {
    return 'The reservation is already cancelled.';
  }
  if (hoursUntilClass < 2) {
    return 'Cancellations close two hours before the class.';
  }
  return null;
}
