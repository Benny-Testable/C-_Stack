/**
 * Types mirroring the contracts in `backend/src/ScholarshipCMGroups.Api/Contracts`.
 *
 * Kept in one module so a change to the API surface is a change in a single place on the client.
 * The enum is declared as a union of string literals because the API serialises enums by name
 * (`JsonStringEnumConverter`), so the wire format and the type stay identical.
 */

export const applicationStatuses = [
  'Draft',
  'Submitted',
  'UnderReview',
  'Approved',
  'Rejected',
  'Withdrawn',
] as const;

export type ApplicationStatus = (typeof applicationStatuses)[number];

export type UserRole = 'Applicant' | 'Administrator';

export interface PagedResult<T> {
  readonly items: readonly T[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
}

export interface Scholarship {
  readonly id: number;
  readonly name: string;
  readonly description: string | null;
  readonly sponsorName: string;
  readonly awardAmount: number;
  readonly totalSlots: number;
  /** ISO date, no time component (`DateOnly` on the server). */
  readonly applicationOpensOn: string;
  readonly applicationClosesOn: string;
  readonly isActive: boolean;
  readonly createdAtUtc: string;
  readonly updatedAtUtc: string | null;
}

export interface ScholarshipInput {
  readonly name: string;
  readonly description: string | null;
  readonly sponsorName: string;
  readonly awardAmount: number;
  readonly totalSlots: number;
  readonly applicationOpensOn: string;
  readonly applicationClosesOn: string;
  readonly isActive: boolean;
}

export interface Applicant {
  readonly id: number;
  readonly fullName: string;
  readonly email: string;
  readonly institutionName: string | null;
  readonly createdAtUtc: string;
  readonly updatedAtUtc: string | null;
}

export interface ApplicantInput {
  readonly fullName: string;
  readonly email: string;
  readonly institutionName: string | null;
}

export interface ScholarshipApplication {
  readonly id: number;
  readonly scholarshipId: number;
  readonly scholarshipName: string | null;
  readonly applicantId: number;
  readonly status: ApplicationStatus;
  readonly statusName: string;
  readonly motivation: string | null;
  readonly reviewerNotes: string | null;
  readonly submittedAtUtc: string | null;
  readonly decidedAtUtc: string | null;
  readonly createdAtUtc: string;
  readonly allowedNextStatuses: readonly ApplicationStatus[];
}

export interface AuthSession {
  readonly accessToken: string;
  readonly expiresAtUtc: string;
  readonly role: UserRole;
  readonly applicantId: number | null;
}

export interface ScholarshipStatistics {
  readonly scholarshipId: number;
  readonly totalSlots: number;
  readonly submittedCount: number;
  readonly underReviewCount: number;
  readonly approvedCount: number;
  readonly rejectedCount: number;
  readonly remainingSlots: number;
}

export interface ErasureReceipt {
  readonly applicantId: number;
  readonly applicationsDeleted: number;
  readonly erasedAtUtc: string;
}
