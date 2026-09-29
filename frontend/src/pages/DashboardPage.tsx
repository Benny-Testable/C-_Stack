import { Link } from 'react-router-dom';
import { loadSession } from '../auth/session';

export function DashboardPage() {
  const session = loadSession();
  return (
    <main>
      <h1>West Coast Fitness Club</h1>
      <p>Scottsdale and Tucson training floors, memberships, and class reservations.</p>
      {session ? <p>Signed in as {session.fullName}.</p> : <p>Sign in to book a class or record an assessment.</p>}
      <nav>
        <Link to="/plans">Plans</Link>
        <Link to="/classes">Classes</Link>
        <Link to="/assessments">Assessments</Link>
      </nav>
    </main>
  );
}
