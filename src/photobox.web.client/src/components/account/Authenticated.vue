<script setup lang="ts">
import { computed, onMounted } from "vue";
import { storeToRefs } from "pinia";
import { useAuthStore } from "@/services/auth";

const authStore = useAuthStore()
const { currentUser } = storeToRefs(authStore)
const { initializeAuth } = authStore

const displayName = computed(() => currentUser.value?.email ?? currentUser.value?.userName ?? "Signed in");

onMounted(async () => {
  await initializeAuth();
});
</script>

<template>
  <v-card class="pa-6">
    <v-card-title>Account</v-card-title>
    <v-card-text>
      <div v-if="currentUser">
        Signed in as {{ displayName }}
      </div>
      <div v-else>
        You are not signed in.
      </div>
    </v-card-text>
  </v-card>
</template>