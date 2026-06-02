<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import {ApiException, RegisterRequest} from '@/OpenApi/Client'
import { apiClient } from '@/services/api'

// Form model
const input = ref({
  email: '',
  password: '',
  confirmPassword: '',
})

// Error message
const errorMessage = ref<string | null>(null)

// Router for navigation
const router = useRouter()

// Submit login
const registerUser = async () => {
  errorMessage.value = null

  if (input.value.password !== input.value.confirmPassword) {
    errorMessage.value = 'Passwords do not match.'
    return
  }

  try {
    const request = new RegisterRequest({
      email: input.value.email,
      password: input.value.password
    })

    await apiClient.postApiRegister(request);
    await router.push({name: 'login'})
  } catch (err) {
    if (err instanceof ApiException) {
      if (err.status === 401) {
        errorMessage.value = 'Invalid registration attempt.'
      } else {
        errorMessage.value = `Error ${err.status}: ${err.response || 'Unexpected error'}`
      }
    } else {
      // Non-API exceptions (network issues, TypeScript errors)
      errorMessage.value = 'An unexpected error occurred.'
      console.error(err)
    }
  }
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" md="6">
        <v-card class="pa-6">
          <v-card-title>
            <span class="text-h5">Register</span>
          </v-card-title>

          <!-- Error message -->
          <v-alert v-if="errorMessage" type="error" dense outlined class="mb-4">
            {{ errorMessage }}
          </v-alert>

          <v-form @submit.prevent="registerUser">
            <!-- Email -->
            <v-text-field
              v-model="input.email"
              label="Email"
              type="email"
              required
              autocomplete="username"
            />

            <!-- Password -->
            <v-text-field
              v-model="input.password"
              label="Password"
              type="password"
              required
              autocomplete="current-password"
            />

            <v-text-field
              v-model="input.confirmPassword"
              label="Confirm Password"
              type="password"
              required
              autocomplete="current-password"
            />

            <!-- Submit button -->
            <v-btn type="submit" color="primary" class="mt-4" block>
              Register
            </v-btn>
          </v-form>

        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
/* Optional spacing tweaks */
</style>
