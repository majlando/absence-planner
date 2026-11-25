import { Form, redirect, useActionData, useLoaderData, type ActionFunctionArgs, type LoaderFunctionArgs } from 'react-router'
import { api, errorMessages } from '../api/index.ts'
import type { UserResponse } from '../api/ApiClient.ts'

// The same page is used to create a user (/users/new) and to edit one (/users/:id).

export async function userFormLoader({ params }: LoaderFunctionArgs) {
  if (!params.id) return null // a new user
  try {
    const response = await api.api.getApiUsers(params.id)
    return response.data
  } catch {
    return redirect('/users')
  }
}

export async function userFormAction({ params, request }: ActionFunctionArgs) {
  const form = await request.formData()
  const email = String(form.get('email'))
  const role = String(form.get('role'))
  try {
    if (params.id) {
      await api.api.putApiUsers(params.id, { email, role, active: form.get('active') === 'on' })
    } else {
      await api.api.postApiUsers({ email, role, password: String(form.get('password')) })
    }
  } catch (failed) {
    return { errors: await errorMessages(failed) }
  }
  return redirect('/users')
}

export function UserFormPage() {
  const user = useLoaderData() as UserResponse | null
  const result = useActionData() as { errors: string[] } | undefined

  return (
    <>
      <h1>{user ? 'Edit user' : 'New user'}</h1>
      <Form method="post">
        <label>
          Email <input name="email" type="email" defaultValue={user?.email} required />
        </label>
        {!user && (
          <label>
            Password <input name="password" type="password" minLength={8} required />
          </label>
        )}
        <label>
          Role{' '}
          <select name="role" defaultValue={user?.role ?? 'Employee'}>
            <option>Employee</option>
            <option>Accounting</option>
            <option>Admin</option>
          </select>
        </label>
        {user && (
          <label>
            <input name="active" type="checkbox" defaultChecked={user.active} /> Active
          </label>
        )}
        <button type="submit">Save</button>
      </Form>
      {result?.errors.map((error) => (
        <p role="alert" key={error}>
          {error}
        </p>
      ))}
    </>
  )
}
