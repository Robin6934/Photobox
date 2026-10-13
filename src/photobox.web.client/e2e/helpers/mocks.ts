import type { Page } from '@playwright/test'

// ── Shared test data ────────────────────────────────────────────────────────

export const TEST_USER = {
  id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
  email: 'test@photobox.dev',
  userName: 'test@photobox.dev',
  emailConfirmed: true,
}

export const ACCESS_TOKEN = 'test-access-token'
export const REFRESH_TOKEN = 'test-refresh-token'

const TOKEN_RESPONSE = {
  tokenType: 'Bearer',
  accessToken: ACCESS_TOKEN,
  expiresIn: 900,
  refreshToken: REFRESH_TOKEN,
}

// ── Auth helpers ────────────────────────────────────────────────────────────

/**
 * Injects tokens into localStorage and mocks /users/me so the app boots as authenticated.
 * Call this before page.goto() on any page that requires auth.
 */
export async function setupAuthenticatedSession(page: Page) {
  await page.addInitScript(
    ({ accessToken, refreshToken }) => {
      localStorage.setItem('pb_access_token', accessToken)
      localStorage.setItem('pb_refresh_token', refreshToken)
    },
    { accessToken: ACCESS_TOKEN, refreshToken: REFRESH_TOKEN },
  )
  await page.route('**/users/me', (route) => route.fulfill({ json: TEST_USER }))
}

// ── Individual endpoint mocks ───────────────────────────────────────────────

export async function mockLoginSuccess(page: Page) {
  await page.route('**/api/login**', (route) => route.fulfill({ json: TOKEN_RESPONSE }))
}

export async function mockLoginFailure(page: Page) {
  await page.route('**/api/login**', (route) =>
    route.fulfill({
      status: 401,
      contentType: 'application/json',
      body: JSON.stringify({ detail: 'Failed' }),
    }),
  )
}

/**
 * First call returns a 2FA-required 401; subsequent calls succeed.
 */
export async function mockLoginRequires2fa(page: Page) {
  let calls = 0
  await page.route('**/api/login**', (route) => {
    calls++
    if (calls === 1) {
      route.fulfill({
        status: 401,
        contentType: 'application/json',
        body: JSON.stringify({
          detail:
            'Succeeded: False, IsLockedOut: False, IsNotAllowed: False, RequiresTwoFactor: True',
        }),
      })
    } else {
      route.fulfill({ json: TOKEN_RESPONSE })
    }
  })
}

export async function mockLogout(page: Page) {
  await page.route('**/api/logout', (route) => route.fulfill({ status: 200 }))
}

export async function mockRegisterSuccess(page: Page) {
  await page.route('**/api/register', (route) => route.fulfill({ status: 200 }))
}

export async function mockRegisterFailure(page: Page, message = 'Email already in use.') {
  await page.route('**/api/register', (route) =>
    route.fulfill({
      status: 400,
      contentType: 'application/json',
      body: JSON.stringify({ detail: message }),
    }),
  )
}

export async function mockForgotPassword(page: Page) {
  await page.route('**/api/forgotPassword', (route) => route.fulfill({ status: 200 }))
}

export async function mockResetPasswordSuccess(page: Page) {
  await page.route('**/api/resetPassword', (route) => route.fulfill({ status: 200 }))
}

export async function mockResetPasswordFailure(page: Page, message = 'Invalid reset code.') {
  await page.route('**/api/resetPassword', (route) =>
    route.fulfill({
      status: 400,
      contentType: 'application/json',
      body: JSON.stringify({ detail: message }),
    }),
  )
}

export async function mockManageInfo(
  page: Page,
  data: { email: string; isEmailConfirmed: boolean } = {
    email: TEST_USER.email,
    isEmailConfirmed: true,
  },
) {
  await page.route('**/api/manage/info', (route) => {
    if (route.request().method() === 'GET') {
      route.fulfill({ json: data })
    } else {
      route.fulfill({ json: data })
    }
  })
}

export async function mockChangePasswordSuccess(page: Page) {
  await page.route('**/api/manage/info', (route) => {
    if (route.request().method() === 'POST') {
      route.fulfill({ json: { email: TEST_USER.email, isEmailConfirmed: true } })
    } else {
      route.fulfill({ json: { email: TEST_USER.email, isEmailConfirmed: true } })
    }
  })
}

export async function mockChangePasswordFailure(page: Page, message = 'Incorrect password.') {
  await page.route('**/api/manage/info', (route) => {
    if (route.request().method() === 'POST') {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ detail: message }),
      })
    } else {
      route.fulfill({ json: { email: TEST_USER.email, isEmailConfirmed: true } })
    }
  })
}

export async function mockResendConfirmation(page: Page) {
  await page.route('**/api/resendConfirmationEmail', (route) => route.fulfill({ status: 200 }))
}

export async function mockDownloadPersonalData(page: Page) {
  await page.route('**/api/Account/DownloadPersonalData', (route) =>
    route.fulfill({
      status: 200,
      contentType: 'application/json',
      headers: { 'content-disposition': 'attachment; filename=PersonalData.json' },
      body: JSON.stringify({ email: TEST_USER.email }),
    }),
  )
}

export async function mockDeleteAccountSuccess(page: Page) {
  await page.route('**/api/Account/DeleteAccount', (route) => route.fulfill({ status: 200 }))
}

export async function mockDeleteAccountFailure(page: Page, message = 'Incorrect password.') {
  await page.route('**/api/Account/DeleteAccount', (route) =>
    route.fulfill({ status: 400, body: message }),
  )
}