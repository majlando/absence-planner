import { expect, test } from '@playwright/test'

test('admin creates a user, who logs in and changes password', async ({ page }) => {
  // A new email every run, because emails must be unique.
  const email = `user-${Date.now()}@example.com`

  // The admin creates the user.
  await page.goto('/login')
  await page.getByLabel('Email').fill(process.env.ADMIN_EMAIL!)
  await page.getByLabel('Password').fill(process.env.ADMIN_PASSWORD!)
  await page.getByRole('button', { name: 'Log in' }).click()
  await page.getByRole('link', { name: 'Users' }).click()
  await page.getByRole('link', { name: 'New user' }).click()
  await page.getByLabel('Email').fill(email)
  await page.getByLabel('Password').fill('first-password')
  await page.getByRole('button', { name: 'Save' }).click()
  await expect(page.getByRole('link', { name: email })).toBeVisible()
  await page.getByRole('button', { name: 'Log out' }).click()

  // The new user logs in and changes password. Employees don't see the Users page.
  await page.getByLabel('Email').fill(email)
  await page.getByLabel('Password').fill('first-password')
  await page.getByRole('button', { name: 'Log in' }).click()
  await expect(page.getByRole('link', { name: 'Users' })).toHaveCount(0)
  await page.getByRole('link', { name: 'Change password' }).click()
  await page.getByLabel('Old password').fill('first-password')
  await page.getByLabel('New password').fill('second-password')
  await page.getByRole('button', { name: 'Save' }).click()
  await expect(page.getByRole('status')).toHaveText('Your password is changed')
})
