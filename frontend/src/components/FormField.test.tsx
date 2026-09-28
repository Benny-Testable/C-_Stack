import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { TextField } from './FormField';

describe('TextField', () => {
  it('binds the label, marks the control invalid, and exposes the error to assistive technology', async () => {
    const onChange = vi.fn();
    const user = userEvent.setup();

    render(
      <TextField
        id="login-email"
        label="Email"
        required
        value=""
        error="Email is required."
        onChange={onChange}
      />,
    );

    const input = screen.getByLabelText(/Email/);
    expect(input).toHaveAttribute('aria-invalid', 'true');
    expect(input).toHaveAccessibleDescription('Email is required.');

    await user.type(input, 'a');
    expect(onChange).toHaveBeenCalledWith('a');
  });
});
