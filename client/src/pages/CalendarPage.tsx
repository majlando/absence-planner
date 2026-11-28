import { Form, Link, redirect, useActionData, useLoaderData, type ActionFunctionArgs, type LoaderFunctionArgs } from 'react-router'
import { api, errorMessages } from '../api/index.ts'
import type { DayOffResponse } from '../api/ApiClient.ts'

// The year comes from the address, e.g. /calendar?year=2027. Default is this year.
export async function calendarLoader({ request }: LoaderFunctionArgs) {
  try {
    // Everyone may read the days off, but only admins may see this page.
    const me = await api.api.getApiAuthMe()
    if (me.data.role !== 'Admin') return redirect('/')

    const year = Number(new URL(request.url).searchParams.get('year')) || new Date().getFullYear()
    const response = await api.api.getApiCalendar(year)
    return { year, days: response.data }
  } catch {
    return redirect('/')
  }
}

// The page has two forms: one to add a company day and one (per row) to remove it.
export async function calendarAction({ request }: ActionFunctionArgs) {
  const form = await request.formData()
  try {
    if (form.get('intent') === 'remove') {
      await api.api.deleteApiCalendarCompanyDays(String(form.get('id')))
    } else {
      await api.api.postApiCalendarCompanyDays({ date: String(form.get('date')), name: String(form.get('name')) })
    }
  } catch (failed) {
    return { errors: await errorMessages(failed) }
  }
  return { errors: [] }
}

export function CalendarPage() {
  const { year, days } = useLoaderData() as { year: number; days: DayOffResponse[] }
  const result = useActionData() as { errors: string[] } | undefined

  return (
    <>
      <h1>Calendar {year}</h1>
      <Link to={`/calendar?year=${year - 1}`}>Previous year</Link> <Link to={`/calendar?year=${year + 1}`}>Next year</Link>
      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>Name</th>
            <th>Type</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {days.map((day) => (
            <tr key={day.date}>
              <td>{day.date}</td>
              <td>{day.name}</td>
              <td>{day.companyDayId ? 'Company day off' : 'Public holiday'}</td>
              <td>
                {day.companyDayId && (
                  <Form method="post">
                    <input type="hidden" name="id" value={day.companyDayId} />
                    <button type="submit" name="intent" value="remove" aria-label={`Remove ${day.name}`}>
                      Remove
                    </button>
                  </Form>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h2>Add company day off</h2>
      <Form method="post">
        <label>
          Date <input name="date" type="date" required />
        </label>
        <label>
          Name <input name="name" required />
        </label>
        <button type="submit" name="intent" value="add">
          Add
        </button>
      </Form>
      {result?.errors.map((error) => (
        <p role="alert" key={error}>
          {error}
        </p>
      ))}
    </>
  )
}
