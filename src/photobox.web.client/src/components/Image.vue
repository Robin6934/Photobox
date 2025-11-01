<template>
  <div class="d-flex flex-column mb-3">
    <v-img
      :src="imageUrl"
      alt="Description"
      class="rounded"
      max-width="100%"
      cover
    ></v-img>

    <v-btn
      class="rounded-0 mt-2"
      color="info"
      variant="flat"
      size="large"
      prepend-icon="mdi-download"
      :href="presignedUrl"
    >
      Download
    </v-btn>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ImageClient } from "../OpenApi/Client.ts";
import { VImg, VBtn } from 'vuetify/components'

const props = defineProps<{
  imageName: string
  presignedUrl: string
}>()

const imageUrl = ref<string>('')

const client = new ImageClient()

onMounted(async () => {
  try {
    imageUrl.value = await client.getPreviewImagePreSignedUrl(props.imageName)
  } catch (err) {
    console.error('Failed to fetch preview image URL:', err)
  }
})
</script>

<style scoped>
.rounded {
  border-radius: 0.25rem;
}
</style>
