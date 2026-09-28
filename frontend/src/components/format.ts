/**
 * Presentation formatting.
 *
 * Every formatter tolerates an unparseable value by returning a dash. A malformed date from the API
 * should show as "not set", not as "Invalid Date" or a thrown exception that blanks the page.
 *
 * The locale is fixed to `en-GB` so that rendered output is identical in the browser, in CI, and in
 * the test suite. Leaving it to the host locale makes assertions on formatted text fail depending on
 * the machine, which is a common source of the flaky tests the workbook measures.
 */
const placeholder = '—';

const dateFormatter = new Intl.DateTimeFormat('en-GB', {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
});

const dateTimeFormatter = new Intl.DateTimeFormat('en-GB', {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
  timeZone: 'UTC',
});

const currencyFormatter = new Intl.NumberFormat('en-GB', {
  style: 'currency',
  currency: 'USD',
  currencyDisplay: 'narrowSymbol',
  maximumFractionDigits: 0,
});

function parse(value: string | null): Date | null {
  if (value === null || value.length === 0) {
    return null;
  }

  const parsed = new Date(value);

  return Number.isNaN(parsed.getTime()) ? null : parsed;
}

export function formatDate(value: string | null): string {
  const parsed = parse(value);

  return parsed === null ? placeholder : dateFormatter.format(parsed);
}

export function formatDateTime(value: string | null): string {
  const parsed = parse(value);

  return parsed === null ? placeholder : `${dateTimeFormatter.format(parsed)} UTC`;
}

export function formatCurrency(value: number): string {
  return Number.isFinite(value) ? currencyFormatter.format(value) : placeholder;
}

export function formatOptionalText(value: string | null): string {
  return value === null || value.trim().length === 0 ? placeholder : value;
}
