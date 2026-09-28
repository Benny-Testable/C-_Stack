import type { ReactNode } from 'react';

/**
 * A labelled input with its validation message.
 *
 * The label is bound to the control by id, and the message is referenced through `aria-describedby`
 * with `aria-invalid`, so assistive technology reports which field failed and why. Doing this in one
 * component is what keeps it consistent across every form.
 */
interface FieldShellProps {
  readonly id: string;
  readonly label: string;
  readonly error: string | undefined;
  readonly hint?: string;
  readonly required?: boolean;
  readonly children: (attributes: {
    readonly id: string;
    readonly 'aria-invalid': boolean;
    readonly 'aria-describedby': string | undefined;
  }) => ReactNode;
}

function FieldShell({ id, label, error, hint, required, children }: FieldShellProps): ReactNode {
  const errorId = `${id}-error`;
  const hintId = `${id}-hint`;
  const describedBy = [hint !== undefined ? hintId : null, error !== undefined ? errorId : null]
    .filter((value): value is string => value !== null)
    .join(' ');

  return (
    <div className="field">
      <label className="field__label" htmlFor={id}>
        {label}
        {required === true && (
          <span className="field__required" aria-hidden="true">
            {' *'}
          </span>
        )}
      </label>

      {hint !== undefined && (
        <p className="field__hint" id={hintId}>
          {hint}
        </p>
      )}

      {children({
        id,
        'aria-invalid': error !== undefined,
        'aria-describedby': describedBy.length > 0 ? describedBy : undefined,
      })}

      {error !== undefined && (
        <p className="field__error" id={errorId}>
          {error}
        </p>
      )}
    </div>
  );
}

interface TextFieldProps {
  readonly id: string;
  readonly label: string;
  readonly value: string;
  readonly onChange: (value: string) => void;
  readonly error?: string;
  readonly hint?: string;
  readonly type?: 'text' | 'email' | 'password' | 'date' | 'number';
  readonly required?: boolean;
  readonly maxLength?: number;
  readonly autoComplete?: string;
  readonly min?: string;
  readonly step?: string;
}

export function TextField({
  id,
  label,
  value,
  onChange,
  error,
  hint,
  type = 'text',
  required,
  maxLength,
  autoComplete,
  min,
  step,
}: TextFieldProps): ReactNode {
  return (
    <FieldShell id={id} label={label} error={error} {...(hint !== undefined ? { hint } : {})} {...(required !== undefined ? { required } : {})}>
      {(attributes) => (
        <input
          {...attributes}
          className="field__input"
          type={type}
          value={value}
          onChange={(event) => {
            onChange(event.target.value);
          }}
          {...(maxLength !== undefined ? { maxLength } : {})}
          {...(autoComplete !== undefined ? { autoComplete } : {})}
          {...(min !== undefined ? { min } : {})}
          {...(step !== undefined ? { step } : {})}
        />
      )}
    </FieldShell>
  );
}

interface TextAreaFieldProps {
  readonly id: string;
  readonly label: string;
  readonly value: string;
  readonly onChange: (value: string) => void;
  readonly error?: string;
  readonly hint?: string;
  readonly rows?: number;
  readonly maxLength?: number;
}

export function TextAreaField({
  id,
  label,
  value,
  onChange,
  error,
  hint,
  rows = 5,
  maxLength,
}: TextAreaFieldProps): ReactNode {
  return (
    <FieldShell id={id} label={label} error={error} {...(hint !== undefined ? { hint } : {})}>
      {(attributes) => (
        <textarea
          {...attributes}
          className="field__input field__input--textarea"
          rows={rows}
          value={value}
          onChange={(event) => {
            onChange(event.target.value);
          }}
          {...(maxLength !== undefined ? { maxLength } : {})}
        />
      )}
    </FieldShell>
  );
}

interface CheckboxFieldProps {
  readonly id: string;
  readonly label: string;
  readonly checked: boolean;
  readonly onChange: (checked: boolean) => void;
}

export function CheckboxField({ id, label, checked, onChange }: CheckboxFieldProps): ReactNode {
  return (
    <div className="field field--checkbox">
      <input
        id={id}
        className="field__checkbox"
        type="checkbox"
        checked={checked}
        onChange={(event) => {
          onChange(event.target.checked);
        }}
      />
      <label className="field__label field__label--inline" htmlFor={id}>
        {label}
      </label>
    </div>
  );
}
