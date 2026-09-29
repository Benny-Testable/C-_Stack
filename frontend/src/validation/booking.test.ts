import { cancellationError } from './booking';

describe('cancellationError', () => {
  it('closes inside two hours', () => {
    expect(cancellationError(1, false)).toMatch(/two hours/);
  });

  it('allows a later cancellation', () => {
    expect(cancellationError(5, false)).toBeNull();
  });
});
