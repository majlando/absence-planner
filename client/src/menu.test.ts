import { expect, test } from 'vitest'
import { menuFor } from './menu.ts'

test('admin sees the admin pages', () => {
  expect(menuFor('Admin').map((item) => item.label)).toEqual(['Home', 'Users', 'Rules', 'Calendar', 'Change password'])
})

test('employee does not see the admin pages', () => {
  expect(menuFor('Employee').map((item) => item.label)).toEqual(['Home', 'Change password'])
})
