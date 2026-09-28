import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { EmptyState, ErrorMessage, LoadingIndicator, SuccessMessage } from './Feedback';

describe('feedback states', () => {
  it('announces loading through a live region', () => {
    render(<LoadingIndicator label="Loading scholarships…" />);

    expect(screen.getByRole('status')).toHaveTextContent('Loading scholarships…');
  });

  it('announces an error as an alert and retries when asked', async () => {
    const onRetry = vi.fn();
    const user = userEvent.setup();
    render(<ErrorMessage message="The catalogue could not be loaded." onRetry={onRetry} />);

    expect(screen.getByRole('alert')).toHaveTextContent('The catalogue could not be loaded.');
    await user.click(screen.getByRole('button', { name: 'Try again' }));
    expect(onRetry).toHaveBeenCalledTimes(1);
  });

  it('renders empty and success copy without a retry control', () => {
    render(
      <>
        <EmptyState message="No scholarships match the current filters." />
        <SuccessMessage message="Draft saved." />
      </>,
    );

    expect(screen.getByText('No scholarships match the current filters.')).toBeInTheDocument();
    expect(screen.getByText('Draft saved.')).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Try again' })).not.toBeInTheDocument();
  });
});
