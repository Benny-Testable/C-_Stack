import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { postJson } from '../api/client';
import { saveSession, type Session } from '../auth/session';

export function LoginPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    try {
      const session = await postJson<Session>('/api/auth/login', { email, password });
      saveSession(session);
      void navigate('/');
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Sign-in failed.');
    }
  }

  return (
    <main>
      <h1>Sign in</h1>
      <form onSubmit={(event) => void onSubmit(event)}>
        <label>
          Email
          <input value={email} onChange={(event) => setEmail(event.target.value)} type="email" required />
        </label>
        <label>
          Password
          <input value={password} onChange={(event) => setPassword(event.target.value)} type="password" required />
        </label>
        {error ? <p role="alert">{error}</p> : null}
        <button type="submit">Sign in</button>
      </form>
      <p>
        New member? <Link to="/register">Create an account</Link>
      </p>
    </main>
  );
}
