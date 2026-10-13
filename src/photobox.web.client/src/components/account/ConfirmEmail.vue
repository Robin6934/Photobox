<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { apiClient } from '@/services/api'
import { ApiException } from '@/OpenApi/Client'

const route = useRoute()
const status = ref<'pending' | 'success' | 'error'>('pending')
const message = ref('')

onMounted(async () => {
  const userId = route.query.userId as string | null
  const code = route.query.code as string | null
  const changedEmail = (route.query.changedEmail as string) ?? undefined

  if (!userId || !code) {
    status.value = 'error'
    message.value = 'Invalid confirmation link. Please request a new one.'
    return
  }

  try {
    await apiClient.getApiConfirmEmail(userId, code, changedEmail ?? null)
    status.value = 'success'
    message.value = 'Your email has been confirmed.'
  } catch (err) {
    status.value = 'error'
    if (err instanceof ApiException && err.status === 400) {
      message.value = 'Email confirmation failed. The link may have expired.'
    } else {
      message.value = 'An unexpected error occurred.'
    }
  }
})
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" md="6">
        <v-card class="pa-6">
          <v-card-title>Email confirmation</v-card-title>
          <v-card-text>
            <div v-if="status === 'pending'" class="d-flex align-center ga-3">
              <v-progress-circular indeterminate color="primary" size="20" />
              Confirming your email…
            </div>
            <template v-else-if="status === 'success'">
              <v-alert type="success" class="mb-4">{{ message }}</v-alert>
              <v-btn :to="{ name: 'login' }" color="primary">Go to login</v-btn>
            </template>
            <v-alert v-else type="error">
              {{ message }}
              <div class="mt-3">
                <router-link :to="{ name: 'login' }">Go to login</router-link>
              </div>
            </v-alert>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>