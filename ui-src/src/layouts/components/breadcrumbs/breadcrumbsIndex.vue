<template>
  <q-breadcrumbs style="font-size: 16px">
    <q-breadcrumbs-el exact-active-class="text-secondary" v-for="item in matchedRoutes" :key="item.path"
      :label="item.meta.label" :to="item.path" :icon="item.meta.icon" />
  </q-breadcrumbs>
</template>

<script lang="ts" setup>
const route = useRoute()
const matchedRoutes = computed(() => {
  // 获取路由的全路径
  const results = route.matched
  return results
})
</script>

<style lang="scss" scoped>
::v-deep(.q-breadcrumbs__el:hover) {
  color: $accent;
}
</style>
