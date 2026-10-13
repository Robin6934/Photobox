<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { apiClient } from '@/services/api'
import { ApiException, ResetPasswordRequest } from '@/OpenApi/Client'

const route = useRoute()

const email = ref('')
const resetCode = ref((route.query.code as string) ?? '')
const newPassword = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const success = ref(false)
const errorMessage = ref<string | null>(null)

const submit = async () => {
  errorMessage.value = null
  if (newPassword.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match.'
    return
  }
  loading.value = true
  try {
    await apiClient.postApiResetPassword(new ResetPasswordRequest({
      email: email.value,
      resetCode: resetCode.value,
      newPassword: newPassword.value,
    }))
    success.value = true
  } catch (err) {
    if (err instanceof ApiException) {
      errorMessage.value = err.response || `Error ${err.status}`
    } else {
      errorMessage.value = 'An unexpected error occurred.'
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" md="6">
        <v-card class="pa-6">
          <v-card-title>
            <span class="text-h5">Reset password</span>
          </v-card-title>

          <v-card-text v-if="success">
            <v-alert type="success" class="mb-4">Your password has been reset successfully.</v-alert>
            <v-btn :to="{ name: 'login' }" color="primary">Go to login</v-btn>
          </v-card-text>

          <template v-else>
            <v-alert v-if="errorMessage" type="error" class="mb-4">{{ errorMessage }}</v-alert>

            <v-form @submit.prevent="submit">
              <v-text-field
                v-model="email"
                label="Email"
                type="email"
                required
                autocomplete="username"
              />
              <v-text-field
                v-model="resetCode"
                label="Reset code"
                required
                hint="Enter the code from the email you received"
                persistent-hint
              />
              <v-text-field
                v-model="newPassword"
                label="New password"
                type="password"
                required
                autocomplete="new-password"
                class="mt-2"
              />
              <v-text-field
                v-model="confirmPassword"
                label="Confirm new password"
                type="password"
                required
                autocomplete="new-password"
              />
              <v-btn type="submit" color="primary" block :loading="loading" class="mt-2">
                Reset password
              </v-btn>
            </v-form>

            <v-row class="mt-4">
              <v-col>
                <router-link :to="{ name: 'forgot-password' }">Request a new code</router-link>
              </v-col>
            </v-row>
          </template>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>