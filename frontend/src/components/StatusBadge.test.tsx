import { render, screen } from '@testing-library/react';
import { describe, expect, it } from '@jest/globals';
import { StatusBadge } from './StatusBadge';
import { statusLabel } from './statusLabel';

describe('StatusBadge', () => {
  it('renders a human-readable label without relying on colour alone', () => {
    render(<StatusBadge status="UnderReview" />);

    expect(screen.getByTestId('status-badge')).toHaveTextContent('Under review');
    expect(statusLabel('UnderReview')).toBe('Under review');
  });
});
