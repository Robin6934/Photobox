<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import type { ImageDownloadListResponse } from '@/OpenApi/Client'
import { galleryClient } from '@/services/api'
import Image from '@/components/Image.vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const code = ref(route.params.code as string)
const images = ref<ImageDownloadListResponse | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

const page = ref(1)
const pageSize = 20

const loadGallery = async (galleryCode: string) => {
  loading.value = true
  error.value = null
  try {
    images.value = await galleryClient.getImagesFromGalleryCode(galleryCode)
  } catch {
    error.value = 'Failed to load gallery. Check that the code is correct.'
  } finally {
    loading.value = false
  }
}

onMounted(() => loadGallery(code.value))

watch(
  () => route.params.code,
  (newCode) => {
    code.value = newCode as string
    page.value = 1
    loadGallery(code.value)
  }
)

const totalPages = computed(() =>
  images.value?.images ? Math.ceil(images.value.images.length / pageSize) : 1
)

const paginatedItems = computed(() => {
  if (!images.value?.images) return []
  const start = (page.value - 1) * pageSize
  return images.value.images.slice(start, start + pageSize)
})
</script>

<template>
  <div>
    <div v-if="loading" class="gallery-grid">
      <v-skeleton-loader v-for="n in 6" :key="n" type="image" class="rounded" />
    </div>

    <v-alert v-else-if="error" type="error" class="mb-4">
      {{ error }}
    </v-alert>

    <template v-else-if="images">
      <p class="text-body-2 text-medium-emphasis mb-4">
        {{ images.images?.length ?? 0 }} photos
      </p>

      <div class="gallery-grid">
        <Image
          v-for="item in paginatedItems"
          :key="item.originalImageUrl"
          :preview-url="item.downscaledImageUrl ?? ''"
          :download-url="item.originalImageUrl ?? '#'"
        />
      </div>

      <v-pagination
        v-if="totalPages > 1"
        v-model="page"
        :length="totalPages"
        class="mt-4"
      />
    </template>
  </div>
</template>

<style scoped>
.gallery-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 1rem;
}
</style>