import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import Dashboard from '../views/Dashboard.vue';
import EditProfile from '../views/EditProfile.vue';
import Confirmations from '../views/Confirmations.vue';
import RatingRecommendations from '../views/RatingRecommendations.vue';
import ExportProfile from '../views/ExportProfile.vue';
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes:  [
  { path: '/', component: HomeView },
  { path: '/dashboard', component: Dashboard },
  { path: '/profile/edit', component: EditProfile },
  { path: '/confirmations', component: Confirmations },
  { path: '/rating', component: RatingRecommendations },
  { path: '/export', component: ExportProfile }
]
})

export default router
