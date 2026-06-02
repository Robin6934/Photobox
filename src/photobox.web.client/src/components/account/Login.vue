<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import {ApiException} from '@/OpenApi/Client'
import { useAuth } from '@/services/auth'

// Form model
const input = ref({
  email: '',
  password: '',
  rememberMe: false,
})

// Error message
const errorMessage = ref<string | null>(null)

// Router for navigation
const router = useRouter()
const route = useRoute()
const { login } = useAuth()

// Optional returnUrl from query
const returnUrl = route.query.returnUrl as string || '/'

// Submit login
const loginUser = async () => {
  errorMessage.value = null
  try {
    await login(input.value.email, input.value.password, input.value.rememberMe)
    await router.push(returnUrl)
  } catch (err) {
    if (err instanceof ApiException) {
      if (err.status === 401) {
        errorMessage.value = 'Invalid login attempt.'
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
            <span class="text-h5">Log in</span>
          </v-card-title>

          <!-- Error message -->
          <v-alert v-if="errorMessage" type="error" dense outlined class="mb-4">
            {{ errorMessage }}
          </v-alert>

          <v-form @submit.prevent="loginUser">
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

            <v-checkbox
              v-model="input.rememberMe"
              label="Remember me"
            />

            <!-- Submit button -->
            <v-btn type="submit" color="primary" class="mt-4" block>
              Log in
            </v-btn>
          </v-form>

          <v-row class="mt-4">
            <v-col cols="12">
              <router-link :to="{ name: 'register' }">Register as a new user</router-link>
            </v-col>
          </v-row>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
</style>
