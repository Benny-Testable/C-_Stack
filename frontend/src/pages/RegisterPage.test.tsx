import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, jest } from '@jest/globals';
import { RegisterPage } from './RegisterPage';
import { renderWithProviders } from '../test/renderWithProviders';

afterEach(() => {
  jest.restoreAllMocks();
});

describe('RegisterPage', () => {
  it('blocks a short password without calling the API', async () => {
    const fetchMock = jest.fn();
    global.fetch = fetchMock as typeof fetch;
    const user = userEvent.setup();
    renderWithProviders(<RegisterPage />, { route: '/register' });

    await user.type(screen.getByLabelText(/Full name/), 'Ada Lovelace');
    await user.type(screen.getByLabelText(/Email/), 'ada@example.edu');
    await user.type(screen.getByLabelText(/^Password/), 'short');
    await user.click(screen.getByRole('button', { name: 'Create account' }));

    expect(screen.getByText('Password must be at least 12 characters.')).toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();
  });
});
