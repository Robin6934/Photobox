<script setup lang="ts">
import {PhotoBoxClient} from "@/OpenApi/Client.ts";

const client = new PhotoBoxClient();

const response = client.checkIfPhotoboxExists("")
</script>

<template>
</template>

<style scoped>

</style>
