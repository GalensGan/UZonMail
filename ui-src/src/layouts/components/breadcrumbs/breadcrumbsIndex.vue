<template>
  <q-breadcrumbs style="font-size: 15px">
    <q-breadcrumbs-el v-for="item in matchedRoutes" :key="item.path" class="animated fadeInDown"
      exact-active-class="text-secondary" :to="item.path" :icon="item.meta.icon" :label="item.meta.label">
    </q-breadcrumbs-el>
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
  color: $negative;
}
</style>
