<template>
  <transition appear enter-active-class="animated zoomIn slower"
    leave-active-class="animated zoomOut slower">
    <Dock :model="items">
      <template #item="{ item }">
        <q-icon :name="item.icon" class="p-dock-link cursor-pointer text-primary" size="3em" />
      </template>
    </Dock>
  </transition>
</template>

<script lang="ts" setup>
import Dock from 'primevue/dock'
import { useRouteHistories } from '../tags/routeHistories'
import { IRouteHistory } from '../tags/types'

const items = useRouteHistories()

const examples: IRouteHistory[] = [
  {
    label: 'Finder',
    icon: 'img:https://primefaces.org/cdn/primevue//images/dock/finder.svg'
  },
  {
    label: 'Terminal',
    icon: 'img:https://primefaces.org/cdn/primevue//images/dock/terminal.svg'
  },
  {
    label: 'App Store',
    icon: 'img:https://primefaces.org/cdn/primevue//images/dock/appstore.svg'
  },
  {
    label: 'Safari',
    icon: 'img:https://primefaces.org/cdn/primevue//images/dock/safari.svg'
  },
  {
    label: 'Photos',
    icon: 'img:https://primefaces.org/cdn/primevue//images/dock/photos.svg'
  },
  {
    label: 'GitHub',
    icon: 'img:https://primefaces.org/cdn/primevue//images/dock/github.svg'
  },
  {
    label: 'Trash',
    icon: 'img:https://primefaces.org/cdn/primevue//images/dock/trash.png'
  }
]

items.value.push(...examples)

console.log('items', items)
// function onDockItemClick (event: Event) {
//   console.log('Dock item clicked')
//   event.preventDefault()
// }
</script>

<style lang="scss" scoped></style>
