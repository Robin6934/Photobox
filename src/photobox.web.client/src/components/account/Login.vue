<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ApiException } from '@/OpenApi/Client'
import { useAuthStore } from '@/services/auth'

type LoginState = 'credentials' | '2fa-code' | '2fa-recovery'

const credentials = ref({ email: '', password: '' })
const twoFactorCode = ref('')
const recoveryCode = ref('')
const loginState = ref<LoginState>('credentials')
const loading = ref(false)
const errorMessage = ref<string | null>(null)

const router = useRouter()
const route = useRoute()
const { login } = useAuthStore()
const returnUrl = (route.query.returnUrl as string) || '/'

const attempt = async (twoFactorCode?: string, twoFactorRecoveryCode?: string) => {
  errorMessage.value = null
  loading.value = true
  try {
    await login(credentials.value.email, credentials.value.password, twoFactorCode, twoFactorRecoveryCode)
    await router.push(returnUrl)
  } catch (err) {
    if (err instanceof ApiException && err.status === 401) {
      if (err.response?.includes('RequiresTwoFactor')) {
        loginState.value = '2fa-code'
      } else {
        errorMessage.value = loginState.value === 'credentials'
          ? 'Invalid email or password.'
          : 'Invalid code. Please try again.'
      }
    } else {
      errorMessage.value = 'An unexpected error occurred.'
      console.error(err)
    }
  } finally {
    loading.value = false
  }
}

const submitCredentials = () => attempt()
const submit2faCode = () => attempt(twoFactorCode.value)
const submitRecovery = () => attempt(undefined, recoveryCode.value)

const switchToRecovery = () => {
  loginState.value = '2fa-recovery'
  twoFactorCode.value = ''
  errorMessage.value = null
}

const switchToAuthenticator = () => {
  loginState.value = '2fa-code'
  recoveryCode.value = ''
  errorMessage.value = null
}

const backToCredentials = () => {
  loginState.value = 'credentials'
  twoFactorCode.value = ''
  recoveryCode.value = ''
  errorMessage.value = null
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" md="6">

        <!-- Step 1: email + password -->
        <v-card v-if="loginState === 'credentials'" class="pa-6">
          <v-card-title>
            <span class="text-h5">Log in</span>
          </v-card-title>

          <v-alert v-if="errorMessage" type="error" class="mb-4">{{ errorMessage }}</v-alert>

          <v-form @submit.prevent="submitCredentials">
            <v-text-field
              v-model="credentials.email"
              label="Email"
              type="email"
              required
              autocomplete="username"
            />
            <v-text-field
              v-model="credentials.password"
              label="Password"
              type="password"
              required
              autocomplete="current-password"
            />
            <v-btn type="submit" color="primary" block :loading="loading" class="mt-2">
              Log in
            </v-btn>
          </v-form>

          <v-row class="mt-4">
            <v-col cols="12">
              <router-link :to="{ name: 'register' }">Register as a new user</router-link>
            </v-col>
            <v-col cols="12">
              <router-link :to="{ name: 'forgot-password' }">Forgot your password?</router-link>
            </v-col>
          </v-row>
        </v-card>

        <!-- Step 2a: authenticator app code -->
        <v-card v-else-if="loginState === '2fa-code'" class="pa-6">
          <v-card-title>
            <span class="text-h5">Two-factor authentication</span>
          </v-card-title>
          <v-card-subtitle class="mb-4">
            Enter the code from your authenticator app.
          </v-card-subtitle>

          <v-alert v-if="errorMessage" type="error" class="mb-4">{{ errorMessage }}</v-alert>

          <v-form @submit.prevent="submit2faCode">
            <v-text-field
              v-model="twoFactorCode"
              label="Authenticator code"
              required
              autocomplete="one-time-code"
              inputmode="numeric"
              autofocus
            />
            <v-btn type="submit" color="primary" block :loading="loading" class="mt-2">
              Verify
            </v-btn>
          </v-form>

          <v-row class="mt-4">
            <v-col cols="12">
              <a href="#" @click.prevent="switchToRecovery">Use a recovery code instead</a>
            </v-col>
            <v-col cols="12">
              <a href="#" @click.prevent="backToCredentials">← Back to login</a>
            </v-col>
          </v-row>
        </v-card>

        <!-- Step 2b: recovery code -->
        <v-card v-else class="pa-6">
          <v-card-title>
            <span class="text-h5">Recovery code</span>
          </v-card-title>
          <v-card-subtitle class="mb-4">
            Enter one of your recovery codes.
          </v-card-subtitle>

          <v-alert v-if="errorMessage" type="error" class="mb-4">{{ errorMessage }}</v-alert>

          <v-form @submit.prevent="submitRecovery">
            <v-text-field
              v-model="recoveryCode"
              label="Recovery code"
              required
              autocomplete="off"
              autofocus
            />
            <v-btn type="submit" color="primary" block :loading="loading" class="mt-2">
              Verify
            </v-btn>
          </v-form>

          <v-row class="mt-4">
            <v-col cols="12">
              <a href="#" @click.prevent="switchToAuthenticator">Use authenticator app instead</a>
            </v-col>
            <v-col cols="12">
              <a href="#" @click.prevent="backToCredentials">← Back to login</a>
            </v-col>
          </v-row>
        </v-card>

      </v-col>
    </v-row>
  </v-container>
</template>