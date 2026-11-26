import { Form, redirect, useActionData, type ActionFunctionArgs } from 'react-router'
import { api } from '../api/index.ts'

// Runs when the form is sent.
export async function loginAction({ request }: ActionFunctionArgs) {
  const form = await request.formData()
  try {
    await api.api.postApiAuthLogin({
      email: String(form.get('email')),
      password: String(form.get('password')),
    })
  } catch (failed) {
    const wrongLogin = failed instanceof Response && failed.status === 401
    return { error: wrongLogin ? 'Wrong email or password' : 'Something went wrong' }
  }
  return redirect('/')
}

export function LoginPage() {
  const result = useActionData() as { error: string } | undefined

  return (
    <main>
      <h1>Log in</h1>
      <Form method="post">
        <label>
          Email <input name="email" type="email" required />
        </label>
        <label>
          Password <input name="password" type="password" required />
        </label>
        <button type="submit">Log in</button>
      </Form>
      {result && <p role="alert">{result.error}</p>}
    </main>
  )
}
