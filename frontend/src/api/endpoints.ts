import type { HttpClient, RequestOptions } from './httpClient';
import type {
  Applicant,
  ApplicantInput,
  ApplicationStatus,
  AuthSession,
  ErasureReceipt,
  PagedResult,
  Scholarship,
  ScholarshipApplication,
  ScholarshipInput,
  ScholarshipStatistics,
} from './types';

/**
 * Typed wrappers around the API routes.
 *
 * One function per endpoint, grouped by resource. Route strings live only here, so a route change is
 * a one-line edit and no component contains a URL.
 */

export interface ScholarshipQuery {
  readonly page?: number;
  readonly pageSize?: number;
  readonly activeOnly?: boolean;
  readonly search?: string;
}

export interface ApplicationQuery {
  readonly page?: number;
  readonly pageSize?: number;
  readonly scholarshipId?: number;
  readonly status?: ApplicationStatus;
}

export function createAuthApi(client: HttpClient) {
  return {
    register: (
      input: { fullName: string; email: string; password: string; institutionName: string | null },
      options?: RequestOptions,
    ): Promise<AuthSession> => client.post<AuthSession>('/api/auth/register', input, options),

    login: (
      input: { email: string; password: string },
      options?: RequestOptions,
    ): Promise<AuthSession> => client.post<AuthSession>('/api/auth/login', input, options),
  };
}

export function createScholarshipApi(client: HttpClient) {
  return {
    list: (query: ScholarshipQuery, options?: RequestOptions): Promise<PagedResult<Scholarship>> =>
      client.get<PagedResult<Scholarship>>('/api/scholarships', {
        ...options,
        query: asQuery(query),
      }),

    getById: (id: number, options?: RequestOptions): Promise<Scholarship> =>
      client.get<Scholarship>(`/api/scholarships/${String(id)}`, options),

    create: (input: ScholarshipInput, options?: RequestOptions): Promise<Scholarship> =>
      client.post<Scholarship>('/api/scholarships', input, options),

    update: (id: number, input: ScholarshipInput, options?: RequestOptions): Promise<Scholarship> =>
      client.put<Scholarship>(`/api/scholarships/${String(id)}`, input, options),

    remove: (id: number, options?: RequestOptions): Promise<void> =>
      client.delete<void>(`/api/scholarships/${String(id)}`, options),

    statistics: (id: number, options?: RequestOptions): Promise<ScholarshipStatistics> =>
      client.get<ScholarshipStatistics>(`/api/scholarships/${String(id)}/statistics`, options),
  };
}

export function createApplicationApi(client: HttpClient) {
  return {
    list: (query: ApplicationQuery, options?: RequestOptions): Promise<PagedResult<ScholarshipApplication>> =>
      client.get<PagedResult<ScholarshipApplication>>('/api/applications', {
        ...options,
        query: asQuery(query),
      }),

    getById: (id: number, options?: RequestOptions): Promise<ScholarshipApplication> =>
      client.get<ScholarshipApplication>(`/api/applications/${String(id)}`, options),

    createDraft: (
      input: { scholarshipId: number; applicantId: number; motivation: string | null },
      options?: RequestOptions,
    ): Promise<ScholarshipApplication> =>
      client.post<ScholarshipApplication>('/api/applications', input, options),

    updateDraft: (
      id: number,
      input: { motivation: string | null },
      options?: RequestOptions,
    ): Promise<ScholarshipApplication> =>
      client.put<ScholarshipApplication>(`/api/applications/${String(id)}`, input, options),

    transition: (
      id: number,
      input: { targetStatus: ApplicationStatus; reviewerNotes: string | null },
      options?: RequestOptions,
    ): Promise<ScholarshipApplication> =>
      client.post<ScholarshipApplication>(`/api/applications/${String(id)}/transitions`, input, options),
  };
}

export function createApplicantApi(client: HttpClient) {
  return {
    getById: (id: number, options?: RequestOptions): Promise<Applicant> =>
      client.get<Applicant>(`/api/applicants/${String(id)}`, options),

    update: (id: number, input: ApplicantInput, options?: RequestOptions): Promise<Applicant> =>
      client.put<Applicant>(`/api/applicants/${String(id)}`, input, options),

    erase: (id: number, options?: RequestOptions): Promise<ErasureReceipt> =>
      client.delete<ErasureReceipt>(`/api/applicants/${String(id)}`, options),
  };
}

export type AuthApi = ReturnType<typeof createAuthApi>;
export type ScholarshipApi = ReturnType<typeof createScholarshipApi>;
export type ApplicationApi = ReturnType<typeof createApplicationApi>;
export type ApplicantApi = ReturnType<typeof createApplicantApi>;

function asQuery(
  values: ScholarshipQuery | ApplicationQuery,
): Readonly<Record<string, string | number | boolean | undefined>> {
  const result: Record<string, string | number | boolean | undefined> = {};

  for (const [key, value] of Object.entries(values)) {
    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
      result[key] = value;
    }
  }

  return result;
}
