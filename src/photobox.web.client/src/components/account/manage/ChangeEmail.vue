<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiClient } from '@/services/api'
import { ApiException, InfoRequest, ResendConfirmationEmailRequest } from '@/OpenApi/Client'

const currentEmail = ref('')
const newEmail = ref('')
const isEmailConfirmed = ref(false)
const loading = ref(false)
const resendLoading = ref(false)
const errorMessage = ref<string | null>(null)
const successMessage = ref<string | null>(null)

onMounted(async () => {
  try {
    const info = await apiClient.getApiManageInfo()
    currentEmail.value = info.email ?? ''
    isEmailConfirmed.value = info.isEmailConfirmed ?? false
    newEmail.value = info.email ?? ''
  } catch {
    errorMessage.value = 'Failed to load account info.'
  }
})

const submit = async () => {
  errorMessage.value = null
  successMessage.value = null
  if (newEmail.value === currentEmail.value) {
    errorMessage.value = 'The new email is the same as your current email.'
    return
  }
  loading.value = true
  try {
    await apiClient.postApiManageInfo(new InfoRequest({ newEmail: newEmail.value }))
    successMessage.value = 'Email updated. If confirmation is required, check your inbox.'
    currentEmail.value = newEmail.value
    isEmailConfirmed.value = false
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

const resendConfirmation = async () => {
  errorMessage.value = null
  successMessage.value = null
  resendLoading.value = true
  try {
    await apiClient.postApiResendConfirmationEmail(
      new ResendConfirmationEmailRequest({ email: currentEmail.value })
    )
    successMessage.value = 'Confirmation email sent. Please check your inbox.'
  } catch (err) {
    if (err instanceof ApiException) {
      errorMessage.value = err.response || `Error ${err.status}`
    } else {
      errorMessage.value = 'An unexpected error occurred.'
    }
  } finally {
    resendLoading.value = false
  }
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" md="6">
        <v-card class="pa-6">
          <v-card-title>Change email</v-card-title>

          <v-alert v-if="successMessage" type="success" class="mb-4">{{ successMessage }}</v-alert>
          <v-alert v-if="errorMessage" type="error" class="mb-4">{{ errorMessage }}</v-alert>

          <v-card-subtitle class="mb-2">Current email</v-card-subtitle>
          <v-card-text class="pt-0">
            {{ currentEmail }}
            <v-chip
              :color="isEmailConfirmed ? 'success' : 'warning'"
              size="x-small"
              variant="tonal"
              class="ml-2"
            >
              {{ isEmailConfirmed ? 'Confirmed' : 'Not confirmed' }}
            </v-chip>
            <div v-if="!isEmailConfirmed" class="mt-2">
              <v-btn
                size="small"
                variant="outlined"
                :loading="resendLoading"
                @click="resendConfirmation"
              >
                Resend confirmation email
              </v-btn>
            </div>
          </v-card-text>

          <v-form @submit.prevent="submit">
            <v-text-field
              v-model="newEmail"
              label="New email"
              type="email"
              required
              autocomplete="email"
            />
            <v-btn type="submit" color="primary" block :loading="loading" class="mt-2">
              Change email
            </v-btn>
          </v-form>

          <div class="mt-4">
            <router-link :to="{ name: 'manage-account' }">← Back to account</router-link>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>