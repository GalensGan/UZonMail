<template>
  <div style="max-width: 350px">
    <q-list bordered class="q-my-md q-ml-md q-mr-xs">
      <LogoItem />

      <q-separator />

      <MenuItem v-for="route in constantRoutes" :key="route.name" :routeRaw="route">
      </MenuItem>
    </q-list>
  </div>
</template>

<script lang="ts" setup>
import LogoItem from './logoItem.vue'
import MenuItem from './menuItem.vue'

import { constantRoutes } from 'src/router/routes'
console.log(constantRoutes)
</script>

<style lang="scss" scoped></style>
