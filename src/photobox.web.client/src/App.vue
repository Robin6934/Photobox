<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useAuth } from "@/services/auth";

const drawer = ref(true)
const router = useRouter()
const { initialized, initializeAuth, isAuthenticated, logout } = useAuth()

const logoutUser = async () => {
  await logout()
  await router.push({ name: "hello" })
}

onMounted(async () => {
  await initializeAuth()
})
</script>

<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-app-bar-nav-icon @click="drawer = !drawer" :icon="drawer ? 'mdi-close' : 'mdi-menu'" />
      <v-app-bar-title>Photobox</v-app-bar-title>
    </v-app-bar>

    <v-navigation-drawer app v-model="drawer" width="240">
      <v-list nav>
        <v-list-item title="Home" :to="{ name: 'hello' }" link />
        <v-list-item
          v-if="initialized && !isAuthenticated"
          title="Register"
          :to="{ name: 'register' }"
          link
        />
        <v-list-item
          v-if="initialized && !isAuthenticated"
          title="Login"
          :to="{ name: 'login' }"
          link
        />
        <v-list-item
          v-if="isAuthenticated"
          title="My Photoboxes"
          :to="{ name: 'photoboxes' }"
          link
        />
        <v-list-item
          v-if="isAuthenticated"
          title="Account"
          :to="{ name: 'loggedin' }"
          link
        />
        <v-list-item
          v-if="isAuthenticated"
          title="Logout"
          @click="logoutUser"
        />
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <v-container fluid>
        <router-view />
      </v-container>
    </v-main>
  </v-app>
</template>