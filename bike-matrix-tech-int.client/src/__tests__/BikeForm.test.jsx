import { render, screen, fireEvent } from '@testing-library/react';
import { describe, test, expect, vi, beforeEach } from 'vitest';
import BikeForm from '../components/bike/BikeForm';

beforeEach(() => {
  global.fetch = vi.fn(() =>
    Promise.resolve({
      ok: true
    })
  );
});

describe('BikeForm', () => {
  test('Submit create bike form', async () => {
    render(<BikeForm />);

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.change(screen.getByLabelText(/brand/i), {
      target: { value: 'Canyon' }
    });
    fireEvent.change(screen.getByLabelText(/model/i), {
      target: { value: 'Dude' }
    });
    fireEvent.change(screen.getByLabelText(/year/i), {
      target: { value: '2020' }
    });

    fireEvent.click(screen.getByRole('button', { name: /create bike/i }));

    expect(await screen.findByText(/bike created/i)).toBeInTheDocument();
  });
});