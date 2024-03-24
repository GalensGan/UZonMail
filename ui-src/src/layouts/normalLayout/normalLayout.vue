<template>
  <q-layout view="lHh lpR fFf">
    <q-header class="bg-white text-primary q-ma-md q-card">

      <q-toolbar class="row items-center full-width">
        <MenuOpenButton v-model="drawer" />

        <q-toolbar-title>
          <BreadcrumbsIndex />
        </q-toolbar-title>

        <UserInfo />
      </q-toolbar>

      <q-separator></q-separator>

      <TagsView />
    </q-header>

    <q-drawer v-model="drawer" show-if-above :mini="miniState">
      <MenuTree />
    </q-drawer>
    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
// 导入组件
import MenuOpenButton from '../components/menuOpen/menuOpenButton.vue'
import UserInfo from '../components/userInfo/userInfo.vue'
import BreadcrumbsIndex from '../components/breadcrumbs/breadcrumbsIndex.vue'
import TagsView from '../components/tags/tagsView.vue'
// import LeftSidebarIndex from '../components/leftSidebar/leftSidebarIndex.vue'
import MenuTree from '../components/leftSidebar/menuTree.vue'

const drawer = ref(false)
const miniState = ref(false)
// const collapse = ref(false)

// #region 开关左侧抽屉
const $q = useQuasar()
if ($q.platform.is.desktop) {
  drawer.value = true
}
</script>

<style lang="scss" scoped>
.menu-button {
  transform: scale(0.6);
}
</style>
