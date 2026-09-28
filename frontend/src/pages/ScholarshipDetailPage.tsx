import { useState, type ReactNode } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { useAsyncData } from '../hooks/useAsyncData';
import { useSubmit } from '../hooks/useSubmit';
import { ErrorMessage, LoadingIndicator } from '../components/Feedback';
import { TextAreaField } from '../components/FormField';
import { formatCurrency, formatDate, formatOptionalText } from '../components/format';
import { fieldLimits, validateOptionalText } from '../validation/validators';
import type { Scholarship } from '../api/types';

/**
 * One scholarship, with the apply form for a signed-in applicant.
 *
 * The apply form is shown only when the caller has an applicant id. An administrator has no applicant
 * record, so offering them the form would produce a request the API refuses.
 */
export function ScholarshipDetailPage(): ReactNode {
  const { id } = useParams<{ id: string }>();
  const scholarshipId = Number(id);
  const { api } = useAuth();

  const { state, reload } = useAsyncData<Scholarship>(
    (signal) => api.scholarships.getById(scholarshipId, { signal }),
    [api, scholarshipId],
  );

  if (!Number.isInteger(scholarshipId) || scholarshipId <= 0) {
    return <ErrorMessage message="That scholarship reference is not valid." />;
  }

  if (state.status === 'loading') {
    return <LoadingIndicator label="Loading scholarship…" />;
  }

  if (state.status === 'error') {
    return <ErrorMessage message={state.error} onRetry={reload} />;
  }

  return <ScholarshipDetailBody scholarship={state.data} />;
}

function ScholarshipDetailBody({ scholarship }: { readonly scholarship: Scholarship }): ReactNode {
  const { isAdministrator } = useAuth();

  return (
    <section className="page">
      <header className="page__header">
        <h1>{scholarship.name}</h1>
        <p className="page__lead">{scholarship.sponsorName}</p>
        {isAdministrator && (
          <p>
            <Link to={`/admin/scholarships/${String(scholarship.id)}/edit`}>Edit programme</Link>
          </p>
        )}
      </header>

      <dl className="detail-grid">
        <div>
          <dt>Award</dt>
          <dd>{formatCurrency(scholarship.awardAmount)}</dd>
        </div>
        <div>
          <dt>Places</dt>
          <dd>{scholarship.totalSlots}</dd>
        </div>
        <div>
          <dt>Opens</dt>
          <dd>{formatDate(scholarship.applicationOpensOn)}</dd>
        </div>
        <div>
          <dt>Closes</dt>
          <dd>{formatDate(scholarship.applicationClosesOn)}</dd>
        </div>
        <div>
          <dt>Status</dt>
          <dd>{scholarship.isActive ? 'Open' : 'Closed'}</dd>
        </div>
      </dl>

      <section className="panel">
        <h2>About this programme</h2>
        <p>{formatOptionalText(scholarship.description)}</p>
      </section>

      <ApplyPanel scholarshipId={scholarship.id} />
    </section>
  );
}

function ApplyPanel({ scholarshipId }: { readonly scholarshipId: number }): ReactNode {
  const { api, isAuthenticated, applicantId } = useAuth();
  const navigate = useNavigate();
  const [motivation, setMotivation] = useState('');
  const [motivationError, setMotivationError] = useState<string | undefined>(undefined);
  const { state: submitState, submit } = useSubmit(api.applications.createDraft);

  if (!isAuthenticated) {
    return (
      <section className="panel">
        <h2>Apply</h2>
        <p>
          <Link to="/login">Sign in</Link> or <Link to="/register">register</Link> to apply for this
          scholarship.
        </p>
      </section>
    );
  }

  if (applicantId === null) {
    return (
      <section className="panel">
        <h2>Apply</h2>
        <p>Administrator accounts manage programmes and do not submit applications.</p>
      </section>
    );
  }

  const apply = async (): Promise<void> => {
    const error = validateOptionalText(motivation, 'Motivation', fieldLimits.longTextMax);
    setMotivationError(error);
    if (error !== undefined) {
      return;
    }

    const trimmed = motivation.trim();
    const created = await submit({
      scholarshipId,
      applicantId,
      motivation: trimmed.length > 0 ? trimmed : null,
    });

    if (created !== null) {
      await navigate(`/applications/${String(created.id)}`);
    }
  };

  return (
    <section className="panel">
      <h2>Apply</h2>
      <form
        className="form"
        onSubmit={(event) => {
          event.preventDefault();
          void apply();
        }}
      >
        <TextAreaField
          id="motivation"
          label="Motivation statement"
          hint="Optional. You can add or revise this while the application is still a draft."
          value={motivation}
          onChange={setMotivation}
          maxLength={fieldLimits.longTextMax}
          {...(motivationError !== undefined ? { error: motivationError } : {})}
        />
        {submitState.error !== null && <ErrorMessage message={submitState.error} />}
        <button type="submit" className="button button--primary" disabled={submitState.isSubmitting}>
          {submitState.isSubmitting ? 'Creating draft…' : 'Start application'}
        </button>
      </form>
    </section>
  );
}
