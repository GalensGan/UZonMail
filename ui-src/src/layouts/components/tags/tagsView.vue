<template>
  <div class="tags-view q-ml-md row items-center justify-start">
    <q-chip class="q-mr-sm route-tag row items-center" v-for="item in routes" :key="item.fullPath"
      :class="getTagClass(item)" square clickable transition-show="jump-right" transition-hide="jump-left"
      @click="goToRoute(item)" :removable="item.showCloseIcon" @mouseenter="mouseenterTag(item)"
      @mouseleave="item.showCloseIcon = false" @remove="removeTag(item)">
      <div>{{ item.label }}</div>
      <ContextMenu :items="tagContextItems" :value="item" />
    </q-chip>
  </div>
</template>

<script lang="ts" setup>
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'

import { IRouteHistory } from './types'
import { useRouteHistories } from './routeHistories'
import { IContextMenuItem } from 'src/components/contextMenu/types'

// 显示和跳转 tag
const routes = useRouteHistories()
function getTagClass (item: IRouteHistory) {
  return {
    'text-primary': item.isActive,
    'text-white': !item.isActive
  }
}
const router = useRouter()
function goToRoute (item: IRouteHistory) {
  router.push({
    path: item.fullPath
  })
}

// tags 功能
// 首页没有移除功能
function mouseenterTag (item: IRouteHistory) {
  if (item.fullPath === '/') return
  item.showCloseIcon = true
}

// 移除按钮
async function removeTag (item: IRouteHistory) {
  // 如果仅有一个且是首页，则不允许删除
  if (routes.value.length === 1 && routes.value[0].fullPath === '/') {
    return
  }

  const currentTagIndex = routes.value.findIndex(x => x.fullPath === item.fullPath)
  routes.value = routes.value.filter((route) => route.fullPath !== item.fullPath)

  // 如果已经没有 tags，则跳转到首页
  if (routes.value.length === 0) {
    router.push({
      path: '/'
    })
    return
  }

  // 向前显示
  if (currentTagIndex - 1 > -1) {
    router.push({
      path: routes.value[currentTagIndex - 1].fullPath
    })
  } else {
    // 显示当前
    router.push({
      path: routes.value[currentTagIndex].fullPath
    })
  }
}

// 右键菜单
const tagContextItems: IContextMenuItem[] = [
  {
    name: 'close',
    label: '关闭',
    tooltip: '关闭当前标签',
    onClick: params => removeTag(params as IRouteHistory)
  }, {
    name: 'closeOther',
    label: '关闭其他',
    onClick: async (params) => {
      const current = params as IRouteHistory
      routes.value = routes.value.filter((route) => route.fullPath === current.fullPath)
      // 激活当前
      router.push({
        path: current.fullPath
      })
    }
  }, {
    name: 'closeAll',
    label: '关闭所有',
    onClick: async () => {
      routes.value = []
      router.push({
        path: '/'
      })
    }
  }
]
</script>

<style lang="scss" scoped></style>
