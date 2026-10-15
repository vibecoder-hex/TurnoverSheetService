<script setup lang="ts">
    import Toolbar from "primevue/toolbar";
    import Button from "primevue/button";
</script>

<template>
    <Toolbar>
        <template #start>
          <div class="flex gap-4 items-center">
            <span class="font-bold text-lg ml-3">Оборотная ведомость</span>
            <Button variant="text" severity="primary">
              <RouterLink to="/">Главная</RouterLink>
            </Button>
            <Button variant="text" severity="secondary">
              <RouterLink to="/apartment_list">Квартиры</RouterLink>
            </Button>
          </div>
        </template>
    </Toolbar>
    <RouterView/>
</template>

<style scoped></style>
