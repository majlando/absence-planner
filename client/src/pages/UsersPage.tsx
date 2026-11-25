import { redirect, useLoaderData } from 'react-router'
import { api } from '../api/index.ts'
import type { UserResponse } from '../api/ApiClient.ts'

// Only admins may see this page. Others are sent to the home page.
export async function usersLoader() {
  try {
    const response = await api.api.getApiUsers()
    return response.data
  } catch {
    return redirect('/')
  }
}

export function UsersPage() {
  const users = useLoaderData() as UserResponse[]

  return (
    <>
      <h1>Users</h1>
      <table>
        <thead>
          <tr>
            <th>Email</th>
            <th>Role</th>
            <th>Active</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.email}</td>
              <td>{user.role}</td>
              <td>{user.active ? 'Yes' : 'No'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  )
}
