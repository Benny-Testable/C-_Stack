import { useState, type ReactNode } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { useAsyncData } from '../hooks/useAsyncData';
import { useSubmit } from '../hooks/useSubmit';
import { ErrorMessage, LoadingIndicator } from '../components/Feedback';
import { CheckboxField, TextAreaField, TextField } from '../components/FormField';
import {
  emptyScholarshipForm,
  toScholarshipForm,
  toScholarshipInput,
  validateScholarshipForm,
  type ScholarshipFormValues,
} from '../validation/scholarshipForm';
import { fieldLimits, hasErrors, type ValidationErrors } from '../validation/validators';
import type { Scholarship } from '../api/types';

/**
 * Create or replace a scholarship programme.
 *
 * Create and edit share one form so the validation rules cannot drift. The route parameter decides
 * which API method is called after the same client-side checks pass.
 */
export function ScholarshipFormPage(): ReactNode {
  const { id } = useParams<{ id: string }>();
  const isEdit = id !== undefined;
  const scholarshipId = isEdit ? Number(id) : null;
  const { api } = useAuth();

  if (isEdit && (scholarshipId === null || !Number.isInteger(scholarshipId) || scholarshipId <= 0)) {
    return <ErrorMessage message="That scholarship reference is not valid." />;
  }

  if (scholarshipId === null) {
    return <ScholarshipEditor initial={emptyScholarshipForm} scholarshipId={null} />;
  }

  return <ScholarshipEditorLoader scholarshipId={scholarshipId} api={api} />;
}

function ScholarshipEditorLoader({
  scholarshipId,
  api,
}: {
  readonly scholarshipId: number;
  readonly api: ReturnType<typeof useAuth>['api'];
}): ReactNode {
  const { state, reload } = useAsyncData<Scholarship>(
    (signal) => api.scholarships.getById(scholarshipId, { signal }),
    [api, scholarshipId],
  );

  if (state.status === 'loading') {
    return <LoadingIndicator label="Loading scholarship…" />;
  }

  if (state.status === 'error') {
    return <ErrorMessage message={state.error} onRetry={reload} />;
  }

  return <ScholarshipEditor initial={toScholarshipForm(state.data)} scholarshipId={scholarshipId} />;
}

function ScholarshipEditor({
  initial,
  scholarshipId,
}: {
  readonly initial: ScholarshipFormValues;
  readonly scholarshipId: number | null;
}): ReactNode {
  const { api } = useAuth();
  const navigate = useNavigate();
  const [values, setValues] = useState<ScholarshipFormValues>(initial);
  const [errors, setErrors] = useState<ValidationErrors<ScholarshipFormValues>>({});
  const isEdit = scholarshipId !== null;

  const { state, submit } = useSubmit(
    isEdit
      ? (input: ReturnType<typeof toScholarshipInput>) => api.scholarships.update(scholarshipId, input)
      : api.scholarships.create,
  );

  const persist = async (): Promise<void> => {
    const nextErrors = validateScholarshipForm(values);
    setErrors(nextErrors);
    if (hasErrors(nextErrors)) {
      return;
    }

    const saved = await submit(toScholarshipInput(values));
    if (saved !== null) {
      await navigate(`/scholarships/${String(saved.id)}`);
    }
  };

  const setField = <K extends keyof ScholarshipFormValues>(key: K, value: ScholarshipFormValues[K]): void => {
    setValues((current) => ({ ...current, [key]: value }));
  };

  return (
    <section className="page page--narrow">
      <header className="page__header">
        <h1>{isEdit ? 'Edit scholarship' : 'New scholarship'}</h1>
        <p className="page__lead">
          {isEdit ? 'Replace the programme details. Every field is revalidated on save.' : 'Add a programme to the catalogue.'}
        </p>
      </header>

      <form
        className="form"
        onSubmit={(event) => {
          event.preventDefault();
          void persist();
        }}
        noValidate
      >
        <ScholarshipFields values={values} errors={errors} onChange={setField} />
        {state.error !== null && <ErrorMessage message={state.error} />}
        <button type="submit" className="button button--primary" disabled={state.isSubmitting}>
          {state.isSubmitting ? 'Saving…' : isEdit ? 'Save changes' : 'Create scholarship'}
        </button>
      </form>
    </section>
  );
}

function ScholarshipFields({
  values,
  errors,
  onChange,
}: {
  readonly values: ScholarshipFormValues;
  readonly errors: ValidationErrors<ScholarshipFormValues>;
  readonly onChange: <K extends keyof ScholarshipFormValues>(key: K, value: ScholarshipFormValues[K]) => void;
}): ReactNode {
  return (
    <>
      <TextField
        id="scholarship-name"
        label="Name"
        required
        maxLength={fieldLimits.nameMax}
        value={values.name}
        onChange={(name) => {
          onChange('name', name);
        }}
        {...(errors.name !== undefined ? { error: errors.name } : {})}
      />
      <TextField
        id="scholarship-sponsor"
        label="Sponsor"
        required
        maxLength={fieldLimits.nameMax}
        value={values.sponsorName}
        onChange={(sponsorName) => {
          onChange('sponsorName', sponsorName);
        }}
        {...(errors.sponsorName !== undefined ? { error: errors.sponsorName } : {})}
      />
      <TextAreaField
        id="scholarship-description"
        label="Description"
        hint="Optional."
        maxLength={fieldLimits.longTextMax}
        value={values.description}
        onChange={(description) => {
          onChange('description', description);
        }}
        {...(errors.description !== undefined ? { error: errors.description } : {})}
      />
      <TextField
        id="scholarship-award"
        label="Award amount"
        type="number"
        required
        min="0.01"
        step="0.01"
        value={values.awardAmount}
        onChange={(awardAmount) => {
          onChange('awardAmount', awardAmount);
        }}
        {...(errors.awardAmount !== undefined ? { error: errors.awardAmount } : {})}
      />
      <TextField
        id="scholarship-slots"
        label="Total slots"
        type="number"
        required
        min="1"
        step="1"
        value={values.totalSlots}
        onChange={(totalSlots) => {
          onChange('totalSlots', totalSlots);
        }}
        {...(errors.totalSlots !== undefined ? { error: errors.totalSlots } : {})}
      />
      <TextField
        id="scholarship-opens"
        label="Opens on"
        type="date"
        required
        value={values.applicationOpensOn}
        onChange={(applicationOpensOn) => {
          onChange('applicationOpensOn', applicationOpensOn);
        }}
        {...(errors.applicationOpensOn !== undefined ? { error: errors.applicationOpensOn } : {})}
      />
      <TextField
        id="scholarship-closes"
        label="Closes on"
        type="date"
        required
        value={values.applicationClosesOn}
        onChange={(applicationClosesOn) => {
          onChange('applicationClosesOn', applicationClosesOn);
        }}
        {...(errors.applicationClosesOn !== undefined ? { error: errors.applicationClosesOn } : {})}
      />
      <CheckboxField
        id="scholarship-active"
        label="Programme is open for applications"
        checked={values.isActive}
        onChange={(isActive) => {
          onChange('isActive', isActive);
        }}
      />
    </>
  );
}
