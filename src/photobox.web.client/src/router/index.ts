import { createRouter, createWebHistory } from 'vue-router'
import ImageGallery from '../components/ImageGallery.vue'
import Register from '@/components/account/Register.vue'
import Login from '@/components/account/Login.vue'
import Authenticated from '@/components/account/Authenticated.vue'
import Hello from '@/components/Hello.vue'
import PhotoBoxes from '@/components/PhotoBoxes.vue'
import ForgotPassword from '@/components/account/ForgotPassword.vue'
import ResetPassword from '@/components/account/ResetPassword.vue'
import ConfirmEmail from '@/components/account/ConfirmEmail.vue'
import ManageAccount from '@/components/account/ManageAccount.vue'
import ChangePassword from '@/components/account/manage/ChangePassword.vue'
import ChangeEmail from '@/components/account/manage/ChangeEmail.vue'
import PersonalData from '@/components/account/manage/PersonalData.vue'

const routes = [
  { path: '/gallery/:code', component: ImageGallery, name: 'gallery' },
  { path: '/', component: Hello, name: 'hello' },
  { path: '/photoboxes', component: PhotoBoxes, name: 'photoboxes', meta: { requiresAuth: true } },
  {
    path: '/account',
    children: [
      { path: 'register', component: Register, name: 'register' },
      { path: 'login', component: Login, name: 'login' },
      { path: 'loggedin', component: Authenticated, name: 'loggedin' },
      { path: 'forgot-password', component: ForgotPassword, name: 'forgot-password' },
      { path: 'reset-password', component: ResetPassword, name: 'reset-password' },
      { path: 'confirm-email', component: ConfirmEmail, name: 'confirm-email' },
      { path: 'manage', component: ManageAccount, name: 'manage-account', meta: { requiresAuth: true } },
      { path: 'manage/change-password', component: ChangePassword, name: 'change-password', meta: { requiresAuth: true } },
      { path: 'manage/email', component: ChangeEmail, name: 'change-email', meta: { requiresAuth: true } },
      { path: 'manage/personal-data', component: PersonalData, name: 'personal-data', meta: { requiresAuth: true } },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach(async (to) => {
  if (!to.meta.requiresAuth) return true

  const { useAuthStore } = await import('@/services/auth')
  const authStore = useAuthStore()
  await authStore.initializeAuth()

  if (!authStore.isAuthenticated) {
    return { name: 'login', query: { returnUrl: to.fullPath } }
  }
})

export default router