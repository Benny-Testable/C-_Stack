import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { postJson } from '../api/client';
import { saveSession, type Session } from '../auth/session';
import { registrationError } from '../validation/member';

export function RegisterPage() {
  const navigate = useNavigate();
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    const validation = registrationError({ email, fullName, password });
    if (validation) {
      setError(validation);
      return;
    }
    try {
      const session = await postJson<Session>('/api/auth/register', { email, fullName, password });
      saveSession(session);
      void navigate('/');
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Registration failed.');
    }
  }

  return (
    <main>
      <h1>Join the club</h1>
      <form onSubmit={(event) => void onSubmit(event)}>
        <label>
          Full name
          <input value={fullName} onChange={(event) => setFullName(event.target.value)} required />
        </label>
        <label>
          Email
          <input value={email} onChange={(event) => setEmail(event.target.value)} type="email" required />
        </label>
        <label>
          Password
          <input value={password} onChange={(event) => setPassword(event.target.value)} type="password" required />
        </label>
        {error ? <p role="alert">{error}</p> : null}
        <button type="submit">Create account</button>
      </form>
      <p>
        Already a member? <Link to="/login">Sign in</Link>
      </p>
    </main>
  );
}
