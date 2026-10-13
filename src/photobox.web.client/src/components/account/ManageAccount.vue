<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'
import { apiClient } from '@/services/api'
import { ApiException } from '@/OpenApi/Client'
import { useAuthStore } from '@/services/auth'

const router = useRouter()
const authStore = useAuthStore()
const { logout } = authStore

const email = ref('')
const isEmailConfirmed = ref(false)
const loading = ref(true)
const errorMessage = ref<string | null>(null)

const logoutUser = async () => {
  await logout()
  await router.push({ name: 'login' })
}

onMounted(async () => {
  try {
    const info = await apiClient.getApiManageInfo()
    email.value = info.email ?? ''
    isEmailConfirmed.value = info.isEmailConfirmed ?? false
  } catch (err) {
    if (err instanceof ApiException) {
      errorMessage.value = 'Failed to load account info.'
    }
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" md="6">
        <v-card>
          <v-card-title class="pa-6 pb-2">Account</v-card-title>

          <v-card-text v-if="loading" class="pa-6">
            <v-progress-circular indeterminate color="primary" />
          </v-card-text>

          <template v-else>
            <v-alert v-if="errorMessage" type="error" class="mx-6 mb-2">{{ errorMessage }}</v-alert>

            <v-list-item class="px-6 pb-2">
              <v-list-item-title>{{ email }}</v-list-item-title>
              <v-list-item-subtitle>
                <v-chip
                  :color="isEmailConfirmed ? 'success' : 'warning'"
                  size="x-small"
                  variant="tonal"
                  class="mt-1"
                >
                  {{ isEmailConfirmed ? 'Email confirmed' : 'Email not confirmed' }}
                </v-chip>
              </v-list-item-subtitle>
            </v-list-item>

            <v-divider />

            <v-list nav>
              <v-list-item :to="{ name: 'change-password' }" link prepend-icon="mdi-lock-outline">
                <v-list-item-title>Change password</v-list-item-title>
                <template #append><v-icon size="small">mdi-chevron-right</v-icon></template>
              </v-list-item>

              <v-list-item :to="{ name: 'change-email' }" link prepend-icon="mdi-email-outline">
                <v-list-item-title>Change email</v-list-item-title>
                <template #append><v-icon size="small">mdi-chevron-right</v-icon></template>
              </v-list-item>

              <v-list-item :to="{ name: 'personal-data' }" link prepend-icon="mdi-shield-account-outline">
                <v-list-item-title>Personal data</v-list-item-title>
                <template #append><v-icon size="small">mdi-chevron-right</v-icon></template>
              </v-list-item>
            </v-list>

            <v-divider />

            <div class="pa-4">
              <v-btn
                variant="text"
                color="error"
                prepend-icon="mdi-logout"
                @click="logoutUser"
              >
                Sign out
              </v-btn>
            </div>
          </template>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>