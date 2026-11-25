import type { RouteObject } from 'react-router'
import { HomePage } from './pages/HomePage.tsx'
import { Layout, layoutLoader, logoutAction } from './pages/Layout.tsx'
import { LoginPage, loginAction } from './pages/LoginPage.tsx'
import { UsersPage, usersLoader } from './pages/UsersPage.tsx'

export const routes: RouteObject[] = [
  { path: '/login', element: <LoginPage />, action: loginAction },
  { path: '/logout', action: logoutAction },
  {
    // Pages that need login.
    path: '/',
    element: <Layout />,
    loader: layoutLoader,
    children: [
      { index: true, element: <HomePage /> },
      { path: 'users', element: <UsersPage />, loader: usersLoader },
    ],
  },
]
