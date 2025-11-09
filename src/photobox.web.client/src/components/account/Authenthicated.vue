<script setup lang="ts">
import { ApiException, PhotoBoxClient } from "@/OpenApi/Client.ts";
import { ref, onMounted } from "vue";

const client = new PhotoBoxClient();
const message = ref<string>("");

onMounted(async () => {
  try {
    const response = await client.checkIfPhotoboxExists(
      "K1NR40J2T4DCJYKWC88PYPSPNJ4PSNBGH2GRRQZ7DXB2SNAXSYDG"
    );
    message.value = response.exists ? "exists" : "not found";
  } catch (error) {
    if (error instanceof ApiException) {
      message.value = error.message;
    } else {
      message.value = "an error occurred";
      console.error(error);
    }
  }
});
</script>

<template>
  <div>
    {{ message }}
  </div>
</template>

<style scoped>
</style>
