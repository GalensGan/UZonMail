<template>
  <q-list class="text-accent">
    <LogoItem />

    <q-separator />

    <MenuItem v-for="route in menuRoutes" :key="route.name" :routeRaw="route">
    </MenuItem>
  </q-list>
</template>

<script lang="ts" setup>
import LogoItem from './logoItem.vue'
import MenuItem from './menuItem.vue'

import { getMenuRoute } from './helper'
import { constantRoutes } from 'src/router/routes'

const menuRoutes = constantRoutes.map(x => getMenuRoute(x))
</script>

<style lang="scss" scoped></style>
