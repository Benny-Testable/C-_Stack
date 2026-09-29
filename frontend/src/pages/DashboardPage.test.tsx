import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { DashboardPage } from './DashboardPage';

describe('DashboardPage', () => {
  it('names the club', () => {
    render(
      <MemoryRouter>
        <DashboardPage />
      </MemoryRouter>,
    );
    expect(screen.getByRole('heading', { name: /West Coast Fitness Club/ })).toBeInTheDocument();
  });
});
