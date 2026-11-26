import { cleanup, fireEvent, render, screen } from '@testing-library/react'
import { createMemoryRouter, RouterProvider } from 'react-router'
import { afterEach, expect, test, vi } from 'vitest'
import { routes } from './routes.tsx'

// Every call to the API answers with this status, e.g. 401 (not logged in / wrong password).
function apiAnswers(status: number) {
  vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(null, { status }))
}

function logIn() {
  render(<RouterProvider router={createMemoryRouter(routes, { initialEntries: ['/login'] })} />)
  fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'anna@example.com' } })
  fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'wrong' } })
  fireEvent.click(screen.getByRole('button', { name: 'Log in' }))
}

afterEach(() => {
  cleanup()
  vi.restoreAllMocks()
})

test('not logged in is sent to the login page', async () => {
  apiAnswers(401)

  render(<RouterProvider router={createMemoryRouter(routes)} />)

  expect(await screen.findByRole('heading', { name: 'Log in' })).toBeDefined()
})

test('wrong password shows an error', async () => {
  apiAnswers(401)

  logIn()

  expect((await screen.findByRole('alert')).textContent).toBe('Wrong email or password')
})

test('server error during login shows a general error', async () => {
  apiAnswers(500)

  logIn()

  expect((await screen.findByRole('alert')).textContent).toBe('Something went wrong')
})
