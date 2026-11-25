import { Link, redirect, useLoaderData } from 'react-router'
import { api } from '../api/index.ts'
import type { UserResponse } from '../api/ApiClient.ts'

// Only admins may see this page. Others are sent to the home page.
export async function usersLoader() {
  try {
    const response = await api.api.getApiUsersAll()
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
      <Link to="/users/new">New user</Link>
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
              <td>
                <Link to={`/users/${user.id}`}>{user.email}</Link>
              </td>
              <td>{user.role}</td>
              <td>{user.active ? 'Yes' : 'No'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  )
}
