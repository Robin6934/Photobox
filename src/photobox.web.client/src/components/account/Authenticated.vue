<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useAuth } from "@/services/auth";

const { currentUser, initializeAuth } = useAuth();

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