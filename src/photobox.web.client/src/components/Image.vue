<template>
  <div class="d-flex flex-column mb-3">
    <v-img
      :src="previewUrl"
      alt="Description"
      class="rounded"
      cover
    ></v-img>

    <v-btn
      class="rounded-0 mt-2"
      color="info"
      variant="flat"
      size="large"
      prepend-icon="mdi-download"
      :href="downloadUrl"
    >
      Download
    </v-btn>
  </div>
</template>

<script setup lang="ts">
import { VImg, VBtn } from 'vuetify/components'

defineProps<{
  previewUrl: string
  downloadUrl: string
}>()
</script>

<style scoped>
.rounded {
  border-radius: 0.25rem;
}
</style>
