import { useEffect, useState } from 'react';
import { getJson, type Plan } from '../api/client';

export function PlansPage() {
  const [plans, setPlans] = useState<Plan[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    void getJson<Plan[]>('/api/plans')
      .then(setPlans)
      .catch((caught: unknown) => {
        setError(caught instanceof Error ? caught.message : 'Plans are unavailable.');
      });
  }, []);

  return (
    <main>
      <h1>Membership plans</h1>
      {error ? <p role="alert">{error}</p> : null}
      <ul>
        {plans.map((plan) => (
          <li key={plan.id}>
            <strong>{plan.name}</strong> — ${plan.monthlyPrice} / month, {plan.maxClassesPerWeek} classes a week
            <p>{plan.window}</p>
          </li>
        ))}
      </ul>
    </main>
  );
}
