<script setup lang="ts">
import { ref } from 'vue'
import { apiClient } from '@/services/api'
import { ApiException, InfoRequest } from '@/OpenApi/Client'

const oldPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const errorMessage = ref<string | null>(null)
const successMessage = ref<string | null>(null)

const submit = async () => {
  errorMessage.value = null
  successMessage.value = null
  if (newPassword.value !== confirmPassword.value) {
    errorMessage.value = 'New passwords do not match.'
    return
  }
  loading.value = true
  try {
    await apiClient.postApiManageInfo(new InfoRequest({
      oldPassword: oldPassword.value,
      newPassword: newPassword.value,
    }))
    successMessage.value = 'Password changed successfully.'
    oldPassword.value = ''
    newPassword.value = ''
    confirmPassword.value = ''
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
          <v-card-title>Change password</v-card-title>

          <v-alert v-if="successMessage" type="success" class="mb-4">{{ successMessage }}</v-alert>
          <v-alert v-if="errorMessage" type="error" class="mb-4">{{ errorMessage }}</v-alert>

          <v-form @submit.prevent="submit">
            <v-text-field
              v-model="oldPassword"
              label="Current password"
              type="password"
              required
              autocomplete="current-password"
            />
            <v-text-field
              v-model="newPassword"
              label="New password"
              type="password"
              required
              autocomplete="new-password"
            />
            <v-text-field
              v-model="confirmPassword"
              label="Confirm new password"
              type="password"
              required
              autocomplete="new-password"
            />
            <v-btn type="submit" color="primary" block :loading="loading" class="mt-2">
              Update password
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