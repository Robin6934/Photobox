import { test, expect } from '@playwright/test'
import {
  setupAuthenticatedSession,
  mockManageInfo,
  mockChangePasswordSuccess,
  mockChangePasswordFailure,
  mockResendConfirmation,
  mockLogout,
  mockDownloadPersonalData,
  mockDeleteAccountSuccess,
  mockDeleteAccountFailure,
  TEST_USER,
} from './helpers/mocks'

// ── Manage account hub ────────────────────────────────────────────────────────

test.describe('Manage account', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuthenticatedSession(page)
    await mockManageInfo(page, { email: TEST_USER.email, isEmailConfirmed: true })
  })

  test('shows email and confirmed status', async ({ page }) => {
    await page.goto('/account/manage')
    await expect(page.getByText(TEST_USER.email)).toBeVisible()
    await expect(page.getByText('Email confirmed')).toBeVisible()
  })

  test('shows unconfirmed chip when email not confirmed', async ({ page }) => {
    await page.unroute('**/api/manage/info')
    await mockManageInfo(page, { email: TEST_USER.email, isEmailConfirmed: false })
    await page.goto('/account/manage')
    await expect(page.getByText('Email not confirmed')).toBeVisible()
  })

  test('shows navigation links', async ({ page }) => {
    await page.goto('/account/manage')
    await expect(page.getByText('Change password')).toBeVisible()
    await expect(page.getByText('Change email')).toBeVisible()
    await expect(page.getByText('Personal data')).toBeVisible()
  })

  test('Change password link navigates correctly', async ({ page }) => {
    await mockChangePasswordSuccess(page)
    await page.goto('/account/manage')
    await page.getByText('Change password').click()
    await expect(page).toHaveURL('/account/manage/change-password')
  })

  test('Change email link navigates correctly', async ({ page }) => {
    await page.goto('/account/manage')
    await page.getByText('Change email').click()
    await expect(page).toHaveURL('/account/manage/email')
  })

  test('Personal data link navigates correctly', async ({ page }) => {
    await page.goto('/account/manage')
    await page.getByText('Personal data').click()
    await expect(page).toHaveURL('/account/manage/personal-data')
  })

  test('Sign out button clears session', async ({ page }) => {
    await mockLogout(page)
    await page.goto('/account/manage')
    await page.getByRole('button', { name: 'Sign out' }).click()
    await expect(page).toHaveURL('/account/login')
  })
})

// ── Change password ───────────────────────────────────────────────────────────

test.describe('Change password', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuthenticatedSession(page)
  })

  test('renders the change password form', async ({ page }) => {
    await page.goto('/account/manage/change-password')
    await expect(page.getByText('Change password')).toBeVisible()
    await expect(page.getByLabel('Current password')).toBeVisible()
    await expect(page.getByLabel('New password')).toBeVisible()
    await expect(page.getByLabel('Confirm new password')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Update password' })).toBeVisible()
  })

  test('shows error when new passwords do not match', async ({ page }) => {
    await page.goto('/account/manage/change-password')

    await page.getByLabel('Current password').fill('OldPass123!')
    await page.getByLabel('New password').fill('NewPass123!')
    await page.getByLabel('Confirm new password').fill('Different!')
    await page.getByRole('button', { name: 'Update password' }).click()

    await expect(page.getByText('New passwords do not match.')).toBeVisible()
  })

  test('shows success message on password change', async ({ page }) => {
    await mockChangePasswordSuccess(page)
    await page.goto('/account/manage/change-password')

    await page.getByLabel('Current password').fill('OldPass123!')
    await page.getByLabel('New password').fill('NewPass123!')
    await page.getByLabel('Confirm new password').fill('NewPass123!')
    await page.getByRole('button', { name: 'Update password' }).click()

    await expect(page.getByText('Password changed successfully.')).toBeVisible()
  })

  test('clears form fields after successful change', async ({ page }) => {
    await mockChangePasswordSuccess(page)
    await page.goto('/account/manage/change-password')

    await page.getByLabel('Current password').fill('OldPass123!')
    await page.getByLabel('New password').fill('NewPass123!')
    await page.getByLabel('Confirm new password').fill('NewPass123!')
    await page.getByRole('button', { name: 'Update password' }).click()

    await expect(page.getByLabel('Current password')).toHaveValue('')
    await expect(page.getByLabel('New password')).toHaveValue('')
  })

  test('shows error on wrong current password', async ({ page }) => {
    await mockChangePasswordFailure(page, 'Incorrect password.')
    await page.goto('/account/manage/change-password')

    await page.getByLabel('Current password').fill('WrongPass!')
    await page.getByLabel('New password').fill('NewPass123!')
    await page.getByLabel('Confirm new password').fill('NewPass123!')
    await page.getByRole('button', { name: 'Update password' }).click()

    await expect(page.getByText(/Incorrect password/i)).toBeVisible()
  })

  test('back link navigates to account', async ({ page }) => {
    await page.goto('/account/manage/change-password')
    await page.getByRole('link', { name: '← Back to account' }).click()
    await expect(page).toHaveURL('/account/manage')
  })
})

