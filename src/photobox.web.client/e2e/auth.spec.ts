import { test, expect } from '@playwright/test'
import {
  mockLoginSuccess,
  mockLoginFailure,
  mockLoginRequires2fa,
  mockRegisterSuccess,
  mockRegisterFailure,
  mockForgotPassword,
  mockResetPasswordSuccess,
  mockResetPasswordFailure,
  setupAuthenticatedSession,
  TEST_USER,
} from './helpers/mocks'

// ── Login ────────────────────────────────────────────────────────────────────

test.describe('Login', () => {
  test('renders the login form', async ({ page }) => {
    await page.goto('/account/login')
    await expect(page.getByText('Log in')).toBeVisible()
    await expect(page.getByLabel('Email')).toBeVisible()
    await expect(page.getByLabel('Password')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Log in' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Register as a new user' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Forgot your password?' })).toBeVisible()
  })

  test('shows error on invalid credentials', async ({ page }) => {
    await mockLoginFailure(page)
    await page.goto('/account/login')

    await page.getByLabel('Email').fill('wrong@example.com')
    await page.getByLabel('Password').fill('wrongpassword')
    await page.getByRole('button', { name: 'Log in' }).click()

    await expect(page.getByText('Invalid email or password.')).toBeVisible()
  })

  test('redirects to home on successful login', async ({ page }) => {
    await mockLoginSuccess(page)
    await page.route('**/users/me', (route) => route.fulfill({ json: TEST_USER }))
    await page.goto('/account/login')

    await page.getByLabel('Email').fill(TEST_USER.email)
    await page.getByLabel('Password').fill('Password123!')
    await page.getByRole('button', { name: 'Log in' }).click()

    await expect(page).toHaveURL('/')
  })

  test('redirects to returnUrl after login', async ({ page }) => {
    await mockLoginSuccess(page)
    await page.route('**/users/me', (route) => route.fulfill({ json: TEST_USER }))
    await page.goto('/account/login?returnUrl=%2Faccount%2Fmanage')

    await page.getByLabel('Email').fill(TEST_USER.email)
    await page.getByLabel('Password').fill('Password123!')
    await page.getByRole('button', { name: 'Log in' }).click()

    await expect(page).toHaveURL('/account/manage')
  })

  test('shows 2FA step when required', async ({ page }) => {
    await mockLoginRequires2fa(page)
    await page.route('**/users/me', (route) => route.fulfill({ json: TEST_USER }))
    await page.goto('/account/login')

    await page.getByLabel('Email').fill(TEST_USER.email)
    await page.getByLabel('Password').fill('Password123!')
    await page.getByRole('button', { name: 'Log in' }).click()

    await expect(page.getByText('Two-factor authentication')).toBeVisible()
    await expect(page.getByLabel('Authenticator code')).toBeVisible()
    await expect(page.getByRole('link', { name: 'Use a recovery code instead' })).toBeVisible()
  })

  test('completes login with 2FA code', async ({ page }) => {
    await mockLoginRequires2fa(page)
    await page.route('**/users/me', (route) => route.fulfill({ json: TEST_USER }))
    await page.goto('/account/login')

    await page.getByLabel('Email').fill(TEST_USER.email)
    await page.getByLabel('Password').fill('Password123!')
    await page.getByRole('button', { name: 'Log in' }).click()

    await expect(page.getByLabel('Authenticator code')).toBeVisible()
    await page.getByLabel('Authenticator code').fill('123456')
    await page.getByRole('button', { name: 'Verify' }).click()

    await expect(page).toHaveURL('/')
  })

  test('switches to recovery code and completes login', async ({ page }) => {
    await mockLoginRequires2fa(page)
    await page.route('**/users/me', (route) => route.fulfill({ json: TEST_USER }))
    await page.goto('/account/login')

    await page.getByLabel('Email').fill(TEST_USER.email)
    await page.getByLabel('Password').fill('Password123!')
    await page.getByRole('button', { name: 'Log in' }).click()

    await page.getByRole('link', { name: 'Use a recovery code instead' }).click()
    await expect(page.getByText('Recovery code')).toBeVisible()
    await expect(page.getByLabel('Recovery code')).toBeVisible()

    await page.getByLabel('Recovery code').fill('ABCD-1234')
    await page.getByRole('button', { name: 'Verify' }).click()

    await expect(page).toHaveURL('/')
  })

  test('can go back to credentials from 2FA step', async ({ page }) => {
    await mockLoginRequires2fa(page)
    await page.goto('/account/login')

    await page.getByLabel('Email').fill(TEST_USER.email)
    await page.getByLabel('Password').fill('Password123!')
    await page.getByRole('button', { name: 'Log in' }).click()

    await page.getByRole('link', { name: '← Back to login' }).click()
    await expect(page.getByText('Log in')).toBeVisible()
    await expect(page.getByLabel('Email')).toBeVisible()
  })
})

// ── Register ─────────────────────────────────────────────────────────────────

test.describe('Register', () => {
  test('renders the registration form', async ({ page }) => {
    await page.goto('/account/register')
    await expect(page.getByText('Register')).toBeVisible()
    await expect(page.getByLabel('Email')).toBeVisible()
    await expect(page.getByLabel('Password', { exact: true })).toBeVisible()
    await expect(page.getByLabel('Confirm Password')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Register' })).toBeVisible()
  })

  test('shows error when passwords do not match', async ({ page }) => {
    await page.goto('/account/register')

    await page.getByLabel('Email').fill('new@example.com')
    await page.getByLabel('Password', { exact: true }).fill('Password123!')
    await page.getByLabel('Confirm Password').fill('DifferentPassword!')
    await page.getByRole('button', { name: 'Register' }).click()

    await expect(page.getByText('Passwords do not match.')).toBeVisible()
  })

  test('redirects to login on successful registration', async ({ page }) => {
    await mockRegisterSuccess(page)
    await page.goto('/account/register')

    await page.getByLabel('Email').fill('new@example.com')
    await page.getByLabel('Password', { exact: true }).fill('Password123!')
    await page.getByLabel('Confirm Password').fill('Password123!')
    await page.getByRole('button', { name: 'Register' }).click()

    await expect(page).toHaveURL('/account/login')
  })

  test('shows error on failed registration', async ({ page }) => {
    await mockRegisterFailure(page, 'Email already in use.')
    await page.goto('/account/register')

    await page.getByLabel('Email').fill('existing@example.com')
    await page.getByLabel('Password', { exact: true }).fill('Password123!')
    await page.getByLabel('Confirm Password').fill('Password123!')
    await page.getByRole('button', { name: 'Register' }).click()

    await expect(page.getByText(/Error/)).toBeVisible()
  })
})

// ── Forgot password ──────────────────────────────────────────────────────────

test.describe('Forgot password', () => {
  test('renders the forgot password form', async ({ page }) => {
    await page.goto('/account/forgot-password')
    await expect(page.getByText('Forgot password?')).toBeVisible()
    await expect(page.getByLabel('Email')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Send reset code' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Back to login' })).toBeVisible()
  })

  test('shows success message after submission', async ({ page }) => {
    await mockForgotPassword(page)
    await page.goto('/account/forgot-password')

    await page.getByLabel('Email').fill('test@example.com')
    await page.getByRole('button', { name: 'Send reset code' }).click()

    await expect(
      page.getByText(/password reset code has been sent/i),
    ).toBeVisible()
    await expect(page.getByRole('link', { name: 'Enter your reset code' })).toBeVisible()
  })
})

