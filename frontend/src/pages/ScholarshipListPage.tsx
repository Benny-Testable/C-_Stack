import { useCallback, useState, type ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { useAsyncData } from '../hooks/useAsyncData';
import { EmptyState, ErrorMessage, LoadingIndicator } from '../components/Feedback';
import { Pagination } from '../components/Pagination';
import { formatCurrency, formatDate } from '../components/format';
import type { PagedResult, Scholarship } from '../api/types';

/**
 * Browsable scholarship catalogue.
 *
 * Public: the API allows anonymous reads on this route so a prospective applicant can see what is on
 * offer before creating an account.
 *
 * Filtering is server-side. Fetching every row and filtering in the browser would work at this data
 * volume and fail badly later, and it is the pattern the workbook's query-efficiency metrics look for.
 */
export function ScholarshipListPage(): ReactNode {
  const { api } = useAuth();
  const [page, setPage] = useState(1);
  const [activeOnly, setActiveOnly] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');

  const { state, reload } = useAsyncData<PagedResult<Scholarship>>(
    (signal) => api.scholarships.list({ page, pageSize: 10, activeOnly, search }, { signal }),
    [api, page, activeOnly, search],
  );

  const applySearch = useCallback((): void => {
    setSearch(searchInput.trim());
    setPage(1);
  }, [searchInput]);

  return (
    <section className="page">
      <header className="page__header">
        <h1>Scholarships</h1>
        <p className="page__lead">Browse the programmes currently accepting applications.</p>
      </header>

      <form
        className="filters"
        onSubmit={(event) => {
          event.preventDefault();
          applySearch();
        }}
      >
        <div className="filters__search">
          <label className="field__label" htmlFor="scholarship-search">
            Search by name or sponsor
          </label>
          <input
            id="scholarship-search"
            className="field__input"
            type="search"
            value={searchInput}
            onChange={(event) => {
              setSearchInput(event.target.value);
            }}
            maxLength={200}
          />
        </div>

        <div className="filters__actions">
          <button type="submit" className="button button--primary">
            Search
          </button>
          <label className="filters__toggle" htmlFor="scholarship-active-only">
            <input
              id="scholarship-active-only"
              type="checkbox"
              checked={activeOnly}
              onChange={(event) => {
                setActiveOnly(event.target.checked);
                setPage(1);
              }}
            />
            Open programmes only
          </label>
        </div>
      </form>

      {state.status === 'loading' && <LoadingIndicator label="Loading scholarships…" />}
      {state.status === 'error' && <ErrorMessage message={state.error} onRetry={reload} />}

      {state.status === 'ready' && state.data.items.length === 0 && (
        <EmptyState message="No scholarships match the current filters." />
      )}

      {state.status === 'ready' && state.data.items.length > 0 && (
        <>
          <ul className="card-list">
            {state.data.items.map((scholarship) => (
              <li className="card" key={scholarship.id}>
                <h2 className="card__title">
                  <Link to={`/scholarships/${String(scholarship.id)}`}>{scholarship.name}</Link>
                </h2>
                <p className="card__meta">{scholarship.sponsorName}</p>
                <dl className="card__facts">
                  <div>
                    <dt>Award</dt>
                    <dd>{formatCurrency(scholarship.awardAmount)}</dd>
                  </div>
                  <div>
                    <dt>Places</dt>
                    <dd>{scholarship.totalSlots}</dd>
                  </div>
                  <div>
                    <dt>Closes</dt>
                    <dd>{formatDate(scholarship.applicationClosesOn)}</dd>
                  </div>
                </dl>
              </li>
            ))}
          </ul>

          <Pagination page={state.data.page} totalPages={state.data.totalPages} onPageChange={setPage} />
        </>
      )}
    </section>
  );
}
