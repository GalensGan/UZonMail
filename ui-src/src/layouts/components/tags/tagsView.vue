<template>
  <div>
    <q-chip v-for="item in routes" :key="item.fullPath" :class="getTagClass(item)" square clickable
      transition-show="jump-right" transition-hide="jump-left" @click="goToRoute(item)">
      {{ item.label }}
    </q-chip>
  </div>
</template>

<script lang="ts" setup>
import { IRouteHistory } from './types'
import { useRouteHistories } from './routeHistories'

// 获取历史 routes
const routes = useRouteHistories()

function getTagClass (item: IRouteHistory) {
  return {
    'bg-secondary': item.isActive,
    'text-white': !item.isActive
  }
}

const router = useRouter()
function goToRoute (item: IRouteHistory) {
  router.push({
    path: item.fullPath
  })
}
</script>

<style lang="scss" scoped></style>