// ── Reset password ───────────────────────────────────────────────────────────

test.describe('Reset password', () => {
  test('renders the reset password form', async ({ page }) => {
    await page.goto('/account/reset-password')
    await expect(page.getByText('Reset password')).toBeVisible()
    await expect(page.getByLabel('Email')).toBeVisible()
    await expect(page.getByLabel('Reset code')).toBeVisible()
    await expect(page.getByLabel('New password')).toBeVisible()
    await expect(page.getByLabel('Confirm new password')).toBeVisible()
  })

  test('pre-fills reset code from URL query param', async ({ page }) => {
    await page.goto('/account/reset-password?code=MY-RESET-CODE-123')
    const codeField = page.getByLabel('Reset code')
    await expect(codeField).toHaveValue('MY-RESET-CODE-123')
  })

  test('shows error when passwords do not match', async ({ page }) => {
    await page.goto('/account/reset-password')

    await page.getByLabel('Email').fill('test@example.com')
    await page.getByLabel('Reset code').fill('some-code')
    await page.getByLabel('New password').fill('NewPass123!')
    await page.getByLabel('Confirm new password').fill('Different!')
    await page.getByRole('button', { name: 'Reset password' }).click()

    await expect(page.getByText('Passwords do not match.')).toBeVisible()
  })

  test('shows success on valid reset', async ({ page }) => {
    await mockResetPasswordSuccess(page)
    await page.goto('/account/reset-password?code=valid-code')

    await page.getByLabel('Email').fill('test@example.com')
    await page.getByLabel('Reset code').fill('valid-code')
    await page.getByLabel('New password').fill('NewPass123!')
    await page.getByLabel('Confirm new password').fill('NewPass123!')
    await page.getByRole('button', { name: 'Reset password' }).click()

    await expect(
      page.getByText('Your password has been reset successfully.'),
    ).toBeVisible()
    await expect(page.getByRole('button', { name: 'Go to login' })).toBeVisible()
  })

  test('shows error on failed reset', async ({ page }) => {
    await mockResetPasswordFailure(page, 'Invalid reset code.')
    await page.goto('/account/reset-password')

    await page.getByLabel('Email').fill('test@example.com')
    await page.getByLabel('Reset code').fill('bad-code')
    await page.getByLabel('New password').fill('NewPass123!')
    await page.getByLabel('Confirm new password').fill('NewPass123!')
    await page.getByRole('button', { name: 'Reset password' }).click()

    await expect(page.getByText(/Invalid reset code/i)).toBeVisible()
  })
})