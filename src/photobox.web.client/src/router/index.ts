import { createRouter, createWebHistory } from 'vue-router'
import ImageGallery from '../components/ImageGallery.vue'
import Register from "@/components/account/Register.vue";
import Login from "@/components/account/Login.vue";
import Authenthicated from "@/components/account/Authenthicated.vue";

const routes = [
  { path: '/:code', component: ImageGallery, name: 'Gallery' },
  { path: '/', component: ImageGallery, name: 'Gallery' },
  { path: '/account', children: [
      { path: 'register', component: Register },
      { path: 'login', component: Login },
      { path: 'loggedin', component: Authenthicated },
    ]}
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
