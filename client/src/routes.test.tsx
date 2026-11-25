import { cleanup, fireEvent, render, screen } from '@testing-library/react'
import { createMemoryRouter, RouterProvider } from 'react-router'
import { afterEach, expect, test, vi } from 'vitest'
import { routes } from './routes.tsx'

// Every call to the API answers 401 (not logged in / wrong password).
function apiAnswers401() {
  vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(null, { status: 401 }))
}

afterEach(() => {
  cleanup()
  vi.restoreAllMocks()
})

test('not logged in is sent to the login page', async () => {
  apiAnswers401()

  render(<RouterProvider router={createMemoryRouter(routes)} />)

  expect(await screen.findByRole('heading', { name: 'Log in' })).toBeDefined()
})

test('wrong password shows an error', async () => {
  apiAnswers401()
  render(<RouterProvider router={createMemoryRouter(routes, { initialEntries: ['/login'] })} />)

  fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'anna@example.com' } })
  fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'wrong' } })
  fireEvent.click(screen.getByRole('button', { name: 'Log in' }))

  expect((await screen.findByRole('alert')).textContent).toBe('Wrong email or password')
})
