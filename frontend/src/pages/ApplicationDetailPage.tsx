import { useState, type ReactNode } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { useAsyncData } from '../hooks/useAsyncData';
import { useSubmit } from '../hooks/useSubmit';
import { ErrorMessage, LoadingIndicator, SuccessMessage } from '../components/Feedback';
import { TextAreaField } from '../components/FormField';
import { StatusBadge } from '../components/StatusBadge';
import { statusLabel } from '../components/statusLabel';
import { formatDateTime, formatOptionalText } from '../components/format';
import { fieldLimits, validateOptionalText } from '../validation/validators';
import type { ApplicationStatus, ScholarshipApplication } from '../api/types';

/**
 * One application: draft editing, lifecycle transitions, and reviewer notes.
 *
 * Allowed next statuses come from the API rather than being recomputed here, so the client cannot
 * offer a transition the server would reject. The server still re-checks the same table.
 */
export function ApplicationDetailPage(): ReactNode {
  const { id } = useParams<{ id: string }>();
  const applicationId = Number(id);
  const { api, isAdministrator } = useAuth();

  const { state, reload } = useAsyncData<ScholarshipApplication>(
    (signal) => api.applications.getById(applicationId, { signal }),
    [api, applicationId],
  );

  if (!Number.isInteger(applicationId) || applicationId <= 0) {
    return <ErrorMessage message="That application reference is not valid." />;
  }

  if (state.status === 'loading') {
    return <LoadingIndicator label="Loading application…" />;
  }

  if (state.status === 'error') {
    return <ErrorMessage message={state.error} onRetry={reload} />;
  }

  return (
    <ApplicationDetail
      application={state.data}
      isAdministrator={isAdministrator}
      onReload={reload}
    />
  );
}

function ApplicationDetail({
  application,
  isAdministrator,
  onReload,
}: {
  readonly application: ScholarshipApplication;
  readonly isAdministrator: boolean;
  readonly onReload: () => void;
}): ReactNode {
  const { api } = useAuth();
  const [motivation, setMotivation] = useState(application.motivation ?? '');
  const [motivationError, setMotivationError] = useState<string | undefined>(undefined);
  const [notes, setNotes] = useState('');
  const [saved, setSaved] = useState(false);

  const { state: saveState, submit: saveDraft } = useSubmit(api.applications.updateDraft);
  const { state: transitionState, submit: transition } = useSubmit(api.applications.transition);

  const persistDraft = async (): Promise<void> => {
    const error = validateOptionalText(motivation, 'Motivation', fieldLimits.longTextMax);
    setMotivationError(error);
    if (error !== undefined) {
      return;
    }

    const trimmed = motivation.trim();
    const updated = await saveDraft(application.id, {
      motivation: trimmed.length > 0 ? trimmed : null,
    });
    if (updated !== null) {
      setSaved(true);
      onReload();
    }
  };

  const moveTo = async (targetStatus: ApplicationStatus): Promise<void> => {
    const trimmed = notes.trim();
    const updated = await transition(application.id, {
      targetStatus,
      reviewerNotes: trimmed.length > 0 ? trimmed : null,
    });
    if (updated !== null) {
      setSaved(false);
      onReload();
    }
  };

  return (
    <section className="page">
      <header className="page__header">
        <h1>
          <Link to={`/scholarships/${String(application.scholarshipId)}`}>
            {application.scholarshipName ?? `Scholarship #${String(application.scholarshipId)}`}
          </Link>
        </h1>
        <p className="page__lead">Application #{String(application.id)}</p>
        <StatusBadge status={application.status} />
      </header>

      <dl className="detail-grid">
        <div>
          <dt>Opened</dt>
          <dd>{formatDateTime(application.createdAtUtc)}</dd>
        </div>
        <div>
          <dt>Submitted</dt>
          <dd>{formatDateTime(application.submittedAtUtc)}</dd>
        </div>
        <div>
          <dt>Decided</dt>
          <dd>{formatDateTime(application.decidedAtUtc)}</dd>
        </div>
      </dl>

      <MotivationPanel
        application={application}
        motivation={motivation}
        motivationError={motivationError}
        saved={saved}
        saveState={saveState}
        onMotivationChange={(value) => {
          setMotivation(value);
          setSaved(false);
        }}
        onSave={() => {
          void persistDraft();
        }}
      />

      {application.reviewerNotes !== null && application.reviewerNotes.length > 0 && (
        <section className="panel">
          <h2>Reviewer notes</h2>
          <p>{application.reviewerNotes}</p>
        </section>
      )}

      <TransitionPanel
        application={application}
        isAdministrator={isAdministrator}
        notes={notes}
        transitionState={transitionState}
        onNotesChange={setNotes}
        onMove={(next) => {
          void moveTo(next);
        }}
      />
    </section>
  );
}

function MotivationPanel({
  application,
  motivation,
  motivationError,
  saved,
  saveState,
  onMotivationChange,
  onSave,
}: {
  readonly application: ScholarshipApplication;
  readonly motivation: string;
  readonly motivationError: string | undefined;
  readonly saved: boolean;
  readonly saveState: { readonly isSubmitting: boolean; readonly error: string | null };
  readonly onMotivationChange: (value: string) => void;
  readonly onSave: () => void;
}): ReactNode {
  if (application.status !== 'Draft') {
    return (
      <section className="panel">
        <h2>Motivation statement</h2>
        <p>{formatOptionalText(application.motivation)}</p>
      </section>
    );
  }

  return (
    <section className="panel">
      <h2>Motivation statement</h2>
      <form
        className="form"
        onSubmit={(event) => {
          event.preventDefault();
          onSave();
        }}
      >
        <TextAreaField
          id="application-motivation"
          label="Motivation"
          value={motivation}
          onChange={onMotivationChange}
          maxLength={fieldLimits.longTextMax}
          {...(motivationError !== undefined ? { error: motivationError } : {})}
        />
        {saveState.error !== null && <ErrorMessage message={saveState.error} />}
        {saved && <SuccessMessage message="Draft saved." />}
        <button type="submit" className="button button--secondary" disabled={saveState.isSubmitting}>
          {saveState.isSubmitting ? 'Saving…' : 'Save draft'}
        </button>
      </form>
    </section>
  );
}

function TransitionPanel({
  application,
  isAdministrator,
  notes,
  transitionState,
  onNotesChange,
  onMove,
}: {
  readonly application: ScholarshipApplication;
  readonly isAdministrator: boolean;
  readonly notes: string;
  readonly transitionState: { readonly isSubmitting: boolean; readonly error: string | null };
  readonly onNotesChange: (value: string) => void;
  readonly onMove: (status: ApplicationStatus) => void;
}): ReactNode {
  if (application.allowedNextStatuses.length === 0) {
    return null;
  }

  return (
    <section className="panel">
      <h2>Next step</h2>
      {isAdministrator && (
        <TextAreaField
          id="reviewer-notes"
          label="Reviewer notes"
          hint="Optional. Recorded with approve and reject decisions."
          value={notes}
          onChange={onNotesChange}
          maxLength={fieldLimits.longTextMax}
        />
      )}
      {transitionState.error !== null && <ErrorMessage message={transitionState.error} />}
      <div className="button-row">
        {application.allowedNextStatuses.map((next) => (
          <button
            key={next}
            type="button"
            className={next === 'Rejected' || next === 'Withdrawn' ? 'button button--danger' : 'button button--primary'}
            disabled={transitionState.isSubmitting}
            onClick={() => {
              onMove(next);
            }}
          >
            {statusLabel(next)}
          </button>
        ))}
      </div>
    </section>
  );
}
