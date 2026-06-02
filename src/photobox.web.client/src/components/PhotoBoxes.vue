<script setup lang="ts">
import { onMounted, ref } from "vue";
import { type PhotoBoxResponse } from "@/OpenApi/Client";
import { photoBoxClient } from "@/services/api";

const photoBoxes = ref<PhotoBoxResponse[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);

onMounted(async () => {
  try {
    photoBoxes.value = await photoBoxClient.getMyPhotoBoxes();
  } catch {
    error.value = "Failed to load photoboxes.";
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <v-card>
    <v-card-title class="mb-2">Meine Fotoboxen</v-card-title>
    <v-card-text>
      <v-progress-circular v-if="loading" indeterminate color="primary" />

      <v-alert v-else-if="error" type="error">{{ error }}</v-alert>

      <v-alert v-else-if="photoBoxes.length === 0" type="info">
        Du hast noch keine Fotoboxen registriert.
      </v-alert>

      <v-expansion-panels v-else multiple variant="accordion">
        <v-expansion-panel
          v-for="box in photoBoxes"
          :key="box.id"
          :title="box.name"
        >
          <v-expansion-panel-text>
            <v-row align="center">
              <v-col cols="auto">
                <v-avatar size="small" color="success" />
              </v-col>
              <v-col>
                <template v-if="box.currentEventCode">
                  <v-btn
                    variant="text"
                    color="primary"
                    :to="{ name: 'gallery', params: { code: box.currentEventCode } }"
                    class="pa-0"
                  >
                    Current Event: {{ box.currentEventName }}
                  </v-btn>
                </template>
                <span v-else class="text-medium-emphasis">(No Event)</span>
              </v-col>
            </v-row>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-card-text>
  </v-card>
</template>
