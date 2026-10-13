<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { getAccessToken } from '@/services/tokenStore'
import { useAuthStore } from '@/services/auth'

const router = useRouter()
const authStore = useAuthStore()

const downloading = ref(false)
const downloadError = ref<string | null>(null)

const deletePassword = ref('')
const deleting = ref(false)
const deleteError = ref<string | null>(null)
const showDeleteConfirm = ref(false)

const downloadPersonalData = async () => {
  downloadError.value = null
  downloading.value = true
  try {
    const token = getAccessToken()
    const response = await window.fetch('/api/Account/DownloadPersonalData', {
      method: 'POST',
      headers: token ? { Authorization: `Bearer ${token}` } : {},
    })
    if (!response.ok) throw new Error(`HTTP ${response.status}`)
    const blob = await response.blob()
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = 'PersonalData.json'
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    URL.revokeObjectURL(url)
  } catch {
    downloadError.value = 'Download failed. Please try again.'
  } finally {
    downloading.value = false
  }
}

const deleteAccount = async () => {
  deleteError.value = null
  deleting.value = true
  try {
    const token = getAccessToken()
    const response = await window.fetch('/api/Account/DeleteAccount', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify({ password: deletePassword.value }),
    })
    if (!response.ok) {
      const text = await response.text()
      deleteError.value = text || `Error ${response.status}`
      return
    }
    authStore.clearSession()
    await router.push({ name: 'hello' })
  } catch {
    deleteError.value = 'Failed to delete account. Please try again.'
  } finally {
    deleting.value = false
  }
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" md="6">
        <v-card class="pa-6">
          <v-card-title>Personal data</v-card-title>
          <v-card-text>
            Your account contains personal data you have given us. You can download or permanently delete it below.
          </v-card-text>

          <v-divider class="mb-4" />

          <v-card-subtitle>Download your data</v-card-subtitle>
          <v-card-text>
            <v-alert v-if="downloadError" type="error" class="mb-3">{{ downloadError }}</v-alert>
            <v-btn
              color="primary"
              prepend-icon="mdi-download"
              :loading="downloading"
              @click="downloadPersonalData"
            >
              Download personal data
            </v-btn>
          </v-card-text>

          <v-divider class="my-4" />

          <v-card-subtitle class="text-error">Delete account</v-card-subtitle>
          <v-card-text>
            <v-alert type="warning" variant="tonal" class="mb-4">
              Deleting your account is permanent and cannot be undone. All your data will be removed.
            </v-alert>

            <v-btn
              v-if="!showDeleteConfirm"
              color="error"
              variant="outlined"
              prepend-icon="mdi-delete-forever"
              @click="showDeleteConfirm = true"
            >
              Delete my account
            </v-btn>

            <template v-else>
              <v-alert v-if="deleteError" type="error" class="mb-3">{{ deleteError }}</v-alert>
              <v-text-field
                v-model="deletePassword"
                label="Confirm your password"
                type="password"
                autocomplete="current-password"
                class="mb-2"
              />
              <div class="d-flex ga-2">
                <v-btn
                  color="error"
                  :loading="deleting"
                  @click="deleteAccount"
                >
                  Permanently delete account
                </v-btn>
                <v-btn variant="text" @click="showDeleteConfirm = false; deletePassword = ''">
                  Cancel
                </v-btn>
              </div>
            </template>
          </v-card-text>

          <v-divider class="mb-4" />

          <div class="px-2">
            <router-link :to="{ name: 'manage-account' }">← Back to account</router-link>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>