// ── Change email ──────────────────────────────────────────────────────────────

test.describe('Change email', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuthenticatedSession(page)
    await mockManageInfo(page, { email: TEST_USER.email, isEmailConfirmed: true })
  })

  test('renders current email and confirmed status', async ({ page }) => {
    await page.goto('/account/manage/email')
    await expect(page.getByText(TEST_USER.email)).toBeVisible()
    await expect(page.getByText('Confirmed')).toBeVisible()
  })

  test('shows resend confirmation button when email unconfirmed', async ({ page }) => {
    await page.unroute('**/api/manage/info')
    await mockManageInfo(page, { email: TEST_USER.email, isEmailConfirmed: false })
    await page.goto('/account/manage/email')
    await expect(page.getByRole('button', { name: 'Resend confirmation email' })).toBeVisible()
  })

  test('shows success after resending confirmation', async ({ page }) => {
    await page.unroute('**/api/manage/info')
    await mockManageInfo(page, { email: TEST_USER.email, isEmailConfirmed: false })
    await mockResendConfirmation(page)
    await page.goto('/account/manage/email')

    await page.getByRole('button', { name: 'Resend confirmation email' }).click()
    await expect(page.getByText('Confirmation email sent')).toBeVisible()
  })

  test('shows error when new email same as current', async ({ page }) => {
    await page.goto('/account/manage/email')

    const newEmailField = page.getByLabel('New email')
    await newEmailField.clear()
    await newEmailField.fill(TEST_USER.email)
    await page.getByRole('button', { name: 'Change email' }).click()

    await expect(page.getByText('The new email is the same as your current email.')).toBeVisible()
  })

  test('shows success after email change', async ({ page }) => {
    await page.goto('/account/manage/email')

    const newEmailField = page.getByLabel('New email')
    await newEmailField.clear()
    await newEmailField.fill('newemail@example.com')
    await page.getByRole('button', { name: 'Change email' }).click()

    await expect(page.getByText(/Email updated/i)).toBeVisible()
  })
})

// ── Personal data ─────────────────────────────────────────────────────────────

test.describe('Personal data', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuthenticatedSession(page)
  })

  test('renders download and delete sections', async ({ page }) => {
    await page.goto('/account/manage/personal-data')
    await expect(page.getByText('Download your data')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Download personal data' })).toBeVisible()
    await expect(page.getByText('Delete account')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Delete my account' })).toBeVisible()
  })

  test('delete confirmation dialog appears on click', async ({ page }) => {
    await page.goto('/account/manage/personal-data')

    await page.getByRole('button', { name: 'Delete my account' }).click()

    await expect(page.getByLabel('Confirm your password')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Permanently delete account' })).toBeVisible()
    await expect(page.getByRole('button', { name: 'Cancel' })).toBeVisible()
  })

  test('cancel hides deletion form', async ({ page }) => {
    await page.goto('/account/manage/personal-data')

    await page.getByRole('button', { name: 'Delete my account' }).click()
    await expect(page.getByLabel('Confirm your password')).toBeVisible()

    await page.getByRole('button', { name: 'Cancel' }).click()
    await expect(page.getByLabel('Confirm your password')).not.toBeVisible()
    await expect(page.getByRole('button', { name: 'Delete my account' })).toBeVisible()
  })

  test('shows error on wrong password during deletion', async ({ page }) => {
    await mockDeleteAccountFailure(page, 'Incorrect password.')
    await page.goto('/account/manage/personal-data')

    await page.getByRole('button', { name: 'Delete my account' }).click()
    await page.getByLabel('Confirm your password').fill('wrongpassword')
    await page.getByRole('button', { name: 'Permanently delete account' }).click()

    await expect(page.getByText('Incorrect password.')).toBeVisible()
  })

  test('redirects to home after successful deletion', async ({ page }) => {
    await mockDeleteAccountSuccess(page)
    await mockLogout(page)
    await page.goto('/account/manage/personal-data')

    await page.getByRole('button', { name: 'Delete my account' }).click()
    await page.getByLabel('Confirm your password').fill('Password123!')
    await page.getByRole('button', { name: 'Permanently delete account' }).click()

    await expect(page).toHaveURL('/')
  })

  test('back link navigates to account', async ({ page }) => {
    await page.goto('/account/manage/personal-data')
    await page.getByRole('link', { name: '← Back to account' }).click()
    await expect(page).toHaveURL('/account/manage')
  })
})