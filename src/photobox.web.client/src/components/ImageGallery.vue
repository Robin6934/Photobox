<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { GalleryClient, type ImageDownloadListResponse } from '../OpenApi/Client.ts'
import Image from "../components/Image.vue";
import { useRoute } from 'vue-router'

const images = ref<ImageDownloadListResponse>()
const client = new GalleryClient()

const route = useRoute()
const code = route.params.code as string

console.log(code)

onMounted(async () => {
  images.value = await client.getImagesFromGalleryCode(code)
})
</script>


<template>
  <div class="d-flex" v-for="item in images?.images" :key="item.originalImageUrl">
    <Image :preview-url="item.downscaledImageUrl ?? ''" :download-url="item.originalImageUrl ?? '#'"/>
  </div>
</template>


<style scoped>

</style>
