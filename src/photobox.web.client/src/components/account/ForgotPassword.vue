<script setup lang="ts">
import { ref } from 'vue'
import { apiClient } from '@/services/api'
import { ApiException, ForgotPasswordRequest } from '@/OpenApi/Client'

const email = ref('')
const loading = ref(false)
const submitted = ref(false)
const errorMessage = ref<string | null>(null)

const submit = async () => {
  errorMessage.value = null
  loading.value = true
  try {
    await apiClient.postApiForgotPassword(new ForgotPasswordRequest({ email: email.value }))
    submitted.value = true
  } catch (err) {
    if (err instanceof ApiException) {
      errorMessage.value = `Error ${err.status}: ${err.response || 'Unexpected error'}`
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
            <span class="text-h5">Forgot password?</span>
          </v-card-title>

          <v-card-text v-if="submitted">
            <v-alert type="success" class="mb-4">
              If an account with that email exists, a password reset code has been sent. Please check your email.
            </v-alert>
            <router-link :to="{ name: 'reset-password' }">Enter your reset code</router-link>
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
              <v-btn type="submit" color="primary" block :loading="loading" class="mt-2">
                Send reset code
              </v-btn>
            </v-form>

            <v-row class="mt-4">
              <v-col>
                <router-link :to="{ name: 'login' }">Back to login</router-link>
              </v-col>
            </v-row>
          </template>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>