import type { Scholarship, ScholarshipInput } from '../api/types';
import {
  fieldLimits,
  validateDate,
  validateDateOrder,
  validateOptionalText,
  validatePositiveInteger,
  validatePositiveNumber,
  validateRequiredText,
  type ValidationErrors,
} from './validators';

/**
 * The scholarship form as the inputs hold it: every field is a string, because that is what a DOM
 * input produces. Conversion to the API shape happens in {@link toScholarshipInput}, after validation
 * has confirmed the strings are convertible.
 */
export interface ScholarshipFormValues {
  name: string;
  description: string;
  sponsorName: string;
  awardAmount: string;
  totalSlots: string;
  applicationOpensOn: string;
  applicationClosesOn: string;
  isActive: boolean;
}

export const emptyScholarshipForm: ScholarshipFormValues = {
  name: '',
  description: '',
  sponsorName: '',
  awardAmount: '',
  totalSlots: '',
  applicationOpensOn: '',
  applicationClosesOn: '',
  isActive: true,
};

export function toScholarshipForm(scholarship: Scholarship): ScholarshipFormValues {
  return {
    name: scholarship.name,
    description: scholarship.description ?? '',
    sponsorName: scholarship.sponsorName,
    awardAmount: String(scholarship.awardAmount),
    totalSlots: String(scholarship.totalSlots),
    applicationOpensOn: scholarship.applicationOpensOn,
    applicationClosesOn: scholarship.applicationClosesOn,
    isActive: scholarship.isActive,
  };
}

export function toScholarshipInput(values: ScholarshipFormValues): ScholarshipInput {
  const description = values.description.trim();

  return {
    name: values.name.trim(),
    description: description.length > 0 ? description : null,
    sponsorName: values.sponsorName.trim(),
    awardAmount: Number(values.awardAmount),
    totalSlots: Number(values.totalSlots),
    applicationOpensOn: values.applicationOpensOn,
    applicationClosesOn: values.applicationClosesOn,
    isActive: values.isActive,
  };
}

export function validateScholarshipForm(
  values: ScholarshipFormValues,
): ValidationErrors<ScholarshipFormValues> {
  const errors: ValidationErrors<ScholarshipFormValues> = {};

  const name = validateRequiredText(values.name, 'Name', { min: 3, max: fieldLimits.nameMax });
  if (name !== undefined) {
    errors.name = name;
  }

  const sponsor = validateRequiredText(values.sponsorName, 'Sponsor', { min: 2, max: fieldLimits.nameMax });
  if (sponsor !== undefined) {
    errors.sponsorName = sponsor;
  }

  const description = validateOptionalText(values.description, 'Description', fieldLimits.longTextMax);
  if (description !== undefined) {
    errors.description = description;
  }

  const award = validatePositiveNumber(values.awardAmount, 'Award amount');
  if (award !== undefined) {
    errors.awardAmount = award;
  }

  const slots = validatePositiveInteger(values.totalSlots, 'Total slots');
  if (slots !== undefined) {
    errors.totalSlots = slots;
  }

  const opensOn = validateDate(values.applicationOpensOn, 'Opening date');
  if (opensOn !== undefined) {
    errors.applicationOpensOn = opensOn;
  }

  const closesOn =
    validateDate(values.applicationClosesOn, 'Closing date') ??
    validateDateOrder(values.applicationOpensOn, values.applicationClosesOn);
  if (closesOn !== undefined) {
    errors.applicationClosesOn = closesOn;
  }

  return errors;
}
