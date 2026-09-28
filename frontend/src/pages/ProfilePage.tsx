import { useState, type ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { useAsyncData } from '../hooks/useAsyncData';
import { useSubmit } from '../hooks/useSubmit';
import { ErrorMessage, LoadingIndicator, SuccessMessage } from '../components/Feedback';
import { TextField } from '../components/FormField';
import {
  fieldLimits,
  hasErrors,
  validateEmail,
  validateOptionalText,
  validateRequiredText,
  type ValidationErrors,
} from '../validation/validators';
import type { Applicant } from '../api/types';

interface ProfileForm {
  fullName: string;
  email: string;
  institutionName: string;
}

/**
 * Applicant profile, including account erasure.
 *
 * Erasure is irreversible and removes the applicant plus every application that referenced them.
 * The confirmation field must match the word DELETE so a mis-click cannot fire the request.
 */
export function ProfilePage(): ReactNode {
  const { api, applicantId, signOut } = useAuth();
  const navigate = useNavigate();

  const { state, reload } = useAsyncData<Applicant>(
    (signal) => {
      if (applicantId === null) {
        return Promise.reject(new Error('This account does not have an applicant profile.'));
      }
      return api.applicants.getById(applicantId, { signal });
    },
    [api, applicantId],
  );

  if (state.status === 'loading') {
    return <LoadingIndicator label="Loading profile…" />;
  }

  if (state.status === 'error') {
    return <ErrorMessage message={state.error} onRetry={reload} />;
  }

  return (
    <ProfileFormView
      applicant={state.data}
      onSaved={reload}
      onErased={() => {
        signOut();
        void navigate('/scholarships', { replace: true });
      }}
    />
  );
}

function collectProfileErrors(values: ProfileForm): ValidationErrors<ProfileForm> {
  const errors: ValidationErrors<ProfileForm> = {};
  const fullName = validateRequiredText(values.fullName, 'Full name', { min: 2, max: fieldLimits.nameMax });
  const email = validateEmail(values.email);
  const institution = validateOptionalText(values.institutionName, 'Institution', fieldLimits.nameMax);

  if (fullName !== undefined) {
    errors.fullName = fullName;
  }
  if (email !== undefined) {
    errors.email = email;
  }
  if (institution !== undefined) {
    errors.institutionName = institution;
  }

  return errors;
}

function ProfileFormView({
  applicant,
  onSaved,
  onErased,
}: {
  readonly applicant: Applicant;
  readonly onSaved: () => void;
  readonly onErased: () => void;
}): ReactNode {
  const { api } = useAuth();
  const [values, setValues] = useState<ProfileForm>({
    fullName: applicant.fullName,
    email: applicant.email,
    institutionName: applicant.institutionName ?? '',
  });
  const [errors, setErrors] = useState<ValidationErrors<ProfileForm>>({});
  const [saved, setSaved] = useState(false);
  const { state: saveState, submit: save } = useSubmit(api.applicants.update);

  const persist = async (): Promise<void> => {
    const nextErrors = collectProfileErrors(values);
    setErrors(nextErrors);
    if (hasErrors(nextErrors)) {
      return;
    }

    const institutionName = values.institutionName.trim();
    const updated = await save(applicant.id, {
      fullName: values.fullName.trim(),
      email: values.email.trim(),
      institutionName: institutionName.length > 0 ? institutionName : null,
    });

    if (updated !== null) {
      setSaved(true);
      onSaved();
    }
  };

  return (
    <section className="page page--narrow">
      <header className="page__header">
        <h1>Profile</h1>
        <p className="page__lead">The name and email used on your applications.</p>
      </header>

      <form
        className="form"
        onSubmit={(event) => {
          event.preventDefault();
          void persist();
        }}
        noValidate
      >
        <TextField
          id="profile-full-name"
          label="Full name"
          required
          autoComplete="name"
          maxLength={fieldLimits.nameMax}
          value={values.fullName}
          onChange={(fullName) => {
            setValues((current) => ({ ...current, fullName }));
            setSaved(false);
          }}
          {...(errors.fullName !== undefined ? { error: errors.fullName } : {})}
        />
        <TextField
          id="profile-email"
          label="Email"
          type="email"
          required
          autoComplete="email"
          maxLength={fieldLimits.emailMax}
          value={values.email}
          onChange={(email) => {
            setValues((current) => ({ ...current, email }));
            setSaved(false);
          }}
          {...(errors.email !== undefined ? { error: errors.email } : {})}
        />
        <TextField
          id="profile-institution"
          label="Institution"
          hint="Optional."
          autoComplete="organization"
          maxLength={fieldLimits.nameMax}
          value={values.institutionName}
          onChange={(institutionName) => {
            setValues((current) => ({ ...current, institutionName }));
            setSaved(false);
          }}
          {...(errors.institutionName !== undefined ? { error: errors.institutionName } : {})}
        />

        {saveState.error !== null && <ErrorMessage message={saveState.error} />}
        {saved && <SuccessMessage message="Profile saved." />}

        <button type="submit" className="button button--primary" disabled={saveState.isSubmitting}>
          {saveState.isSubmitting ? 'Saving…' : 'Save profile'}
        </button>
      </form>

      <ErasePanel applicantId={applicant.id} onErased={onErased} />
    </section>
  );
}

function ErasePanel({
  applicantId,
  onErased,
}: {
  readonly applicantId: number;
  readonly onErased: () => void;
}): ReactNode {
  const { api } = useAuth();
  const [confirmation, setConfirmation] = useState('');
  const { state: eraseState, submit: erase } = useSubmit(api.applicants.erase);

  const eraseAccount = async (): Promise<void> => {
    if (confirmation.trim() !== 'DELETE') {
      return;
    }

    const receipt = await erase(applicantId);
    if (receipt !== null) {
      onErased();
    }
  };

  return (
    <section className="panel panel--danger">
      <h2>Erase my data</h2>
      <p>
        This permanently deletes your profile and every application that belongs to it. Type{' '}
        <strong>DELETE</strong> to confirm.
      </p>
      <form
        className="form"
        onSubmit={(event) => {
          event.preventDefault();
          void eraseAccount();
        }}
      >
        <TextField
          id="profile-erase-confirm"
          label="Confirmation"
          value={confirmation}
          onChange={setConfirmation}
        />
        {eraseState.error !== null && <ErrorMessage message={eraseState.error} />}
        <button
          type="submit"
          className="button button--danger"
          disabled={eraseState.isSubmitting || confirmation.trim() !== 'DELETE'}
        >
          {eraseState.isSubmitting ? 'Erasing…' : 'Erase my data'}
        </button>
      </form>
    </section>
  );
}
