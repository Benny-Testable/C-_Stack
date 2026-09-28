import { describe, expect, it } from 'vitest';
import {
  emptyScholarshipForm,
  toScholarshipInput,
  validateScholarshipForm,
} from './scholarshipForm';

describe('validateScholarshipForm', () => {
  it('rejects the empty form on every required field', () => {
    const errors = validateScholarshipForm(emptyScholarshipForm);

    expect(errors.name).toBe('Name is required.');
    expect(errors.sponsorName).toBe('Sponsor is required.');
    expect(errors.awardAmount).toBe('Award amount is required.');
    expect(errors.totalSlots).toBe('Total slots is required.');
    expect(errors.applicationOpensOn).toBe('Opening date is required.');
    expect(errors.applicationClosesOn).toBe('Closing date is required.');
  });

  it('rejects inverted application dates', () => {
    const errors = validateScholarshipForm({
      name: 'Merit Award',
      description: '',
      sponsorName: 'Foundation',
      awardAmount: '5000',
      totalSlots: '10',
      applicationOpensOn: '2026-12-01',
      applicationClosesOn: '2026-01-01',
      isActive: true,
    });

    expect(errors.applicationClosesOn).toBe('The closing date must be later than the opening date.');
  });

  it('accepts a complete valid programme', () => {
    const values = {
      name: 'Merit Award',
      description: 'Undergraduate award.',
      sponsorName: 'Foundation',
      awardAmount: '5000',
      totalSlots: '10',
      applicationOpensOn: '2026-01-01',
      applicationClosesOn: '2026-12-01',
      isActive: true,
    };

    expect(validateScholarshipForm(values)).toEqual({});
    expect(toScholarshipInput(values)).toEqual({
      name: 'Merit Award',
      description: 'Undergraduate award.',
      sponsorName: 'Foundation',
      awardAmount: 5000,
      totalSlots: 10,
      applicationOpensOn: '2026-01-01',
      applicationClosesOn: '2026-12-01',
      isActive: true,
    });
  });

  it('sends a blank description as null', () => {
    const input = toScholarshipInput({
      ...emptyScholarshipForm,
      name: 'Merit Award',
      sponsorName: 'Foundation',
      awardAmount: '1',
      totalSlots: '1',
      applicationOpensOn: '2026-01-01',
      applicationClosesOn: '2026-12-01',
      description: '   ',
    });

    expect(input.description).toBeNull();
  });
});
