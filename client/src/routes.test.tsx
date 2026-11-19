import { render, screen } from '@testing-library/react'
import { createMemoryRouter, RouterProvider } from 'react-router'
import { expect, test } from 'vitest'
import { routes } from './routes.tsx'

test('renders home page', () => {
  render(<RouterProvider router={createMemoryRouter(routes)} />)
  expect(screen.getByRole('heading', { name: 'Absence Planner' })).toBeDefined()
})
