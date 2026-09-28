import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, jest } from '@jest/globals';
import { LoginPage } from './LoginPage';
import { renderWithProviders } from '../test/renderWithProviders';

afterEach(() => {
  jest.restoreAllMocks();
});

describe('LoginPage', () => {
  it('shows field errors and does not call the API when the form is empty', async () => {
    const fetchMock = jest.fn();
    global.fetch = fetchMock as typeof fetch;
    const user = userEvent.setup();
    renderWithProviders(<LoginPage />, { route: '/login' });

    await user.click(screen.getByRole('button', { name: 'Sign in' }));

    expect(screen.getByText('Email is required.')).toBeInTheDocument();
    expect(screen.getByText('Password is required.')).toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();
  });

  it('rejects an ill-formed email before sending a request', async () => {
    const fetchMock = jest.fn();
    global.fetch = fetchMock as typeof fetch;
    const user = userEvent.setup();
    renderWithProviders(<LoginPage />, { route: '/login' });

    await user.type(screen.getByLabelText(/Email/), 'not-an-email');
    await user.type(screen.getByLabelText(/Password/), 'password12345');
    await user.click(screen.getByRole('button', { name: 'Sign in' }));

    expect(screen.getByText('Enter a valid email address.')).toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();
  });

  it('renders an API failure as an alert', async () => {
    global.fetch = jest.fn().mockResolvedValue(
      new Response(JSON.stringify({ detail: 'Email or password is incorrect.' }), { status: 400 }),
    ) as typeof fetch;
    const user = userEvent.setup();
    renderWithProviders(<LoginPage />, { route: '/login' });

    await user.type(screen.getByLabelText(/Email/), 'person@example.edu');
    await user.type(screen.getByLabelText(/Password/), 'password12345');
    await user.click(screen.getByRole('button', { name: 'Sign in' }));

    expect(await screen.findByRole('alert')).toHaveTextContent('Email or password is incorrect.');
  });
});
