import { test, expect } from '@playwright/test'
import { setupAuthenticatedSession, mockLogout, TEST_USER } from './helpers/mocks'

// ── Auth guard ───────────────────────────────────────────────────────────────

test.describe('Auth guard — unauthenticated', () => {
  const protectedRoutes = [
    { path: '/account/manage', name: 'manage account' },
    { path: '/account/manage/change-password', name: 'change password' },
    { path: '/account/manage/email', name: 'change email' },
    { path: '/account/manage/personal-data', name: 'personal data' },
    { path: '/photoboxes', name: 'photoboxes' },
  ]

  for (const route of protectedRoutes) {
    test(`redirects ${route.name} to login`, async ({ page }) => {
      await page.goto(route.path)
      await expect(page).toHaveURL(/\/account\/login/)
    })

    test(`${route.name} redirect preserves returnUrl`, async ({ page }) => {
      await page.goto(route.path)
      await expect(page).toHaveURL(new RegExp(`returnUrl=.*${encodeURIComponent(route.path)}`))
    })
  }
})

// ── Sidebar — unauthenticated ─────────────────────────────────────────────────

test.describe('Sidebar — unauthenticated', () => {
  test('shows Register and Login links', async ({ page }) => {
    await page.goto('/')
    await expect(page.getByRole('link', { name: 'Register' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Login' })).toBeVisible()
  })

  test('does not show My Photoboxes, Account or Logout', async ({ page }) => {
    await page.goto('/')
    await expect(page.getByRole('link', { name: 'My Photoboxes' })).not.toBeVisible()
    await expect(page.getByRole('link', { name: 'Account' })).not.toBeVisible()
    await expect(page.getByText('Logout')).not.toBeVisible()
  })
})

// ── Sidebar — authenticated ───────────────────────────────────────────────────

test.describe('Sidebar — authenticated', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuthenticatedSession(page)
  })

  test('shows My Photoboxes, Account and Logout links', async ({ page }) => {
    await page.goto('/')
    await expect(page.getByRole('link', { name: 'My Photoboxes' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Account' })).toBeVisible()
    await expect(page.getByText('Logout')).toBeVisible()
  })

  test('does not show Register or Login links', async ({ page }) => {
    await page.goto('/')
    await expect(page.getByRole('link', { name: 'Register' })).not.toBeVisible()
    await expect(page.getByRole('link', { name: 'Login' })).not.toBeVisible()
  })

  test('Account link navigates to manage page', async ({ page }) => {
    await page.route('**/api/manage/info', (route) =>
      route.fulfill({ json: { email: TEST_USER.email, isEmailConfirmed: true } }),
    )
    await page.goto('/')
    await page.getByRole('link', { name: 'Account' }).click()
    await expect(page).toHaveURL('/account/manage')
  })

  test('Logout clears session and shows unauthenticated sidebar', async ({ page }) => {
    await mockLogout(page)
    await page.goto('/')

    await page.getByText('Logout').click()

    await expect(page).toHaveURL('/')
    await expect(page.getByRole('link', { name: 'Register' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Login' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Account' })).not.toBeVisible()
  })
})

// ── Public routes — authenticated user ───────────────────────────────────────

test.describe('Public routes — authenticated user', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuthenticatedSession(page)
  })

  test('can still visit login page when authenticated', async ({ page }) => {
    await page.goto('/account/login')
    await expect(page).toHaveURL('/account/login')
    await expect(page.getByText('Log in')).toBeVisible()
  })

  test('can still visit register page when authenticated', async ({ page }) => {
    await page.goto('/account/register')
    await expect(page).toHaveURL('/account/register')
  })
})