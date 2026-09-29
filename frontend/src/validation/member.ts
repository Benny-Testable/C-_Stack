export interface RegistrationInput {
  email: string;
  fullName: string;
  password: string;
}

export function registrationError(input: RegistrationInput): string | null {
  const email = input.email.trim();
  const name = input.fullName.trim();
  if (!email.includes('@') || email.length > 256) {
    return 'Enter a valid email address.';
  }
  if (name.length < 2) {
    return "Enter the member's full name.";
  }
  if (input.password.length < 10) {
    return 'Password must be at least 10 characters.';
  }
  const hasUpper = /[A-Z]/.test(input.password);
  const hasLower = /[a-z]/.test(input.password);
  const hasDigit = /[0-9]/.test(input.password);
  if (!hasUpper || !hasLower || !hasDigit) {
    return 'Password must include an uppercase letter, a lowercase letter, and a digit.';
  }
  return null;
}
