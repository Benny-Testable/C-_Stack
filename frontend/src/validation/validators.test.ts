import { describe, expect, it } from 'vitest';
import {
  fieldLimits,
  hasErrors,
  validateDate,
  validateDateOrder,
  validateEmail,
  validateOptionalText,
  validatePassword,
  validatePositiveInteger,
  validatePositiveNumber,
  validateRequiredText,
} from './validators';

describe('validateRequiredText', () => {
  it('rejects blank and whitespace-only values', () => {
    expect(validateRequiredText('   ', 'Name', { max: 10 })).toBe('Name is required.');
  });

  it('rejects values shorter than the minimum', () => {
    expect(validateRequiredText('ab', 'Name', { min: 3, max: 10 })).toBe(
      'Name must be at least 3 characters.',
    );
  });

  it('rejects values longer than the maximum', () => {
    expect(validateRequiredText('abcdefghijk', 'Name', { max: 10 })).toBe(
      'Name must be 10 characters or fewer.',
    );
  });

  it('accepts a value on the allowed range', () => {
    expect(validateRequiredText(' Ada ', 'Name', { min: 2, max: 10 })).toBeUndefined();
  });
});

describe('validateOptionalText', () => {
  it('accepts blank optional text', () => {
    expect(validateOptionalText('', 'Description', 10)).toBeUndefined();
  });

  it('rejects optional text that exceeds the maximum', () => {
    expect(validateOptionalText('abcdefghijk', 'Description', 10)).toBe(
      'Description must be 10 characters or fewer.',
    );
  });
});

describe('validateEmail', () => {
  it('requires a value', () => {
    expect(validateEmail('')).toBe('Email is required.');
  });

  it('rejects an address without a domain', () => {
    expect(validateEmail('person@')).toBe('Enter a valid email address.');
  });

  it('accepts a well-formed address', () => {
    expect(validateEmail('person@example.edu')).toBeUndefined();
  });

  it('rejects an address longer than the shared maximum', () => {
    const local = 'a'.repeat(fieldLimits.emailMax);
    expect(validateEmail(`${local}@x.y`)).toBe(
      `Email must be ${String(fieldLimits.emailMax)} characters or fewer.`,
    );
  });
});

describe('validatePassword', () => {
  it('requires a password', () => {
    expect(validatePassword('')).toBe('Password is required.');
  });

  it('rejects a password shorter than the configured minimum', () => {
    expect(validatePassword('short')).toBe(
      `Password must be at least ${String(fieldLimits.passwordMin)} characters.`,
    );
  });

  it('accepts a password at the minimum length', () => {
    expect(validatePassword('a'.repeat(fieldLimits.passwordMin))).toBeUndefined();
  });
});

describe('numeric and date validators', () => {
  it('rejects a non-numeric award amount', () => {
    expect(validatePositiveNumber('abc', 'Award amount')).toBe('Award amount must be a number.');
  });

  it('rejects zero and negative amounts', () => {
    expect(validatePositiveNumber('0', 'Award amount')).toBe('Award amount must be greater than zero.');
    expect(validatePositiveNumber('-1', 'Award amount')).toBe('Award amount must be greater than zero.');
  });

  it('rejects a fractional slot count', () => {
    expect(validatePositiveInteger('1.5', 'Total slots')).toBe('Total slots must be a whole number.');
  });

  it('accepts a positive integer slot count', () => {
    expect(validatePositiveInteger('12', 'Total slots')).toBeUndefined();
  });

  it('rejects an unparseable date', () => {
    expect(validateDate('not-a-date', 'Opening date')).toBe('Opening date must be a valid date.');
  });

  it('requires the closing date to be later than the opening date', () => {
    expect(validateDateOrder('2026-12-31', '2026-01-01')).toBe(
      'The closing date must be later than the opening date.',
    );
  });

  it('accepts a closing date after the opening date', () => {
    expect(validateDateOrder('2026-01-01', '2026-12-31')).toBeUndefined();
  });
});

describe('hasErrors', () => {
  it('is false for an empty error map', () => {
    expect(hasErrors({})).toBe(false);
  });

  it('is true when any field has a message', () => {
    expect(hasErrors({ name: 'Name is required.' })).toBe(true);
  });
});
