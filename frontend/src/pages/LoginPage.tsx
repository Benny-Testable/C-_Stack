import { useState, type ReactNode } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { useSubmit } from '../hooks/useSubmit';
import { ErrorMessage } from '../components/Feedback';
import { TextField } from '../components/FormField';
import { fieldLimits, isBlank, validateEmail } from '../validation/validators';

/**
 * Sign-in form.
 *
 * The password is only checked for presence, not against the strength policy: the policy applies when
 * a password is chosen, and applying it here would tell an existing user their correct password is
 * invalid.
 *
 * After signing in the user is returned to the route they were trying to reach, which `RequireAuth`
 * recorded in router state.
 */
export function LoginPage(): ReactNode {
  const { signIn } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [emailError, setEmailError] = useState<string | undefined>(undefined);
  const [passwordError, setPasswordError] = useState<string | undefined>(undefined);

  const { state, submit } = useSubmit(signIn);

  const returnTo = (location.state as { from?: string } | null)?.from ?? '/applications';

  const handleSubmit = async (): Promise<void> => {
    const nextEmailError = validateEmail(email);
    const nextPasswordError = isBlank(password) ? 'Password is required.' : undefined;

    setEmailError(nextEmailError);
    setPasswordError(nextPasswordError);

    if (nextEmailError !== undefined || nextPasswordError !== undefined) {
      return;
    }

    // `submit` resolves to null when the request failed; the error is already in `state.error`.
    const outcome = await submit(email.trim(), password);
    if (outcome !== null) {
      await navigate(returnTo, { replace: true });
    }
  };

  return (
    <section className="page page--narrow">
      <header className="page__header">
        <h1>Sign in</h1>
      </header>

      <form
        className="form"
        onSubmit={(event) => {
          event.preventDefault();
          void handleSubmit();
        }}
        noValidate
      >
        <TextField
          id="login-email"
          label="Email"
          type="email"
          required
          autoComplete="username"
          maxLength={fieldLimits.emailMax}
          value={email}
          onChange={setEmail}
          {...(emailError !== undefined ? { error: emailError } : {})}
        />

        <TextField
          id="login-password"
          label="Password"
          type="password"
          required
          autoComplete="current-password"
          maxLength={fieldLimits.passwordMax}
          value={password}
          onChange={setPassword}
          {...(passwordError !== undefined ? { error: passwordError } : {})}
        />

        {state.error !== null && <ErrorMessage message={state.error} />}

        <button type="submit" className="button button--primary" disabled={state.isSubmitting}>
          {state.isSubmitting ? 'Signing in…' : 'Sign in'}
        </button>
      </form>

      <p className="page__aside">
        No account yet? <Link to="/register">Register as an applicant</Link>.
      </p>
    </section>
  );
}
