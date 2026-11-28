import type { RouteObject } from 'react-router'
import { CalendarPage, calendarAction, calendarLoader } from './pages/CalendarPage.tsx'
import { HomePage } from './pages/HomePage.tsx'
import { Layout, layoutLoader, logoutAction } from './pages/Layout.tsx'
import { LoginPage, loginAction } from './pages/LoginPage.tsx'
import { PasswordPage, passwordAction } from './pages/PasswordPage.tsx'
import { RulesPage, rulesLoader } from './pages/RulesPage.tsx'
import { UserFormPage, userFormAction, userFormLoader } from './pages/UserFormPage.tsx'
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
      { path: 'users/new', element: <UserFormPage />, loader: userFormLoader, action: userFormAction },
      { path: 'users/:id', element: <UserFormPage />, loader: userFormLoader, action: userFormAction },
      { path: 'rules', element: <RulesPage />, loader: rulesLoader },
      { path: 'calendar', element: <CalendarPage />, loader: calendarLoader, action: calendarAction },
      { path: 'password', element: <PasswordPage />, action: passwordAction },
    ],
  },
]
