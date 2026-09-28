import { describe, expect, it } from 'vitest';
import { ApiError, toApiError } from './ApiError';

describe('ApiError flags', () => {
  it('classifies common status codes', () => {
    expect(new ApiError(401, 'expired').isUnauthorized).toBe(true);
    expect(new ApiError(403, 'no').isForbidden).toBe(true);
    expect(new ApiError(404, 'missing').isNotFound).toBe(true);
    expect(new ApiError(429, 'slow down').isRateLimited).toBe(true);
  });
});

describe('toApiError', () => {
  it('reads RFC 7807 detail and field errors', async () => {
    const response = new Response(
      JSON.stringify({
        title: 'Validation failed',
        detail: 'Name is required.',
        errors: { Name: ['Name is required.'] },
      }),
      { status: 400 },
    );

    const error = await toApiError(response);

    expect(error.message).toBe('Name is required.');
    expect(error.fieldErrors.Name).toEqual(['Name is required.']);
  });

  it('falls back to a status message when the body is empty', async () => {
    const error = await toApiError(new Response(null, { status: 404 }));

    expect(error.message).toBe('The requested item could not be found.');
  });

  it('falls back when the body is not JSON', async () => {
    const error = await toApiError(new Response('<html>nope</html>', { status: 500 }));

    expect(error.message).toBe('Something went wrong. Please try again.');
  });
});
