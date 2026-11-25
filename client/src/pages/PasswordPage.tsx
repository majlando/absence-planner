import { Form, useActionData, type ActionFunctionArgs } from 'react-router'
import { api, errorMessages } from '../api/index.ts'

export async function passwordAction({ request }: ActionFunctionArgs) {
  const form = await request.formData()
  try {
    await api.api.postApiAuthPassword({
      oldPassword: String(form.get('oldPassword')),
      newPassword: String(form.get('newPassword')),
    })
  } catch (failed) {
    return { errors: await errorMessages(failed) }
  }
  return { errors: [], done: true }
}

export function PasswordPage() {
  const result = useActionData() as { errors: string[]; done?: boolean } | undefined

  return (
    <>
      <h1>Change password</h1>
      <Form method="post">
        <label>
          Old password <input name="oldPassword" type="password" required />
        </label>
        <label>
          New password <input name="newPassword" type="password" minLength={8} required />
        </label>
        <button type="submit">Save</button>
      </Form>
      {result?.done && <p role="status">Your password is changed</p>}
      {result?.errors.map((error) => (
        <p role="alert" key={error}>
          {error}
        </p>
      ))}
    </>
  )
}
