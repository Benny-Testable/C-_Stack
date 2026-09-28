import type { ReactNode } from 'react';
import { Link } from 'react-router-dom';

export function NotFoundPage(): ReactNode {
  return (
    <section className="page page--narrow">
      <header className="page__header">
        <h1>Page not found</h1>
        <p className="page__lead">That address is not a route in Scholarship CMGroups.</p>
      </header>
      <p>
        <Link to="/scholarships">Return to scholarships</Link>
      </p>
    </section>
  );
}
