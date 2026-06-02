<template>
  <div class="d-flex flex-column">
    <v-img
      :src="previewUrl"
      :aspect-ratio="4 / 3"
      cover
      class="rounded"
    />
    <v-btn
      class="rounded-0 mt-2"
      color="info"
      variant="flat"
      size="large"
      prepend-icon="mdi-download"
      :href="downloadUrl"
      target="_blank"
    >
      Download
    </v-btn>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  previewUrl: string
  downloadUrl: string
}>()
</script>