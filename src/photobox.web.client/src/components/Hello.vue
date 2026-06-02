<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const code = ref('')
const error = ref('')

const goToGallery = () => {
  const trimmed = code.value.trim()
  if (!trimmed) {
    error.value = 'Please enter a gallery code.'
    return
  }
  router.push({ name: 'gallery', params: { code: trimmed } })
}
</script>

<template>
  <v-row justify="center" class="mt-8">
    <v-col cols="12" sm="8" md="5">
      <v-card class="pa-6">
        <v-card-title class="text-h5 mb-2">Open a Gallery</v-card-title>
        <v-card-text>
          <v-text-field
            v-model="code"
            label="Gallery code"
            placeholder="e.g. 048349"
            :error-messages="error"
            @update:model-value="error = ''"
            @keyup.enter="goToGallery"
            maxlength="10"
            variant="outlined"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn color="primary" variant="flat" block @click="goToGallery">
            Open Gallery
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>