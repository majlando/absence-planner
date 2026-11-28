// The pages in the menu, and which roles may see them.
export const menu = [
  { to: '/', label: 'Home', roles: ['Employee', 'Accounting', 'Admin'] },
  { to: '/users', label: 'Users', roles: ['Admin'] },
  { to: '/rules', label: 'Rules', roles: ['Admin'] },
  { to: '/calendar', label: 'Calendar', roles: ['Admin'] },
  { to: '/password', label: 'Change password', roles: ['Employee', 'Accounting', 'Admin'] },
]

export function menuFor(role: string) {
  return menu.filter((item) => item.roles.includes(role))
}
