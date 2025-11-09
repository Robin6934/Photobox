import { createRouter, createWebHistory } from 'vue-router'
import ImageGallery from '../components/ImageGallery.vue'
import Register from "@/components/account/Register.vue";
import Login from "@/components/account/Login.vue";
import Authenthicated from "@/components/account/Authenthicated.vue";
import Hello from "@/components/Hello.vue";

const routes = [
  { path: '/gallery/:code', component: ImageGallery, name: 'gallery' },
  { path: '/', component: Hello, name: 'hello' },
  { path: '/account', children: [
      { path: 'register', component: Register, name: 'register' },
      { path: 'login', component: Login, name: 'login' },
      { path: 'loggedin', component: Authenthicated, name: 'loggedin' },
    ]}
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
