<template>
  <q-breadcrumbs active-color="white" style="font-size: 16px">
    <q-breadcrumbs-el exact-active-class="text-warning" v-for="item in matchedRoutes" :key="item.path"
      :label="item.meta.label" :to="item.path" :icon="item.meta.icon" />
  </q-breadcrumbs>
</template>

<script lang="ts" setup>
const route = useRoute()
const matchedRoutes = computed(() => {
  // 获取路由的全路径
  const results = route.matched
  console.log('results', results)
  return results
})
</script>

<style lang="scss" scoped></style>
