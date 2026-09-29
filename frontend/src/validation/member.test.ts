import { registrationError } from './member';

describe('registrationError', () => {
  it('accepts a mixed password', () => {
    expect(registrationError({
      email: 'avery@westcoast.example',
      fullName: 'Avery Stone',
      password: 'DesertGym10',
    })).toBeNull();
  });

  it('rejects a short password', () => {
    expect(registrationError({
      email: 'avery@westcoast.example',
      fullName: 'Avery Stone',
      password: 'short',
    })).toMatch(/10 characters/);
  });
});
