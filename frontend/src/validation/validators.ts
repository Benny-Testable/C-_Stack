/**
 * Client-side form validation.
 *
 * These rules mirror the DataAnnotations on the server contracts so a user is told about a problem
 * before a request is sent. They are a usability measure only — the server revalidates everything,
 * because a client check can be bypassed. Where a limit appears in both places it is the same number,
 * so the two layers cannot disagree.
 */

/** Messages keyed by field name; an empty object means the form is valid. */
export type ValidationErrors<T> = Partial<Record<keyof T, string>>;

export const fieldLimits = {
  nameMax: 200,
  emailMax: 256,
  longTextMax: 2000,
  passwordMax: 256,
  /** Matches `Authentication:MinimumPasswordLength`. See docs/clarifications.md item C-04. */
  passwordMin: 12,
} as const;

/**
 * Deliberately permissive: it rejects the obviously malformed while accepting the unusual but legal.
 * A strict pattern rejects valid addresses, and the server plus a confirmation step are the real
 * check.
 */
const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/u;

export function isBlank(value: string): boolean {
  return value.trim().length === 0;
}

export function validateRequiredText(
  value: string,
  label: string,
  { min = 1, max }: { min?: number; max: number },
): string | undefined {
  const trimmed = value.trim();

  if (trimmed.length === 0) {
    return `${label} is required.`;
  }

  if (trimmed.length < min) {
    return `${label} must be at least ${String(min)} characters.`;
  }

  if (trimmed.length > max) {
    return `${label} must be ${String(max)} characters or fewer.`;
  }

  return undefined;
}

export function validateOptionalText(
  value: string,
  label: string,
  max: number,
): string | undefined {
  return value.trim().length > max ? `${label} must be ${String(max)} characters or fewer.` : undefined;
}

export function validateEmail(value: string): string | undefined {
  const trimmed = value.trim();

  if (trimmed.length === 0) {
    return 'Email is required.';
  }

  if (trimmed.length > fieldLimits.emailMax) {
    return `Email must be ${String(fieldLimits.emailMax)} characters or fewer.`;
  }

  return emailPattern.test(trimmed) ? undefined : 'Enter a valid email address.';
}

export function validatePassword(value: string): string | undefined {
  if (value.length === 0) {
    return 'Password is required.';
  }

  if (value.length < fieldLimits.passwordMin) {
    return `Password must be at least ${String(fieldLimits.passwordMin)} characters.`;
  }

  return value.length > fieldLimits.passwordMax
    ? `Password must be ${String(fieldLimits.passwordMax)} characters or fewer.`
    : undefined;
}

export function validatePositiveNumber(value: string, label: string): string | undefined {
  if (isBlank(value)) {
    return `${label} is required.`;
  }

  const parsed = Number(value);

  if (!Number.isFinite(parsed)) {
    return `${label} must be a number.`;
  }

  return parsed > 0 ? undefined : `${label} must be greater than zero.`;
}

export function validatePositiveInteger(value: string, label: string): string | undefined {
  const numberError = validatePositiveNumber(value, label);
  if (numberError !== undefined) {
    return numberError;
  }

  return Number.isInteger(Number(value)) ? undefined : `${label} must be a whole number.`;
}

export function validateDate(value: string, label: string): string | undefined {
  if (isBlank(value)) {
    return `${label} is required.`;
  }

  return Number.isNaN(Date.parse(value)) ? `${label} must be a valid date.` : undefined;
}

/** The server enforces the same rule in `ScholarshipRequest.Validate`. */
export function validateDateOrder(
  opensOn: string,
  closesOn: string,
): string | undefined {
  if (isBlank(opensOn) || isBlank(closesOn)) {
    return undefined;
  }

  const opens = Date.parse(opensOn);
  const closes = Date.parse(closesOn);

  if (Number.isNaN(opens) || Number.isNaN(closes)) {
    return undefined;
  }

  return closes > opens ? undefined : 'The closing date must be later than the opening date.';
}

export function hasErrors<T>(errors: ValidationErrors<T>): boolean {
  return Object.values(errors).some((message) => message !== undefined);
}
