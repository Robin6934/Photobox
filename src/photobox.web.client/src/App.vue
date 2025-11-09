<script setup lang="ts">
import { ref } from "vue";
import SidebarMenu from "@/components/SidebarMenu.vue";
const drawer = ref(true)
</script>

<template>
  <v-app>
    <!-- Top App Bar -->
    <v-app-bar app color="primary" dark>
      <v-app-bar-nav-icon @click="drawer = !drawer" :icon="drawer ? 'mdi-close' : 'mdi-menu'"/>
      <v-app-bar-title>Photobox.Web</v-app-bar-title>
    </v-app-bar>

    <!-- Left Sidebar -->
    <v-navigation-drawer app v-model="drawer" width="240">
      <v-list nav>
        <v-list-item
          title="Gallery"
          :to="{ name: 'gallery' , params: { code: '048349' } }"
          link
        />
        <v-list-item
          title="Register"
          :to="{ name: 'register' }"
          link
        />
        <v-list-item
          title="Login"
          :to="{ name: 'login' }"
          link
        />
        <v-list-item
          title="Authenthicated"
          :to="{ name: 'loggedin' }"
          link
        />
      </v-list>
    </v-navigation-drawer>


    <!-- Main content -->
    <v-main>
      <v-container fluid width="100vw">
        <router-view/>
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
html, body, #app {
  height: 100%;
  margin: 0;
}

header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
