import { Form, NavLink, Outlet, redirect, useLoaderData } from 'react-router'
import { api } from '../api/index.ts'
import type { MeResponse } from '../api/ApiClient.ts'
import { menuFor } from '../menu.ts'

// Runs before every page inside the layout. Not logged in? Then go to the login page.
export async function layoutLoader() {
  try {
    const response = await api.api.getApiAuthMe()
    return response.data
  } catch {
    return redirect('/login')
  }
}

export async function logoutAction() {
  await api.api.postApiAuthLogout()
  return redirect('/login')
}

export function Layout() {
  const me = useLoaderData() as MeResponse

  return (
    <>
      <nav>
        {menuFor(me.role).map((item) => (
          <NavLink key={item.to} to={item.to}>
            {item.label}
          </NavLink>
        ))}
        <span>{me.email}</span>
        <Form method="post" action="/logout">
          <button type="submit">Log out</button>
        </Form>
      </nav>
      <main>
        <Outlet />
      </main>
    </>
  )
}
