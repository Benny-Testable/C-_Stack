/**
 * An HTTP failure translated into something the UI can render.
 *
 * The API answers failures with RFC 7807 ProblemDetails, so the useful message is in `detail` or
 * `title` rather than in the status text. Parsing that here means no component ever has to inspect a
 * raw Response, and no raw server text is interpolated into the DOM.
 */
export class ApiError extends Error {
  public readonly status: number;

  /** Field-level messages keyed by property name, as produced by MVC model validation. */
  public readonly fieldErrors: Readonly<Record<string, readonly string[]>>;

  public constructor(
    status: number,
    message: string,
    fieldErrors: Readonly<Record<string, readonly string[]>> = {},
  ) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.fieldErrors = fieldErrors;
  }

  public get isUnauthorized(): boolean {
    return this.status === 401;
  }

  public get isForbidden(): boolean {
    return this.status === 403;
  }

  public get isNotFound(): boolean {
    return this.status === 404;
  }

  public get isRateLimited(): boolean {
    return this.status === 429;
  }
}

interface ProblemDetails {
  readonly title?: string;
  readonly detail?: string;
  readonly errors?: Record<string, string[]>;
}

const fallbackMessages: Readonly<Record<number, string>> = {
  400: 'The request was rejected. Please check the highlighted fields.',
  401: 'Your session has expired. Please sign in again.',
  403: 'You do not have permission to perform that action.',
  404: 'The requested item could not be found.',
  409: 'That change conflicts with the current state of the record.',
  429: 'Too many requests. Please wait a moment and try again.',
};

function fallbackMessage(status: number): string {
  return fallbackMessages[status] ?? 'Something went wrong. Please try again.';
}

/**
 * Builds an {@link ApiError} from a failed response.
 *
 * A body that is missing, empty, or not JSON is tolerated: the status-based fallback is used rather
 * than surfacing a parse error, because the user needs to know what failed, not how the body was
 * malformed.
 */
export async function toApiError(response: Response): Promise<ApiError> {
  let problem: ProblemDetails | null = null;

  try {
    const text = await response.text();
    if (text.length > 0) {
      problem = JSON.parse(text) as ProblemDetails;
    }
  } catch {
    problem = null;
  }

  const message = problem?.detail ?? problem?.title ?? fallbackMessage(response.status);

  return new ApiError(response.status, message, problem?.errors ?? {});
}
