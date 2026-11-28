import { expect, test } from '@playwright/test'

test('admin sees the rules and adds and removes a company day off', async ({ page }) => {
  await page.goto('/login')
  await page.getByLabel('Email').fill(process.env.ADMIN_EMAIL!)
  await page.getByLabel('Password').fill(process.env.ADMIN_PASSWORD!)
  await page.getByRole('button', { name: 'Log in' }).click()

  // The rules page shows the seeded rules.
  await page.getByRole('link', { name: 'Rules' }).click()
  await expect(page.getByRole('cell', { name: 'HolidayPayPercent' })).toBeVisible()

  // A year far away, so the test doesn't clash with real company days.
  await page.goto('/calendar?year=2099')
  await expect(page.getByRole('cell', { name: 'Christmas Day' })).toBeVisible()
  await page.getByLabel('Date').fill('2099-12-24')
  await page.getByLabel('Name').fill('Christmas Eve')
  await page.getByRole('button', { name: 'Add' }).click()
  await expect(page.getByRole('cell', { name: 'Christmas Eve' })).toBeVisible()

  await page.getByRole('button', { name: 'Remove Christmas Eve' }).click()
  await expect(page.getByRole('cell', { name: 'Christmas Eve' })).toHaveCount(0)
})
