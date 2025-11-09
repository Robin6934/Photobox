<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { GalleryClient, type ImageDownloadListResponse } from '../OpenApi/Client.ts'
import Image from "../components/Image.vue"
import { useRoute } from 'vue-router'

const client = new GalleryClient()
const route = useRoute()

const code = ref(route.params.code as string)
const images = ref<ImageDownloadListResponse | null>(null)

const page = ref(1)
const pageSize = 20

// Load gallery images
const loadGallery = async (galleryCode: string) => {
  try {
    images.value = await client.getImagesFromGalleryCode(galleryCode)
  } catch (err) {
    console.error('Failed to load gallery:', err)
  }
}

onMounted(() => loadGallery(code.value))

// Watch for route changes (if user navigates to another code without full reload)
watch(
  () => route.params.code,
  (newCode) => {
    code.value = newCode as string
    page.value = 1 // reset pagination
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
  <div v-if="!images">Loading...</div>
  <div v-else>
    <div>
      <Image
        v-for="item in paginatedItems"
        :key="item.originalImageUrl"
        :preview-url="item.downscaledImageUrl ?? ''"
        :download-url="item.originalImageUrl ?? '#'"/>
    </div>

    <v-pagination
      v-model="page"
      :length="totalPages"
      class="mt-4"
    />
  </div>
</template>
