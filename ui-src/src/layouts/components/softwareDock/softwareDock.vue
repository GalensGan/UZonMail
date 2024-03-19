<template>
  <Dock :model="items">
    <template #item="{ item }">
      <q-icon :name="item.icon" class="p-dock-link cursor-pointer text-primary" size="3em" />
    </template>
  </Dock>
</template>

<script lang="ts" setup>
import Dock from 'primevue/dock'
import { useRouteHistories } from '../tags/routeHistories'

const items = useRouteHistories()

console.log('items', items)
// function onDockItemClick (event: Event) {
//   console.log('Dock item clicked')
//   event.preventDefault()
// }
</script>

<style lang="scss" scoped></style>
