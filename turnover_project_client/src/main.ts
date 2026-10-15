import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura'

const app = createApp(App)

app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: Aura
  },
  license: "eyJpZCI6IjUzOTgzMDY0LWY5ODMtNDA5MC1hODdhLWIzNGJhNjQ3N2Y5NCIsInByb2R1Y3QiOiJwcmltZXVpIiwidGllciI6ImNvbW11bml0eSIsInR5cGUiOiJkZXYiLCJpYXQiOjE3OTIwNzI5ODgsImV4cCI6MTgyMzYwODk4OH0.VOeQ1JSZlwdcdvE4_-TAiWEGRpndLxruzZOtVi-uI5x7zDMVr9Th0yPTladYV5UUXyy2rmaSQF0piS2OqVbRAw"
})

app.mount('#app')
