import { describe, expect, it } from 'vitest';
import { formatCurrency, formatDate, formatDateTime, formatOptionalText } from './format';

describe('formatDate', () => {
  it('renders a parseable date in a stable locale', () => {
    expect(formatDate('2026-03-15')).toBe('15 Mar 2026');
  });

  it('renders a dash for an unparseable value', () => {
    expect(formatDate('nonsense')).toBe('—');
    expect(formatDate(null)).toBe('—');
  });
});

describe('formatDateTime', () => {
  it('appends a UTC marker', () => {
    expect(formatDateTime('2026-03-15T12:30:00.000Z')).toContain('UTC');
  });
});

describe('formatCurrency', () => {
  it('renders a finite amount', () => {
    expect(formatCurrency(5000)).toBe('$5,000');
  });

  it('renders a dash for a non-finite amount', () => {
    expect(formatCurrency(Number.NaN)).toBe('—');
  });
});

describe('formatOptionalText', () => {
  it('renders a dash for blank text', () => {
    expect(formatOptionalText('   ')).toBe('—');
    expect(formatOptionalText(null)).toBe('—');
  });

  it('returns the original non-blank text', () => {
    expect(formatOptionalText('Merit award')).toBe('Merit award');
  });
});
