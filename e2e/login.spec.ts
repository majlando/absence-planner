import { expect, test } from '@playwright/test'

test('log in and log out', async ({ page }) => {
  // Not logged in: sent to the login page.
  await page.goto('/')
  await expect(page.getByRole('heading', { name: 'Log in' })).toBeVisible()

  await page.getByLabel('Email').fill(process.env.ADMIN_EMAIL!)
  await page.getByLabel('Password').fill(process.env.ADMIN_PASSWORD!)
  await page.getByRole('button', { name: 'Log in' }).click()

  // Logged in as admin: the home page and the Users menu item.
  await expect(page.getByRole('heading', { name: 'Absence Planner' })).toBeVisible()
  await expect(page.getByRole('link', { name: 'Users' })).toBeVisible()

  await page.getByRole('button', { name: 'Log out' }).click()
  await expect(page.getByRole('heading', { name: 'Log in' })).toBeVisible()
})

test('wrong password shows an error', async ({ page }) => {
  await page.goto('/login')

  await page.getByLabel('Email').fill(process.env.ADMIN_EMAIL!)
  await page.getByLabel('Password').fill('wrong-password')
  await page.getByRole('button', { name: 'Log in' }).click()

  await expect(page.getByRole('alert')).toHaveText('Wrong email or password')
})
