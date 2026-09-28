import { useState, type ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { useAsyncData } from '../hooks/useAsyncData';
import { EmptyState, ErrorMessage, LoadingIndicator } from '../components/Feedback';
import { Pagination } from '../components/Pagination';
import { StatusBadge } from '../components/StatusBadge';
import { statusLabel } from '../components/statusLabel';
import { formatDateTime } from '../components/format';
import { applicationStatuses, type ApplicationStatus, type PagedResult, type ScholarshipApplication } from '../api/types';

/**
 * Application inbox.
 *
 * Applicants see only their own rows; administrators see every application. The API, not this page,
 * is what enforces that split — the filter controls here only refine an already-scoped result.
 */
export function ApplicationListPage(): ReactNode {
  const { api, isAdministrator } = useAuth();
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState<ApplicationStatus | ''>('');

  const { state, reload } = useAsyncData<PagedResult<ScholarshipApplication>>(
    (signal) =>
      api.applications.list(
        {
          page,
          pageSize: 10,
          ...(status !== '' ? { status } : {}),
        },
        { signal },
      ),
    [api, page, status],
  );

  return (
    <section className="page">
      <header className="page__header">
        <h1>{isAdministrator ? 'All applications' : 'My applications'}</h1>
        <p className="page__lead">
          {isAdministrator
            ? 'Review submitted applications and record decisions.'
            : 'Track the status of every application you have opened.'}
        </p>
      </header>

      <form className="filters" onSubmit={(event) => event.preventDefault()}>
        <div className="filters__search">
          <label className="field__label" htmlFor="application-status">
            Status
          </label>
          <select
            id="application-status"
            className="field__input"
            value={status}
            onChange={(event) => {
              const next = event.target.value;
              setStatus(next === '' ? '' : (next as ApplicationStatus));
              setPage(1);
            }}
          >
            <option value="">Any status</option>
            {applicationStatuses.map((item) => (
              <option key={item} value={item}>
                {statusLabel(item)}
              </option>
            ))}
          </select>
        </div>
      </form>

      {state.status === 'loading' && <LoadingIndicator label="Loading applications…" />}
      {state.status === 'error' && <ErrorMessage message={state.error} onRetry={reload} />}

      {state.status === 'ready' && state.data.items.length === 0 && (
        <EmptyState message="No applications match the current filter." />
      )}

      {state.status === 'ready' && state.data.items.length > 0 && (
        <>
          <ul className="card-list">
            {state.data.items.map((application) => (
              <li className="card" key={application.id}>
                <h2 className="card__title">
                  <Link to={`/applications/${String(application.id)}`}>
                    {application.scholarshipName ?? `Scholarship #${String(application.scholarshipId)}`}
                  </Link>
                </h2>
                <p className="card__meta">Opened {formatDateTime(application.createdAtUtc)}</p>
                <StatusBadge status={application.status} />
              </li>
            ))}
          </ul>
          <Pagination page={state.data.page} totalPages={state.data.totalPages} onPageChange={setPage} />
        </>
      )}
    </section>
  );
}
