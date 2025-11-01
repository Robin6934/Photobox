<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ImageClient } from '../OpenApi/Client.ts'
import Image from "../components/Image.vue";

const images = ref<string[]>([])
const client = new ImageClient()

onMounted(async () => {
  images.value = await client.listImages()
})
</script>


<template>
<div class="d-flex" v-for="item in images" :key="item">
  <Image :image-name="item" :presigned-url="item"/>
</div>
</template>

<style scoped>

</style>
