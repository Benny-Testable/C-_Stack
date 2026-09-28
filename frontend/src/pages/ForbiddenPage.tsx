import type { ReactNode } from 'react';
import { Link } from 'react-router-dom';

export function ForbiddenPage(): ReactNode {
  return (
    <section className="page page--narrow">
      <header className="page__header">
        <h1>Not allowed</h1>
        <p className="page__lead">Your account does not have permission to open that page.</p>
      </header>
      <p>
        <Link to="/scholarships">Return to scholarships</Link>
      </p>
    </section>
  );
}
