import { useState, type FormEvent } from 'react';
import { postJson } from '../api/client';
import { loadSession } from '../auth/session';

export function AssessmentsPage() {
  const session = loadSession();
  const [heartRate, setHeartRate] = useState('70');
  const [bmi, setBmi] = useState('24.5');
  const [notes, setNotes] = useState('');
  const [result, setResult] = useState<string | null>(null);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (!session) {
      setResult('Sign in before recording an assessment.');
      return;
    }
    const plan = await postJson<{ title: string; sessionsPerWeek: number }>(
      '/api/assessments',
      { restingHeartRate: Number(heartRate), bodyMassIndex: Number(bmi), notes },
      session.token,
    );
    setResult(`${plan.title}: ${String(plan.sessionsPerWeek)} sessions a week.`);
  }

  return (
    <main>
      <h1>Fitness assessment</h1>
      <form onSubmit={(event) => void onSubmit(event)}>
        <label>
          Resting heart rate
          <input value={heartRate} onChange={(event) => setHeartRate(event.target.value)} inputMode="numeric" />
        </label>
        <label>
          Body mass index
          <input value={bmi} onChange={(event) => setBmi(event.target.value)} inputMode="decimal" />
        </label>
        <label>
          Notes
          <input value={notes} onChange={(event) => setNotes(event.target.value)} />
        </label>
        <button type="submit">Save assessment</button>
      </form>
      {result ? <p>{result}</p> : null}
    </main>
  );
}
