import { createRouter, createWebHistory } from 'vue-router'
import ImageGallery from '../components/ImageGallery.vue'
import Register from "@/components/account/Register.vue";
import Login from "@/components/account/Login.vue";
import Authenticated from "@/components/account/Authenticated.vue";
import Hello from "@/components/Hello.vue";
import PhotoBoxes from "@/components/PhotoBoxes.vue";

const routes = [
  { path: '/gallery/:code', component: ImageGallery, name: 'gallery' },
  { path: '/', component: Hello, name: 'hello' },
  { path: '/photoboxes', component: PhotoBoxes, name: 'photoboxes' },
  { path: '/account', children: [
      { path: 'register', component: Register, name: 'register' },
      { path: 'login', component: Login, name: 'login' },
      { path: 'loggedin', component: Authenticated, name: 'loggedin' },
    ]}
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
