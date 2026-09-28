import { useState, type ReactNode } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { ErrorMessage } from '../components/Feedback';
import { TextField } from '../components/FormField';
import { useSubmit } from '../hooks/useSubmit';
import {
  fieldLimits,
  hasErrors,
  validateEmail,
  validateOptionalText,
  validatePassword,
  validateRequiredText,
  type ValidationErrors,
} from '../validation/validators';

interface RegisterForm {
  fullName: string;
  email: string;
  password: string;
  institutionName: string;
}

type RegisterErrors = ValidationErrors<RegisterForm>;

function validateRegisterForm(values: RegisterForm): RegisterErrors {
  const errors: RegisterErrors = {};
  const fullName = validateRequiredText(values.fullName, 'Full name', { min: 2, max: fieldLimits.nameMax });
  const email = validateEmail(values.email);
  const password = validatePassword(values.password);
  const institution = validateOptionalText(
    values.institutionName,
    'Institution',
    fieldLimits.nameMax,
  );

  if (fullName !== undefined) {
    errors.fullName = fullName;
  }
  if (email !== undefined) {
    errors.email = email;
  }
  if (password !== undefined) {
    errors.password = password;
  }
  if (institution !== undefined) {
    errors.institutionName = institution;
  }

  return errors;
}

/**
 * Applicant self-registration.
 *
 * Creates both the applicant profile and the login in one request. Administrator accounts are not
 * created here: they are provisioned from environment configuration at API start-up.
 */
export function RegisterPage(): ReactNode {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [values, setValues] = useState<RegisterForm>({
    fullName: '',
    email: '',
    password: '',
    institutionName: '',
  });
  const [errors, setErrors] = useState<RegisterErrors>({});
  const { state, submit } = useSubmit(register);

  const handleSubmit = async (): Promise<void> => {
    const nextErrors = validateRegisterForm(values);
    setErrors(nextErrors);

    if (hasErrors(nextErrors)) {
      return;
    }

    const institution = values.institutionName.trim();
    const outcome = await submit({
      fullName: values.fullName.trim(),
      email: values.email.trim(),
      password: values.password,
      institutionName: institution.length > 0 ? institution : null,
    });

    if (outcome !== null) {
      await navigate('/scholarships', { replace: true });
    }
  };

  return (
    <section className="page page--narrow">
      <header className="page__header">
        <h1>Register as an applicant</h1>
        <p className="page__lead">Create an account to start a scholarship application.</p>
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
          id="register-full-name"
          label="Full name"
          required
          autoComplete="name"
          maxLength={fieldLimits.nameMax}
          value={values.fullName}
          onChange={(fullName) => {
            setValues((current) => ({ ...current, fullName }));
          }}
          {...(errors.fullName !== undefined ? { error: errors.fullName } : {})}
        />

        <TextField
          id="register-email"
          label="Email"
          type="email"
          required
          autoComplete="username"
          maxLength={fieldLimits.emailMax}
          value={values.email}
          onChange={(email) => {
            setValues((current) => ({ ...current, email }));
          }}
          {...(errors.email !== undefined ? { error: errors.email } : {})}
        />

        <TextField
          id="register-password"
          label="Password"
          type="password"
          required
          autoComplete="new-password"
          hint={`At least ${String(fieldLimits.passwordMin)} characters.`}
          maxLength={fieldLimits.passwordMax}
          value={values.password}
          onChange={(password) => {
            setValues((current) => ({ ...current, password }));
          }}
          {...(errors.password !== undefined ? { error: errors.password } : {})}
        />

        <TextField
          id="register-institution"
          label="Institution"
          hint="Optional."
          autoComplete="organization"
          maxLength={fieldLimits.nameMax}
          value={values.institutionName}
          onChange={(institutionName) => {
            setValues((current) => ({ ...current, institutionName }));
          }}
          {...(errors.institutionName !== undefined ? { error: errors.institutionName } : {})}
        />

        {state.error !== null && <ErrorMessage message={state.error} />}

        <button type="submit" className="button button--primary" disabled={state.isSubmitting}>
          {state.isSubmitting ? 'Creating account…' : 'Create account'}
        </button>
      </form>

      <p className="page__aside">
        Already registered? <Link to="/login">Sign in</Link>.
      </p>
    </section>
  );
}
