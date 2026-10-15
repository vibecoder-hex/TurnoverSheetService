import { createRouter, createWebHistory } from 'vue-router'
import HomeView from "@/views/HomeView.vue";
import ApartmentListView from "@/views/ApartmentListView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomeView, name: 'HomeView' },
    { path: '/apartment_list', component: ApartmentListView, name: 'ApartmentListView' },
  ],
})

export default router
