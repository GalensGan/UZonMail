<template>
  <q-expansion-item v-if="!hideMenu && existChildren" v-model="openExpansionItem" active-class="text-orange"
    class="rounded-borders" :class="{ 'text-orange': isActive }" expand-separator :icon="icon" :label="label">
    <MenuItem v-for="child in children" :key="child.path" :routeRaw="child">
    </MenuItem>
  </q-expansion-item>
  <q-item v-else-if="!hideMenu" :active="isActive" :class="{ 'text-orange': isActive, 'text-primary': !isActive }"
    clickable v-ripple>
    <q-item-section avatar>
      <q-icon :name="icon" />
    </q-item-section>
    <q-item-section>{{ label }}</q-item-section>
  </q-item>
</template>

<script lang="ts" setup>
import { RouteRecordRaw } from 'vue-router'

// props 参数
const props = defineProps({
  // 路由的名称，是必须项
  routeRaw: {
    type: Object as () => RouteRecordRaw,
    required: true
  }
})

const { name, children, meta: { label, icon, hideMenu } } = props.routeRaw
const existChildren = computed(() => children && children.length > 0)

// 判断当前菜单是否处于激活状态
const route = useRoute()
const isActive = computed(() => {
  return name === route.name
})

// 根据路由的变化，展开或关闭当前菜单
// 若等于当前菜单的 name，则关闭
// 若是下级，则展开
const openExpansionItem = ref(false)
watch(route, () => {
  if (name === route.name) {
    openExpansionItem.value = false
    return
  }

  const matched = route.matched.find(x => x.name === name)
  openExpansionItem.value = !!matched
}, { immediate: true })
</script>

<script lang="ts">
export default {
  name: 'MenuItem'
}
</script>

<style lang="scss" scoped>
.menu-item__active {
  color: $orange;
}

.menu-item__default {
  color: $primary;
}
</style>
