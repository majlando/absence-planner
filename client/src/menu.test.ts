import { expect, test } from 'vitest'
import { menuFor } from './menu.ts'

test('admin sees the users page', () => {
  expect(menuFor('Admin').map((item) => item.label)).toEqual(['Home', 'Users', 'Change password'])
})

test('employee does not see the users page', () => {
  expect(menuFor('Employee').map((item) => item.label)).toEqual(['Home', 'Change password'])
})
